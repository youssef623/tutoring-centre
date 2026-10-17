using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// A person who teaches groups at a centre. Deliberately not the staff user: a centre can list teachers who
/// never sign in, and a staff user's role can change without the teaching record disappearing. The optional
/// link to a staff login is an ID only (<see cref="UserId"/>), the same way <c>Membership.UserId</c> is — this
/// type never holds a user object. Whether that user is actually a Teacher-role member of this centre is not
/// this type's concern (Identity's data, checked through a published contract); neither is whether the linked
/// user's role later changes. The only way to obtain one is <see cref="Create"/>.
/// </summary>
public sealed partial class Teacher : Entity, ITenantOwned
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;

    private Teacher(Guid centreId, string fullName, PhoneNumber? phone)
    {
        CentreId = centreId;
        Phone = phone;
        Status = TeacherStatus.Active;
        SetName(fullName);
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private Teacher()
    {
        FullName = string.Empty;
        SearchName = string.Empty;
    }

    /// <summary>The centre this teacher belongs to; fixed at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string FullName { get; private set; }

    /// <summary>Normalized form of <see cref="FullName"/> for matching, never shown to users.</summary>
    public string SearchName { get; private set; }

    public PhoneNumber? Phone { get; private set; }

    /// <summary>The linked staff user's ID, or null when no account is linked. Never a user object.</summary>
    public Guid? UserId { get; private set; }

    public TeacherStatus Status { get; private set; }

    public static Result<Teacher> Create(Guid centreId, string fullName, PhoneNumber? phone)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Teacher>.Failure(Error.Validation("teacher.centre_required", "A centre is required."));
        }

        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result<Teacher>.Failure(cleanedName.Error!);
        }

        return Result<Teacher>.Success(new Teacher(centreId, cleanedName.Value, phone));
    }

    public Result UpdateDetails(string fullName, PhoneNumber? phone)
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

        SetName(cleanedName.Value);
        Phone = phone;
        return Result.Success();
    }

    /// <summary>Links this teacher to a staff user's ID. Linking the user already linked succeeds and changes nothing.</summary>
    public Result LinkUser(Guid userId)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        if (userId == Guid.Empty)
        {
            return Result.Failure(Error.Validation("teacher.user_required", "A user is required."));
        }

        UserId = userId;
        return Result.Success();
    }

    /// <summary>Clears the account link. Unlinking an already-unlinked teacher succeeds and changes nothing.</summary>
    public Result UnlinkUser()
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        UserId = null;
        return Result.Success();
    }

    /// <summary>Takes the teacher out of use without deleting it. The account link is kept, so restoring brings its scope back.</summary>
    public Result Archive()
    {
        if (Status == TeacherStatus.Archived)
        {
            return Result.Failure(Error.Rule("teacher.already_archived", "This teacher is already archived."));
        }

        Status = TeacherStatus.Archived;
        return Result.Success();
    }

    public Result Restore()
    {
        if (Status == TeacherStatus.Active)
        {
            return Result.Failure(Error.Rule("teacher.not_archived", "This teacher is not archived."));
        }

        Status = TeacherStatus.Active;
        return Result.Success();
    }

    private Result CheckNotArchived() => Status == TeacherStatus.Archived
        ? Result.Failure(Error.Rule("teacher.archived_read_only", "An archived teacher is read-only until it is restored."))
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
            var fields = new Dictionary<string, string[]> { ["fullName"] = ["Teacher name must be 2-120 characters."] };
            return Result<string>.Failure(
                Error.Validation("teacher.name_invalid", "Teacher name must be 2-120 characters.", fields));
        }

        return Result<string>.Success(cleaned);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}
