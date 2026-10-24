using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A fixed "now" for handlers that stamp times; time-zone conversion is not needed by any handler test.</summary>
public sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow { get; set; } = new(2026, 10, 23, 9, 0, 0, TimeSpan.Zero);

    public DateTime ToLocal(DateTimeOffset utc, string timeZoneId) => throw new NotSupportedException();

    public DateTimeOffset FromLocal(DateTime local, string timeZoneId) => throw new NotSupportedException();
}
