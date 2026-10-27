namespace TutoringCentre.Domain.Attendance;

public static class AttendanceWindow
{
    public static readonly TimeSpan OpensBefore = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan ClosesAfter = TimeSpan.FromHours(2);

    public static AttendanceWindowState StateAt(AttendanceSessionTiming timing, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(timing);
        if (now < timing.StartsAt - OpensBefore)
        {
            return AttendanceWindowState.NotOpen;
        }

        return now > timing.EndsAt + ClosesAfter ? AttendanceWindowState.Closed : AttendanceWindowState.Open;
    }

    public static bool HasStarted(AttendanceSessionTiming timing, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(timing);
        return now >= timing.StartsAt;
    }
}
