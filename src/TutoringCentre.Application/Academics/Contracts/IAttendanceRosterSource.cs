namespace TutoringCentre.Application.Academics.Contracts;

/// <summary>Tenant-scoped session facts and historical enrollment expectations; names only.</summary>
public interface IAttendanceRosterSource
{
    Task<CardStudent?> GetCardStudentAsync(Guid studentId, CancellationToken ct);
    Task<CardGroup?> GetCardGroupAsync(Guid groupId, CancellationToken ct);
    Task<AttendanceSessionFacts?> GetSessionAsync(Guid sessionId, CancellationToken ct);
    Task<IReadOnlyList<ExpectedStudent>> GetExpectedStudentsAsync(Guid sessionId, CancellationToken ct);
    Task<IReadOnlyList<SessionOnDate>> ListSessionsOnDateAsync(DateOnly localDate, Guid? teacherId, CancellationToken ct);
}

public sealed record CardStudent(Guid StudentId, string FullName, bool IsArchived);
public sealed record CardGroupStudent(Guid StudentId, string FullName);
public sealed record CardGroup(Guid GroupId, string GroupName, IReadOnlyList<CardGroupStudent> Students);

public sealed record AttendanceSessionFacts(Guid SessionId, Guid GroupId, string GroupName, string SubjectName,
    Guid TeacherId, string TeacherName, DateTimeOffset StartsAt, DateTimeOffset EndsAt, DateOnly LocalDate,
    string? Room, bool IsCancelled);

public sealed record ExpectedStudent(Guid StudentId, Guid EnrollmentId, string FullName);

public sealed record SessionOnDate(Guid SessionId, Guid GroupId, string GroupName, string SubjectName,
    Guid TeacherId, string TeacherName, DateTimeOffset StartsAt, DateTimeOffset EndsAt, DateOnly LocalDate,
    string? Room, bool IsCancelled);
