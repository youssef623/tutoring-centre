using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Student: no-tracking projections straight to DTOs. No tenant predicate written by hand — the
/// EF query filter and row-level security scope <c>students</c>, <c>student_parents</c> and <c>parents</c> the
/// same way, so the search and detail joins can never reach another centre's row on any of the three tables.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed partial class StudentReadService(AppDbContext db) : IStudentReadService
{
    public async Task<StudentPageDto> ListAsync(StudentSearchCriteria criteria, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var query = db.Set<Student>().AsNoTracking();

        if (!criteria.IncludeArchived)
        {
            query = query.Where(student => student.Status == StudentStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var phoneFragment = ExtractPhoneFragment(criteria.Search);
            if (phoneFragment is not null)
            {
                var matchingIds = await MatchByPhoneFragmentAsync(phoneFragment, ct);
                query = query.Where(student => matchingIds.Contains(student.Id));
            }
            else
            {
                var namePattern = "%" + EscapeLike(SearchText.Normalize(criteria.Search)) + "%";
                query = query.Where(student => EF.Functions.Like(student.SearchName, namePattern));
            }
        }

        if (criteria.Cursor is not null && StudentCursor.TryDecode(criteria.Cursor, out var after))
        {
            query = query.Where(student =>
                student.SearchName.CompareTo(after.SearchName) > 0
                || (student.SearchName == after.SearchName && student.Id.CompareTo(after.Id) > 0));
        }

        var rows = await query
            .OrderBy(student => student.SearchName)
            .ThenBy(student => student.Id)
            .Take(criteria.PageSize + 1)
            .Select(student => new { student.Id, student.FullName, student.SchoolYear, student.Status, student.SearchName })
            .ToListAsync(ct);

        var hasNextPage = rows.Count > criteria.PageSize;
        var page = hasNextPage ? rows.Take(criteria.PageSize).ToList() : rows;

        var studentIds = page.Select(row => row.Id).ToList();
        var primaryParents = await db.Set<StudentParentLink>()
            .AsNoTracking()
            .Where(link => link.IsPrimary && studentIds.Contains(link.StudentId))
            .Join(db.Set<Parent>().AsNoTracking(), link => link.ParentId, parent => parent.Id, (link, parent) => new { link.StudentId, parent.FullName, parent.Phone })
            .ToListAsync(ct);
        var primaryByStudent = primaryParents.ToDictionary(row => row.StudentId);

        var items = page
            .Select(row =>
            {
                primaryByStudent.TryGetValue(row.Id, out var primary);
                return new StudentListItemDto(row.Id, row.FullName, row.SchoolYear, row.Status, primary?.FullName, primary?.Phone.Value);
            })
            .ToList();

        var nextCursor = hasNextPage ? StudentCursor.Encode(page[^1].SearchName, page[^1].Id) : null;
        return new StudentPageDto(items, nextCursor);
    }

    public async Task<StudentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct)
    {
        var student = await db.Set<Student>()
            .AsNoTracking()
            .Where(student => student.Id == id)
            .Select(student => new
            {
                student.Id, student.FullName, student.Phone, student.SchoolYear, student.Status,
                Version = EF.Property<uint>(student, "Version"),
            })
            .SingleOrDefaultAsync(ct);

        if (student is null)
        {
            return null;
        }

        var parents = await db.Set<StudentParentLink>()
            .AsNoTracking()
            .Where(link => link.StudentId == id)
            .Join(
                db.Set<Parent>().AsNoTracking(),
                link => link.ParentId,
                parent => parent.Id,
                (link, parent) => new { link.ParentId, parent.FullName, parent.Phone, link.Relationship, link.IsPrimary })
            .OrderByDescending(joined => joined.IsPrimary)
            .ToListAsync(ct);

        var parentDtos = parents
            .Select(row => new StudentParentDto(row.ParentId, row.FullName, row.Phone.Value, row.Relationship, row.IsPrimary))
            .ToList();

        return new StudentDetailDto(student.Id, student.FullName, student.Phone?.Value, student.SchoolYear, student.Status, student.Version, parentDtos);
    }

    /// <summary>
    /// Ids of students whose own phone or a linked parent's phone contains <paramref name="fragment"/>.
    /// Reduced from a SQL LIKE to an in-memory scan: EF Core cannot translate a LIKE against
    /// <see cref="PhoneNumber"/>'s converted column (confirmed empirically — both a direct member access and
    /// <c>EF.Property&lt;string&gt;</c> fail, the latter because the generated LIKE parameter still gets run
    /// through the column's string-to-<see cref="PhoneNumber"/> converter and throws). Each query here is
    /// still tenant-scoped by the normal global filter; only the fragment match itself happens in C#, on the
    /// same materialize-then-read-<c>.Value</c> pattern already used elsewhere in this file and in Day 39's
    /// ParentReadService. Fine at a tutoring centre's scale; would need revisiting if a centre's roster grew
    /// into the tens of thousands.
    /// </summary>
    private async Task<List<Guid>> MatchByPhoneFragmentAsync(string fragment, CancellationToken ct)
    {
        var ownPhones = await db.Set<Student>()
            .AsNoTracking()
            .Where(student => student.Phone != null)
            .Select(student => new { student.Id, student.Phone })
            .ToListAsync(ct);
        var matchingByOwnPhone = ownPhones
            .Where(row => row.Phone!.Value.Contains(fragment, StringComparison.Ordinal))
            .Select(row => row.Id);

        var parentPhones = await db.Set<StudentParentLink>()
            .AsNoTracking()
            .Join(db.Set<Parent>().AsNoTracking(), link => link.ParentId, parent => parent.Id, (link, parent) => new { link.StudentId, parent.Phone })
            .ToListAsync(ct);
        var matchingByParentPhone = parentPhones
            .Where(row => row.Phone.Value.Contains(fragment, StringComparison.Ordinal))
            .Select(row => row.StudentId);

        return matchingByOwnPhone.Concat(matchingByParentPhone).Distinct().ToList();
    }

    /// <summary>
    /// A term made only of digits, '+' and the separators PhoneNumber itself strips, with at least four
    /// digits, is a phone fragment (Day 40 contract). Returns the digits-and-plus-only form to match against,
    /// or null when the term does not qualify — in which case it is searched as a name instead.
    /// </summary>
    private static string? ExtractPhoneFragment(string term)
    {
        var trimmed = term.Trim();
        if (trimmed.Length == 0 || !PhoneFragmentShape().IsMatch(trimmed))
        {
            return null;
        }

        var digitCount = 0;
        var builder = new StringBuilder(trimmed.Length);
        foreach (var c in trimmed)
        {
            if (char.IsDigit(c))
            {
                digitCount++;
                builder.Append(c);
            }
            else if (c == '+')
            {
                builder.Append(c);
            }
        }

        return digitCount >= 4 ? builder.ToString() : null;
    }

    // Backslash is doubled first so the characters it introduces for % and _ are not themselves re-escaped.
    private static string EscapeLike(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("%", "\\%", StringComparison.Ordinal).Replace("_", "\\_", StringComparison.Ordinal);

    // Digits, '+', spaces, hyphens, dots and parentheses only — the same separators PhoneNumber.Create strips.
    [GeneratedRegex(@"^[0-9+\s\-.()]+$")]
    private static partial Regex PhoneFragmentShape();
}
