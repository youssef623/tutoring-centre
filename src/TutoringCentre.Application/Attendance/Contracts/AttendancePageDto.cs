namespace TutoringCentre.Application.Attendance.Contracts;

public sealed record AttendanceHistoryItemDto(Guid RecordId, Guid SessionId, DateOnly LocalDate, DateTimeOffset StartsAt,
    Guid GroupId, string GroupName, string SubjectName, Guid StudentId, string StudentName, string Status, string Source, int CorrectionCount);
public sealed record AttendancePageDto(IReadOnlyList<AttendanceHistoryItemDto> Items, string? NextCursor);
