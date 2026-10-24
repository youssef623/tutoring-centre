using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Application.Common.Time;

/// <summary>
/// "Today" is not a server date: it is the calendar date in the centre's time zone. At 22:30 UTC on 31 December
/// it is already 1 January in Cairo. Handlers that need a date range ("from today") ask here, with the zone from
/// <see cref="TutoringCentre.Application.Centres.Contracts.ICentreTimeZoneProvider"/>.
/// </summary>
public static class CentreCalendar
{
    public static DateOnly Today(IClock clock, TimeZoneInfo zone)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(zone);
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.UtcNow, zone).DateTime);
    }
}
