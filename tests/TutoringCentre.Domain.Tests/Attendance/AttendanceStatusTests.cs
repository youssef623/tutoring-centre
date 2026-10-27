using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Domain.Tests.Attendance;

public sealed class AttendanceStatusTests
{
    [Theory]
    [InlineData(AttendanceStatus.Present, true)]
    [InlineData(AttendanceStatus.Late, true)]
    [InlineData(AttendanceStatus.Absent, false)]
    [InlineData(AttendanceStatus.Excused, false)]
    public void IsArrival_MatchesStatus(AttendanceStatus status, bool expected) =>
        Assert.Equal(expected, status.IsArrival());
}
