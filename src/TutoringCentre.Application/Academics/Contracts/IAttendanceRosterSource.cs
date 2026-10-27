namespace TutoringCentre.Application.Academics.Contracts;

/// <summary>Tenant-scoped session facts and historical enrollment expectations; names only.</summary>
public interface IAttendanceRosterSource
{
    Task<AttendanceSessionFacts?> GetSessionAsync(Guid sessionId, CancellationToken ct);
    Task<IReadOnlyList<ExpectedStudent>> GetExpectedStudentsAsync(Guid sessionId, CancellationToken ct);
    Task<IReadOnlyList<SessionOnDate>> ListSessionsOnDateAsync(DateOnly localDate, Guid? teacherId, CancellationToken ct);
}

public sealed record AttendanceSessionFacts(Guid SessionId, Guid GroupId, string GroupName, string SubjectName,
    Guid TeacherId, string TeacherName, DateTimeOffset StartsAt, DateTimeOffset EndsAt, DateOnly LocalDate,
    string? Room, bool IsCancelled);

public sealed record ExpectedStudent(Guid StudentId, Guid EnrollmentId, string FullName);

public sealed record SessionOnDate(Guid SessionId, Guid GroupId, string GroupName, string SubjectName,
    Guid TeacherId, string TeacherName, DateTimeOffset StartsAt, DateTimeOffset EndsAt, DateOnly LocalDate,
    string? Room, bool IsCancelled);
