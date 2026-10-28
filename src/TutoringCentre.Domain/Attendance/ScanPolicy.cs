namespace TutoringCentre.Domain.Attendance;

public static class ScanPolicy
{
    public static readonly TimeSpan LateAfter = TimeSpan.FromMinutes(15);

    public static AttendanceStatus StatusFor(AttendanceSessionTiming timing, DateTimeOffset scannedAt)
    {
        ArgumentNullException.ThrowIfNull(timing);
        return scannedAt > timing.StartsAt + LateAfter ? AttendanceStatus.Late : AttendanceStatus.Present;
    }
}
