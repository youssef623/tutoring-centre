using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Enrollments;

/// <summary>
/// Read-side port: returns DTO-shaped data directly. Every method takes a <see cref="TeacherScope"/> and there is
/// no overload without one: an enrollment is a group-owned resource, so the same ownership guard as groups
/// applies, as a predicate on the group row in SQL. Out-of-scope rows never leave the database.
/// </summary>
public interface IEnrollmentReadService
{
    /// <summary>
    /// The group's active enrollments with student names, ordered by search name and capped at
    /// <see cref="Group.MaxCapacity"/>. Null when the group does not exist, is not visible to the acting centre,
    /// or is outside <paramref name="scope"/>.
    /// </summary>
    Task<GroupRosterDto?> GetRosterAsync(Guid groupId, TeacherScope scope, CancellationToken ct);

    /// <summary>
    /// The student's enrollments in groups within <paramref name="scope"/>: active first, then newest first.
    /// Null when the student does not exist or is not visible to the acting centre; an empty list when the
    /// student exists but has no visible enrollment.
    /// </summary>
    Task<IReadOnlyList<StudentEnrollmentDto>?> ListForStudentAsync(Guid studentId, TeacherScope scope, CancellationToken ct);
}

/// <summary>Who holds a seat in a group. Names only: no contact data of students or parents.</summary>
public sealed record GroupRosterDto(Guid GroupId, string GroupName, int Capacity, int EnrolledCount, IReadOnlyList<RosterEntryDto> Items);

public sealed record RosterEntryDto(Guid EnrollmentId, Guid StudentId, string StudentName, DateTimeOffset EnrolledAt, uint Version);

public sealed record StudentEnrollmentDto(
    Guid EnrollmentId,
    Guid GroupId,
    string GroupName,
    string SubjectName,
    string TeacherName,
    EnrollmentStatus Status,
    DateTimeOffset EnrolledAt,
    DateTimeOffset? EndedAt,
    EnrollmentEndReason? EndReason,
    uint Version);
