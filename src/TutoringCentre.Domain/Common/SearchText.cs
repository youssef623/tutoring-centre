using System.Text.RegularExpressions;

namespace TutoringCentre.Domain.Common;

/// <summary>
/// Normalizes a name into a form used for matching only, never for display. Applied identically to a stored
/// name (by its owning entity) and to a search term (by the read side, Day 40), so the two sides cannot drift
/// apart.
/// </summary>
public static partial class SearchText
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var collapsed = WhitespacePattern().Replace(value.Trim(), " ");
        var lowered = collapsed.ToLowerInvariant();
        var withoutDiacritics = DiacriticsPattern().Replace(lowered, string.Empty);

        var buffer = new char[withoutDiacritics.Length];
        for (var i = 0; i < withoutDiacritics.Length; i++)
        {
            buffer[i] = MapCharacter(withoutDiacritics[i]);
        }

        return new string(buffer);
    }

    private static char MapCharacter(char c) => c switch
    {
        'أ' or 'إ' or 'آ' or 'ٱ' => 'ا',
        'ى' => 'ي',
        'ة' => 'ه',
        'ؤ' => 'و',
        'ئ' => 'ي',
        >= '٠' and <= '٩' => (char)('0' + (c - '٠')), // Arabic-Indic digits
        >= '۰' and <= '۹' => (char)('0' + (c - '۰')), // Extended Arabic-Indic digits
        _ => c,
    };

    // Any run of whitespace collapses to a single space.
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();

    // Arabic diacritics (U+064B-U+0652, U+0670) and tatweel (U+0640).
    [GeneratedRegex(@"[ً-ْٰـ]")]
    private static partial Regex DiacriticsPattern();
}
