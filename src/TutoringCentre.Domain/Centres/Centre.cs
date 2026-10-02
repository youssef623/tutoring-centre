using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Centres;

/// <summary>A tutoring centre — the tenant. The only way to obtain one is <see cref="Create"/>, which enforces its invariants.</summary>
public sealed partial class Centre : Entity
{
    public const int NameMaxLength = 120;
    public const int SlugMinLength = 3;
    public const int SlugMaxLength = 60;

    private Centre(string name, string slug, string timeZoneId, SupportedLocale defaultLocale)
    {
        Name = name;
        Slug = slug;
        TimeZoneId = timeZoneId;
        DefaultLocale = defaultLocale;
    }

    // For EF Core materialisation (Days 8–9) only; EF then sets every property from the row.
    private Centre()
    {
        Name = string.Empty;
        Slug = string.Empty;
        TimeZoneId = string.Empty;
    }

    public string Name { get; private set; }

    public string Slug { get; private set; }

    /// <summary>IANA time zone identifier, e.g. "Africa/Cairo".</summary>
    public string TimeZoneId { get; private set; }

    public SupportedLocale DefaultLocale { get; private set; }

    public static Result<Centre> Create(string name, string slug, string timeZoneId, SupportedLocale defaultLocale)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Centre>.Failure(Error.Validation("centre.name_required", "Centre name is required."));
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > NameMaxLength)
        {
            return Result<Centre>.Failure(
                Error.Validation("centre.name_too_long", "Centre name must be at most 120 characters."));
        }

        if (slug is null
            || slug.Length < SlugMinLength
            || slug.Length > SlugMaxLength
            || !SlugPattern().IsMatch(slug))
        {
            return Result<Centre>.Failure(Error.Validation(
                "centre.slug_invalid",
                "Slug must be 3–60 characters of lowercase letters and digits, with single hyphens between words."));
        }

        if (string.IsNullOrWhiteSpace(timeZoneId) || !TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _))
        {
            return Result<Centre>.Failure(
                Error.Validation("centre.time_zone_invalid", "Time zone is not a recognised time zone identifier."));
        }

        return Result<Centre>.Success(new Centre(trimmedName, slug, timeZoneId, defaultLocale));
    }

    // Lowercase letters and digits; single hyphens only between words. \z (not $): $ would also accept a trailing newline.
    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*\z")]
    private static partial Regex SlugPattern();
}
