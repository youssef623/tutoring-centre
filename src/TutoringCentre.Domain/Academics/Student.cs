using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// A learner registered at a centre. The aggregate root for its parent links: every link is created and
/// changed only through this type's operations, which guard the invariant that a student always has between
/// <see cref="MinParents"/> and <see cref="MaxParents"/> parent links, never two links to the same parent, and
/// exactly one link marked primary. The only way to obtain one is <see cref="Register"/>.
/// </summary>
public sealed partial class Student : Entity, ITenantOwned
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;
    public const int SchoolYearMaxLength = 40;
    public const int MinParents = 1;
    public const int MaxParents = 4;

    private readonly List<StudentParentLink> _parents = [];

    private Student(Guid centreId, string fullName, PhoneNumber? phone, string? schoolYear)
    {
        CentreId = centreId;
        Phone = phone;
        SchoolYear = schoolYear;
        Status = StudentStatus.Active;
        LinksRevision = 0;
        SetName(fullName);
    }

    // For EF Core materialisation (Day 38) only; EF then sets every property from the row.
    private Student()
    {
        FullName = string.Empty;
        SearchName = string.Empty;
    }

    /// <summary>The centre this student belongs to; fixed at registration and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string FullName { get; private set; }

    /// <summary>Normalized form of <see cref="FullName"/> for matching, never shown to users.</summary>
    public string SearchName { get; private set; }

    public PhoneNumber? Phone { get; private set; }

    public string? SchoolYear { get; private set; }

    public StudentStatus Status { get; private set; }

    /// <summary>Increases by exactly one on every successful parent-link change, never on a failure.</summary>
    public int LinksRevision { get; private set; }

    public IReadOnlyCollection<StudentParentLink> Parents => _parents;

    public static Result<Student> Register(
        Guid centreId,
        string fullName,
        PhoneNumber? phone,
        string? schoolYear,
        Guid primaryParentId,
        ParentRelationship relationship)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Student>.Failure(Error.Validation("student.centre_required", "A centre is required."));
        }

        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result<Student>.Failure(cleanedName.Error!);
        }

        var cleanedSchoolYear = ValidateSchoolYear(schoolYear);
        if (cleanedSchoolYear.IsFailure)
        {
            return Result<Student>.Failure(cleanedSchoolYear.Error!);
        }

        if (primaryParentId == Guid.Empty)
        {
            return Result<Student>.Failure(Error.Validation("student.parent_required", "A primary parent is required."));
        }

        var student = new Student(centreId, cleanedName.Value, phone, cleanedSchoolYear.Value);
        student._parents.Add(new StudentParentLink(centreId, student.Id, primaryParentId, relationship, isPrimary: true));
        return Result<Student>.Success(student);
    }

    public Result UpdateDetails(string fullName, PhoneNumber? phone, string? schoolYear)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result.Failure(cleanedName.Error!);
        }

        var cleanedSchoolYear = ValidateSchoolYear(schoolYear);
        if (cleanedSchoolYear.IsFailure)
        {
            return Result.Failure(cleanedSchoolYear.Error!);
        }

        SetName(cleanedName.Value);
        Phone = phone;
        SchoolYear = cleanedSchoolYear.Value;
        return Result.Success();
    }

    public Result AddParent(Guid parentId, ParentRelationship relationship)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        if (parentId == Guid.Empty)
        {
            return Result.Failure(Error.Validation("student.parent_required", "A parent is required."));
        }

        if (_parents.Any(link => link.ParentId == parentId))
        {
            return Result.Failure(Error.Conflict("student.parent_already_linked", "This parent is already linked to this student."));
        }

        if (_parents.Count >= MaxParents)
        {
            return Result.Failure(Error.Rule("student.too_many_parents", "A student can have at most 4 parents."));
        }

        _parents.Add(new StudentParentLink(CentreId, Id, parentId, relationship, isPrimary: false));
        LinksRevision++;
        return Result.Success();
    }

    public Result RemoveParent(Guid parentId)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        var link = _parents.FirstOrDefault(l => l.ParentId == parentId);
        if (link is null)
        {
            return Result.Failure(Error.NotFound("student.parent_not_linked", "This parent is not linked to this student."));
        }

        if (_parents.Count <= MinParents)
        {
            return Result.Failure(Error.Rule("student.last_parent", "A student must keep at least one parent."));
        }

        if (link.IsPrimary)
        {
            return Result.Failure(Error.Rule(
                "student.primary_parent_required", "Make another parent primary before removing the primary parent."));
        }

        _parents.Remove(link);
        LinksRevision++;
        return Result.Success();
    }

    public Result SetPrimaryParent(Guid parentId)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        var link = _parents.FirstOrDefault(l => l.ParentId == parentId);
        if (link is null)
        {
            return Result.Failure(Error.NotFound("student.parent_not_linked", "This parent is not linked to this student."));
        }

        if (link.IsPrimary)
        {
            return Result.Failure(Error.Rule("student.already_primary", "This parent is already the primary contact."));
        }

        var current = _parents.FirstOrDefault(l => l.IsPrimary);
        current?.ClearPrimary();
        link.MarkPrimary();
        LinksRevision++;
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == StudentStatus.Archived)
        {
            return Result.Failure(Error.Rule("student.already_archived", "This student is already archived."));
        }

        Status = StudentStatus.Archived;
        return Result.Success();
    }

    public Result Restore()
    {
        if (Status == StudentStatus.Active)
        {
            return Result.Failure(Error.Rule("student.not_archived", "This student is not archived."));
        }

        Status = StudentStatus.Active;
        return Result.Success();
    }

    private Result CheckNotArchived() => Status == StudentStatus.Archived
        ? Result.Failure(Error.Rule("student.archived_read_only", "An archived student is read-only until it is restored."))
        : Result.Success();

    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(FullName), nameof(SearchName))]
    private void SetName(string cleanedName)
    {
        FullName = cleanedName;
        SearchName = SearchText.Normalize(cleanedName);
    }

    private static Result<string> ValidateName(string? fullName)
    {
        var cleaned = WhitespacePattern().Replace((fullName ?? string.Empty).Trim(), " ");
        if (cleaned.Length < NameMinLength || cleaned.Length > NameMaxLength)
        {
            var fields = new Dictionary<string, string[]> { ["fullName"] = ["Student name must be 2-120 characters."] };
            return Result<string>.Failure(
                Error.Validation("student.name_invalid", "Student name must be 2-120 characters.", fields));
        }

        return Result<string>.Success(cleaned);
    }

    private static Result<string?> ValidateSchoolYear(string? schoolYear)
    {
        if (string.IsNullOrWhiteSpace(schoolYear))
        {
            return Result<string?>.Success(null);
        }

        var cleaned = WhitespacePattern().Replace(schoolYear.Trim(), " ");
        if (cleaned.Length > SchoolYearMaxLength)
        {
            var fields = new Dictionary<string, string[]> { ["schoolYear"] = ["School year must be at most 40 characters."] };
            return Result<string?>.Failure(
                Error.Validation("student.school_year_invalid", "School year must be at most 40 characters.", fields));
        }

        return Result<string?>.Success(cleaned);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}
