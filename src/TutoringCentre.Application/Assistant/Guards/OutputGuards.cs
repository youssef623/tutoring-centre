using System.Text.RegularExpressions;
namespace TutoringCentre.Application.Assistant.Guards;
public sealed record GuardVerdict(string? Reason)
{
    public bool Allowed => Reason is null;
}
public static class IdentifierGuard
{
    public static GuardVerdict Check(string text) => new(Regex.IsMatch(text,
        @"(?i)\b[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\b|\b[0-9a-f]{32}\b|\b(?:child_\d+|me|get_attendance)\b|[{}]",
        RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)) ? "guard.identifier" : null);
}
public static class GroundingGuard
{
    public static GuardVerdict Check(string text, string userMessage, IReadOnlyCollection<string> familyNames,
        IReadOnlyCollection<string> centreNames, bool hasAttendance, string toolFacts)
    {
        if (!hasAttendance && Regex.IsMatch(text, @"(?i)(?<!\p{L})(?:حضر|غاب|غياب|متأخر|حاضر|attended|absent|late|present)(?!\p{L})",
            RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))) return new("guard.ungrounded");
        foreach (var name in centreNames.Where(name => !string.IsNullOrWhiteSpace(name)))
            if (text.Contains(name, StringComparison.OrdinalIgnoreCase)
                && !familyNames.Contains(name, StringComparer.OrdinalIgnoreCase)
                && !userMessage.Contains(name, StringComparison.OrdinalIgnoreCase)
                && !toolFacts.Contains(name, StringComparison.OrdinalIgnoreCase)) return new("guard.foreign_name");
        return new(null);
    }
}
public static class FormatGuard
{
    public static GuardVerdict Check(string text, int maximum) => new(string.IsNullOrWhiteSpace(text) || text.Length > maximum
        || Regex.IsMatch(text, @"[<>`*_#\[\]]|(?i)\b(?:https?://|www\.)", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))
        ? "guard.format" : null);
}
/// <summary>Deterministic checks only: this is not a semantic fact checker.</summary>
public static class OutputGuards
{
    public static GuardVerdict Check(string text, string userMessage, IReadOnlyCollection<string> familyNames,
        IReadOnlyCollection<string> centreNames, bool hasAttendance, string toolFacts, int maximum)
    {
        var identifier = IdentifierGuard.Check(text);
        if (!identifier.Allowed) return identifier;
        var grounding = GroundingGuard.Check(text, userMessage, familyNames, centreNames, hasAttendance, toolFacts);
        return grounding.Allowed ? FormatGuard.Check(text, maximum) : grounding;
    }
}
public static class FallbackReplies
{
    public static string For(string userMessage)
    {
        var letters = userMessage.Count(char.IsLetter);
        var arabic = userMessage.Count(c => char.IsLetter(c) && c is >= '\u0600' and <= '\u06ff');
        return arabic > letters / 2 ? "عذراً، لا أستطيع الرد الآن. حاول مرة أخرى قريباً."
            : "Sorry, I cannot reply right now. Please try again soon.";
    }
}
