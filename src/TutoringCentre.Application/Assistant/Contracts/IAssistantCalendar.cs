using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Ports;
namespace TutoringCentre.Application.Assistant.Contracts;
public interface IAssistantCalendar
{
    Task<AssistantCalendarDate> GetAsync(CancellationToken ct);
}
public sealed record AssistantCalendarDate(DateOnly Today, TimeZoneInfo Zone);
