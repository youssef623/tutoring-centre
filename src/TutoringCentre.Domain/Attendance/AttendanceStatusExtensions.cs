namespace TutoringCentre.Domain.Attendance;

public static class AttendanceStatusExtensions
{
    public static bool IsArrival(this AttendanceStatus status) =>
        status is AttendanceStatus.Present or AttendanceStatus.Late;
}
