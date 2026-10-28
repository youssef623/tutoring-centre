using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Attendance;

internal static class AttendanceView
{
    public static string Window(DateTimeOffset startsAt, DateTimeOffset endsAt, bool cancelled, DateTimeOffset now) =>
        AttendanceWindow.StateAt(new(startsAt, endsAt, cancelled), now) switch
        {
            AttendanceWindowState.NotOpen => "not_open",
            AttendanceWindowState.Open => "open",
            _ => "closed",
        };
}
