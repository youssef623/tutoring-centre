using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// A class students enroll in and sessions are scheduled for. Refers to its subject and teacher by ID only —
/// no navigation property, no <c>ChangeSubject</c>. The subject is fixed at creation; the teacher can change.
/// Whether the subject or teacher is active, name uniqueness, and the enrolled count are not this type's
/// concern — those are handler and repository responsibilities. The only way to obtain one is
/// <see cref="Create"/>.
/// </summary>
public sealed partial class Group : Entity, ITenantOwned
{
    public const int NameMaxLength = 80;
    public const int MinCapacity = 1;
    public const int MaxCapacity = 200;

    private Group(Guid centreId, string name, Guid subjectId, Guid teacherId, int capacity)
    {
        CentreId = centreId;
        SubjectId = subjectId;
        TeacherId = teacherId;
        Capacity = capacity;
        Status = GroupStatus.Active;
        SetName(name);
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private Group()
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
    }

    /// <summary>The centre this group belongs to; fixed at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string Name { get; private set; }

    /// <summary>Normalized form of <see cref="Name"/> for matching, never shown to users.</summary>
    public string NormalizedName { get; private set; }

    /// <summary>The group's subject; fixed at creation and never changeable.</summary>
    public Guid SubjectId { get; private set; }

    public Guid TeacherId { get; private set; }

    public int Capacity { get; private set; }

    public GroupStatus Status { get; private set; }

    public static Result<Group> Create(Guid centreId, string name, Guid subjectId, Guid teacherId, int capacity)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Group>.Failure(Error.Validation("group.centre_required", "A centre is required."));
        }

        if (subjectId == Guid.Empty)
        {
            return Result<Group>.Failure(Error.Validation("group.subject_required", "A subject is required."));
        }

        if (teacherId == Guid.Empty)
        {
            return Result<Group>.Failure(Error.Validation("group.teacher_required", "A teacher is required."));
        }

        var cleanedName = ValidateName(name);
        if (cleanedName.IsFailure)
        {
            return Result<Group>.Failure(cleanedName.Error!);
        }

        var validatedCapacity = ValidateCapacity(capacity);
        if (validatedCapacity.IsFailure)
        {
            return Result<Group>.Failure(validatedCapacity.Error!);
        }

        return Result<Group>.Success(new Group(centreId, cleanedName.Value, subjectId, teacherId, capacity));
    }

    public Result Rename(string name)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        var cleanedName = ValidateName(name);
        if (cleanedName.IsFailure)
        {
            return Result.Failure(cleanedName.Error!);
        }

        SetName(cleanedName.Value);
        return Result.Success();
    }

    public Result ChangeCapacity(int capacity)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        var validatedCapacity = ValidateCapacity(capacity);
        if (validatedCapacity.IsFailure)
        {
            return Result.Failure(validatedCapacity.Error!);
        }

        Capacity = capacity;
        return Result.Success();
    }

    /// <summary>Assigns this group to a different teacher. Assigning the current teacher succeeds and changes nothing.</summary>
    public Result AssignTeacher(Guid teacherId)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        if (teacherId == Guid.Empty)
        {
            return Result.Failure(Error.Validation("group.teacher_required", "A teacher is required."));
        }

        TeacherId = teacherId;
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == GroupStatus.Archived)
        {
            return Result.Failure(Error.Rule("group.already_archived", "This group is already archived."));
        }

        Status = GroupStatus.Archived;
        return Result.Success();
    }

    public Result Restore()
    {
        if (Status == GroupStatus.Active)
        {
            return Result.Failure(Error.Rule("group.not_archived", "This group is not archived."));
        }

        Status = GroupStatus.Active;
        return Result.Success();
    }

    private Result CheckNotArchived() => Status == GroupStatus.Archived
        ? Result.Failure(Error.Rule("group.archived_read_only", "An archived group is read-only until it is restored."))
        : Result.Success();

    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(Name), nameof(NormalizedName))]
    private void SetName(string cleanedName)
    {
        Name = cleanedName;
        NormalizedName = SearchText.Normalize(cleanedName);
    }

    private static Result<string> ValidateName(string? name)
    {
        var cleaned = WhitespacePattern().Replace((name ?? string.Empty).Trim(), " ");
        if (cleaned.Length < 1 || cleaned.Length > NameMaxLength)
        {
            var fields = new Dictionary<string, string[]> { ["name"] = ["Group name must be 1-80 characters."] };
            return Result<string>.Failure(
                Error.Validation("group.name_invalid", "Group name must be 1-80 characters.", fields));
        }

        return Result<string>.Success(cleaned);
    }

    private static Result ValidateCapacity(int capacity)
    {
        if (capacity < MinCapacity || capacity > MaxCapacity)
        {
            var fields = new Dictionary<string, string[]> { ["capacity"] = ["Capacity must be 1-200."] };
            return Result.Failure(Error.Validation("group.capacity_invalid", "Capacity must be 1-200.", fields));
        }

        return Result.Success();
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}
