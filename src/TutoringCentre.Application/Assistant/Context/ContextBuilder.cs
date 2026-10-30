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
        var system = $"{AssistantPrompt.Version}\n{AssistantPrompt.V1}\nIdentity: {Truncate(input.DisplayName, 200)}; role: {input.RoleKind}\nDate: {input.LocalDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}; weekday: {input.LocalDate.DayOfWeek}; time zone: {input.TimeZone}";
        var messages = input.History.Where(m => m.Role is ModelRole.User or ModelRole.Assistant)
            .TakeLast(historyMessages).Select(m => new ModelMessage(m.Role, Truncate(m.Text ?? "", 1000))).ToList();
        messages.Add(new ModelMessage(ModelRole.User, Truncate(input.InboundText, 1000)));
        return new ContextOutput(system, messages);
    }
    internal static string Truncate(string text, int length) => string.Concat(text.EnumerateRunes().Take(length).Select(r => r.ToString()));
}
