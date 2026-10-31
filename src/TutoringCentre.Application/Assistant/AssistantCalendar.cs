using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Assistant.Contracts;
namespace TutoringCentre.Application.Assistant;
internal sealed class AssistantCalendar(ICentreTimeZoneProvider zones, IClock clock) : IAssistantCalendar
{
    public async Task<AssistantCalendarDate> GetAsync(CancellationToken ct)
    {
        var zone = await zones.GetForCurrentCentreAsync(ct);
        return new(DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.UtcNow, zone).DateTime), zone);
    }
}
