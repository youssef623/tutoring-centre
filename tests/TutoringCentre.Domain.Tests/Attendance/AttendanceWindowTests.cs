using System.Globalization;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Domain.Tests.Attendance;

public sealed class AttendanceWindowTests
{
    private static DateTimeOffset At(string value) => DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
    private static readonly AttendanceSessionTiming Timing = new(At("2026-12-05T16:00Z"), At("2026-12-05T17:30Z"), false);

    [Theory]
    [InlineData("2026-12-05T15:29:59Z", AttendanceWindowState.NotOpen)]
    [InlineData("2026-12-05T15:30Z", AttendanceWindowState.Open)]
    [InlineData("2026-12-05T19:30Z", AttendanceWindowState.Open)]
    [InlineData("2026-12-05T19:30:01Z", AttendanceWindowState.Closed)]
    public void StateAt_LiteralBoundary_ReturnsExpected(string now, AttendanceWindowState expected) =>
        Assert.Equal(expected, AttendanceWindow.StateAt(Timing, At(now)));

    [Theory]
    [InlineData("2026-12-05T15:59:59Z", false)]
    [InlineData("2026-12-05T16:00Z", true)]
    public void HasStarted_LiteralBoundary_ReturnsExpected(string now, bool expected) =>
        Assert.Equal(expected, AttendanceWindow.HasStarted(Timing, At(now)));

    [Theory]
    [InlineData("2026-12-05T15:59Z")]
    [InlineData("2026-12-05T16:00Z")]
    public void Timing_EndNotAfterStart_Throws(string end) =>
        Assert.Throws<ArgumentException>(() => new AttendanceSessionTiming(Timing.StartsAt, At(end), false));

    [Fact]
    public void StateAt_CancelledSession_DoesNotChangeWindow() =>
        Assert.Equal(AttendanceWindowState.Open, AttendanceWindow.StateAt(new(Timing.StartsAt, Timing.EndsAt, true), At("2026-12-05T16:00Z")));
}
