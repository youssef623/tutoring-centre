namespace TutoringCentre.Application.Attendance;

public interface IAttendanceReadService
{
    Task<IReadOnlyList<SessionAttendanceRecordDto>> ListForSessionAsync(Guid sessionId, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, int>> CountForSessionsAsync(IReadOnlyCollection<Guid> sessionIds, CancellationToken ct);
    Task<AttendancePageDto> ListAsync(AttendanceFilter filter, AttendanceAccess access, string? cursor, int pageSize, CancellationToken ct);
}

public sealed record AttendanceAccess
{
    private AttendanceAccess(bool all, Guid? teacherId) { IsAll = all; TeacherId = teacherId; }
    public static AttendanceAccess All { get; } = new(true, null);
    public static AttendanceAccess None { get; } = new(false, null);
    public static AttendanceAccess OwnSessions(Guid teacherId) => teacherId == Guid.Empty ? None : new(false, teacherId);
    public bool IsAll { get; }
    public Guid? TeacherId { get; }
}

public sealed record AttendanceFilter(Guid? StudentId, Guid? GroupId, DateOnly From, DateOnly To);
public sealed record AttendanceRecordDto(Guid RecordId, string Status, string Source, DateTimeOffset MarkedAt,
    string? MarkedByName, int CorrectionCount, uint Version);
public sealed record SessionAttendanceRecordDto(Guid StudentId, AttendanceRecordDto Record);
public sealed record AttendanceStudentDto(Guid StudentId, string StudentName, AttendanceRecordDto? Record);
public sealed record AttendanceSessionDto(Guid SessionId, string GroupName, string SubjectName, string TeacherName,
    DateTimeOffset StartsAt, DateTimeOffset EndsAt, DateOnly LocalDate, string Status);
public sealed record SessionAttendanceDto(AttendanceSessionDto Session, string TimeZoneId, string Window, IReadOnlyList<AttendanceStudentDto> Items);
public sealed record AttendanceSessionListItemDto(Guid SessionId, Guid GroupId, string GroupName, string SubjectName,
    string TeacherName, DateTimeOffset StartsAt, DateTimeOffset EndsAt, string? Room, string Status, string Window,
    int ExpectedCount, int RecordedCount);
public sealed record AttendanceSessionsDto(DateOnly Date, string TimeZoneId, IReadOnlyList<AttendanceSessionListItemDto> Items);
public sealed record AttendanceHistoryItemDto(Guid RecordId, Guid SessionId, DateOnly LocalDate, DateTimeOffset StartsAt,
    Guid GroupId, string GroupName, string SubjectName, Guid StudentId, string StudentName, string Status, string Source, int CorrectionCount);
public sealed record AttendancePageDto(IReadOnlyList<AttendanceHistoryItemDto> Items, string? NextCursor);
