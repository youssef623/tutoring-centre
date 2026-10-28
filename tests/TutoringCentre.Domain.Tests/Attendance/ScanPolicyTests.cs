using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Domain.Tests.Attendance;

public sealed class ScanPolicyTests
{
    [Theory]
    [InlineData(900, AttendanceStatus.Present)]
    [InlineData(901, AttendanceStatus.Late)]
    [InlineData(-600, AttendanceStatus.Present)]
    public void StatusFor_Boundary_UsesInstants(int seconds, AttendanceStatus expected)
    {
        var start = new DateTimeOffset(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);
        var timing = new AttendanceSessionTiming(start, start.AddHours(1), false);
        Assert.Equal(TimeSpan.FromMinutes(15), ScanPolicy.LateAfter);
        Assert.Equal(expected, ScanPolicy.StatusFor(timing, start.AddSeconds(seconds).ToOffset(TimeSpan.FromHours(2))));
    }
}
