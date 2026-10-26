using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// An adult the centre contacts about one or more students. A parent has no status: it is never archived or
/// deleted. The only way to obtain one is <see cref="Create"/>, which enforces its invariants.
/// </summary>
public sealed partial class Parent : Entity, ITenantOwned
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;

    private Parent(Guid centreId, string fullName, PhoneNumber phone)
    {
        CentreId = centreId;
        Phone = phone;
        SetName(fullName);
    }

    // For EF Core materialisation (Day 38) only; EF then sets every property from the row.
    private Parent()
    {
        FullName = string.Empty;
        SearchName = string.Empty;
        Phone = null!;
    }

    /// <summary>The centre this parent belongs to; set once at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string FullName { get; private set; }

    /// <summary>Normalized form of <see cref="FullName"/> for matching, never shown to users.</summary>
    public string SearchName { get; private set; }

    public PhoneNumber Phone { get; private set; }

    public static Result<Parent> Create(Guid centreId, string fullName, PhoneNumber phone)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Parent>.Failure(Error.Validation("parent.centre_required", "A centre is required."));
        }

        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result<Parent>.Failure(cleanedName.Error!);
        }

        return Result<Parent>.Success(new Parent(centreId, cleanedName.Value, phone));
    }

    public Result Update(string fullName, PhoneNumber phone)
    {
        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result.Failure(cleanedName.Error!);
        }

        SetName(cleanedName.Value);
        Phone = phone;
        return Result.Success();
    }

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
            var fields = new Dictionary<string, string[]> { ["fullName"] = ["Parent name must be 2-120 characters."] };
            return Result<string>.Failure(
                Error.Validation("parent.name_invalid", "Parent name must be 2-120 characters.", fields));
        }

        return Result<string>.Success(cleaned);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}
