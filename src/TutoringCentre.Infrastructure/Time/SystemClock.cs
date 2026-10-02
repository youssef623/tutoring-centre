using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Infrastructure.Time;

/// <summary>IClock adapter: "now" from TimeProvider, conversions from the OS/ICU time-zone database (IANA ids such as "Africa/Cairo").</summary>
public sealed class SystemClock : IClock
{
    private readonly TimeProvider _timeProvider;

    public SystemClock(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

    public DateTimeOffset UtcNow => _timeProvider.GetUtcNow();

    public DateTime ToLocal(DateTimeOffset utc, string timeZoneId)
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTime(utc, zone).DateTime;
    }

    public DateTimeOffset FromLocal(DateTime local, string timeZoneId)
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var wallClock = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        var offset = zone.GetUtcOffset(wallClock);
        return new DateTimeOffset(wallClock, offset).ToUniversalTime();
    }
}
