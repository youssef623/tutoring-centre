using System.Globalization;
using System.Text;
using TutoringCentre.Application.Assistant.Models;
namespace TutoringCentre.Application.Assistant.Context;
public static class ContextBuilder
{
    public static ContextOutput Build(ContextInput input, int historyMessages)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentOutOfRangeException.ThrowIfNegative(historyMessages);
        var children = input.Handles is { Children.Count: > 0 }
            ? string.Join('\n', input.Handles.Children.Select(x => $"{x.Key}: {SafeText(x.Value.Name)}"))
            : "No children linked. Explain that no children are linked to this account and do not attempt a lookup.";
        var system = $"{AssistantPrompt.Version}\n{AssistantPrompt.V2}\nIdentity: {SafeText(Truncate(input.DisplayName, 200))}; role: {input.RoleKind}\nDate: {input.LocalDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}; weekday: {input.LocalDate.DayOfWeek}; time zone: {input.TimeZone}\nChildren:\n{children}";
        var messages = input.History.Where(m => m.Role is ModelRole.User or ModelRole.Assistant)
            .TakeLast(historyMessages).Select(m => new ModelMessage(m.Role, SafeText(Truncate(m.Text ?? "", 1000)))).ToList();
        messages.Add(new ModelMessage(ModelRole.User, SafeText(Truncate(input.InboundText, 1000))));
        return new ContextOutput(system, messages);
    }
    internal static string Truncate(string text, int length) => string.Concat(text.EnumerateRunes().Take(length).Select(r => r.ToString()));
    private static string SafeText(string text) => System.Text.RegularExpressions.Regex.Replace(text,
        @"(?i)\b[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\b", "[identifier removed]", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
}
