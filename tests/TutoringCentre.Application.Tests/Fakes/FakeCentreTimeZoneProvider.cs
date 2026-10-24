using TutoringCentre.Application.Centres.Contracts;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A settable stand-in for <see cref="ICentreTimeZoneProvider"/>; starts at Cairo, the zone every seeded centre uses.</summary>
public sealed class FakeCentreTimeZoneProvider : ICentreTimeZoneProvider
{
    public TimeZoneInfo Zone { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    public Task<TimeZoneInfo> GetForCurrentCentreAsync(CancellationToken ct) => Task.FromResult(Zone);
}
