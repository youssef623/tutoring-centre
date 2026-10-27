# `src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs`

## Purpose

The list adapter applies archive/search/cursor criteria, retrieves one extra ordered row, then loads primary-parent data for page IDs. Name matching executes through normalized escaped LIKE; phone-fragment matching intentionally materializes tenant-scoped phone projections because the value conversion prevented the attempted SQL translation. Its memory work has a documented scale limit.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 189 lines and 8,628 bytes. Source hash: `c4192f20f1d85d63e1218f9a5d1585b7acd39e2a381bb57260b2eff3f9347b38`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using System.Text;`
- Line 3: `using System.Text.RegularExpressions;`
- Line 4: `using Microsoft.EntityFrameworkCore;`
- Line 5: `using TutoringCentre.Application.Academics.Students;`
- Line 6: `using TutoringCentre.Domain.Academics;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L18): `internal sealed partial class StudentReadService(AppDbContext db) : IStudentReadService`
- [Line 20](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L20): `public async Task<StudentPageDto> ListAsync(StudentSearchCriteria criteria, CancellationToken ct)`
- [Line 83](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L83): `public async Task<StudentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct)`
- [Line 129](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L129): `private async Task<List<Guid>> MatchByPhoneFragmentAsync(string fragment, CancellationToken ct)`
- [Line 156](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L156): `private static string? ExtractPhoneFragment(string term)`
- [Line 183](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L183): `private static string EscapeLike(string value) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:140](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L140)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs:36](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs#L36)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 99 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
