using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>A subject one centre teaches. The only way to obtain one is <see cref="Create"/>, which enforces its invariants.</summary>
public sealed partial class Subject : Entity, ITenantOwned
{
    public const int NameMaxLength = 80;

    private Subject(Guid centreId, string name)
    {
        CentreId = centreId;
        SetName(name);
        Status = SubjectStatus.Active;
    }

    // For EF Core materialisation (Day 23) only; EF then sets every property from the row.
    private Subject()
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
    }

    /// <summary>The centre this subject belongs to; set once at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string Name { get; private set; }

    /// <summary>Trimmed, inner-whitespace-collapsed, invariant-culture upper-cased form of <see cref="Name"/>, for case/spacing-insensitive comparison.</summary>
    public string NormalizedName { get; private set; }

    public SubjectStatus Status { get; private set; }

    public static Result<Subject> Create(Guid centreId, string name)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Subject>.Failure(Error.Validation("subject.centre_required", "A centre is required."));
        }

        var cleanedName = ValidateName(name);
        if (cleanedName.IsFailure)
        {
            return Result<Subject>.Failure(cleanedName.Error!);
        }

        return Result<Subject>.Success(new Subject(centreId, cleanedName.Value));
    }

    public Result Rename(string name)
    {
        if (Status == SubjectStatus.Archived)
        {
            return Result.Failure(Error.Rule(
                "subject.archived_read_only", "An archived subject cannot be renamed until it is restored."));
        }

        var cleanedName = ValidateName(name);
        if (cleanedName.IsFailure)
        {
            return Result.Failure(cleanedName.Error!);
        }

        SetName(cleanedName.Value);
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == SubjectStatus.Archived)
        {
            return Result.Failure(Error.Rule("subject.already_archived", "This subject is already archived."));
        }

        Status = SubjectStatus.Archived;
        return Result.Success();
    }

    public Result Restore()
    {
        if (Status == SubjectStatus.Active)
        {
            return Result.Failure(Error.Rule("subject.not_archived", "This subject is not archived."));
        }

        Status = SubjectStatus.Active;
        return Result.Success();
    }

    // The cleaned name is already validated; SetName never fails.
    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(Name), nameof(NormalizedName))]
    private void SetName(string cleanedName)
    {
        Name = cleanedName;
        NormalizedName = cleanedName.ToUpperInvariant();
    }

    private static Result<string> ValidateName(string? name)
    {
        var cleaned = WhitespacePattern().Replace((name ?? string.Empty).Trim(), " ");
        if (cleaned.Length < 1 || cleaned.Length > NameMaxLength)
        {
            var fields = new Dictionary<string, string[]> { ["name"] = ["Subject name must be 1-80 characters."] };
            return Result<string>.Failure(
                Error.Validation("subject.name_invalid", "Subject name must be 1-80 characters.", fields));
        }

        return Result<string>.Success(cleaned);
    }

    // Any run of whitespace (spaces, tabs, newlines) collapses to a single space.
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}
