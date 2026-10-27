namespace TutoringCentre.Domain.Attendance;

/// <summary>Session facts supplied by the caller, expressed as instants.</summary>
public sealed record AttendanceSessionTiming
{
    public AttendanceSessionTiming(DateTimeOffset startsAt, DateTimeOffset endsAt, bool isCancelled)
    {
        if (endsAt <= startsAt)
        {
            throw new ArgumentException("Session end must be after its start.", nameof(endsAt));
        }

        StartsAt = startsAt;
        EndsAt = endsAt;
        IsCancelled = isCancelled;
    }

    public DateTimeOffset StartsAt { get; }
    public DateTimeOffset EndsAt { get; }
    public bool IsCancelled { get; }
}
