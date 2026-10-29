using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Attendance;

public enum ScanOutcome
{
    Recorded,
    AlreadyRecorded,
}

public sealed record ScanResult(ScanOutcome Outcome, Guid StudentId, string StudentName,
    AttendanceStatus Status, DateTimeOffset RecordedAt);
