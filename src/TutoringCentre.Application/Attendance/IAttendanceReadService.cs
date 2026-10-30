using TutoringCentre.Application.Attendance.Contracts;
namespace TutoringCentre.Application.Attendance;

public interface IAttendanceReadService
{
    Task<IReadOnlyList<SessionAttendanceRecordDto>> ListForSessionAsync(Guid sessionId, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, int>> CountForSessionsAsync(IReadOnlyCollection<Guid> sessionIds, CancellationToken ct);
    Task<AttendancePageDto> ListAsync(AttendanceFilter filter, AttendanceAccess access, string? cursor, int pageSize, CancellationToken ct);
}

public sealed record AttendanceAccess
{
    private AttendanceAccess(bool all, Guid? teacherId, IReadOnlyList<Guid>? studentIds = null)
    { IsAll = all; TeacherId = teacherId; StudentIds = studentIds; }
    public static AttendanceAccess All { get; } = new(true, null);
    public static AttendanceAccess None { get; } = new(false, null);
    public static AttendanceAccess OwnSessions(Guid teacherId) => teacherId == Guid.Empty ? None : new(false, teacherId);
    public static AttendanceAccess Children(IReadOnlyList<Guid> studentIds)
    {
        ArgumentNullException.ThrowIfNull(studentIds);
        return new(false, null, Array.AsReadOnly(studentIds.Distinct().ToArray()));
    }
    public IReadOnlyList<Guid>? StudentIds { get; }
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
