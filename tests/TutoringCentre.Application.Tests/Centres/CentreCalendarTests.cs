using TutoringCentre.Application.Common.Time;
using TutoringCentre.Application.Tests.Fakes;

namespace TutoringCentre.Application.Tests.Centres;

/// <summary>Task 51.6: "today" is the date in the centre's zone, not the UTC or server date.</summary>
public sealed class CentreCalendarTests
{
    private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    [Fact]
    public void Today_At2230UtcOn31December_IsAlready1JanuaryInCairo()
    {
        // 22:30 UTC is 00:30 the next day at Cairo's winter UTC+2.
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 12, 31, 22, 30, 0, TimeSpan.Zero) };

        Assert.Equal(new DateOnly(2027, 1, 1), CentreCalendar.Today(clock, Cairo));
    }

    [Fact]
    public void Today_At2159UtcOn31December_IsStill31DecemberInCairo()
    {
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 12, 31, 21, 59, 0, TimeSpan.Zero) };

        Assert.Equal(new DateOnly(2026, 12, 31), CentreCalendar.Today(clock, Cairo));
    }
}
