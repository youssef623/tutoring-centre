using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups;

/// <summary>
/// Read-side port: returns DTO-shaped data directly. Every method takes a <see cref="TeacherScope"/> and there
/// is no overload without one, so a new read cannot forget the ownership guard without a compile error. The
/// scope is applied in SQL: out-of-scope rows never leave the database.
/// </summary>
public interface IGroupReadService
{
    /// <summary>Groups ordered by normalized name, scoped to the acting centre and to <paramref name="scope"/>. Capped at 500 rows; there is no paging.</summary>
    Task<IReadOnlyList<GroupDto>> ListAsync(GroupListCriteria criteria, TeacherScope scope, CancellationToken ct);

    /// <summary>One group by ID, or null when it does not exist, is not visible to the acting centre, or is outside <paramref name="scope"/>.</summary>
    Task<GroupDto?> GetAsync(Guid id, TeacherScope scope, CancellationToken ct);
}

/// <summary>Optional filters for the group list. Intersected with the scope, never widening it.</summary>
public sealed record GroupListCriteria(Guid? SubjectId, Guid? TeacherId, bool IncludeArchived);

/// <summary>The one shape every group read returns. <see cref="EnrolledCount"/> is the group's active enrollments, counted when read.</summary>
public sealed record GroupDto(
    Guid Id,
    string Name,
    Guid SubjectId,
    string SubjectName,
    Guid TeacherId,
    string TeacherName,
    int Capacity,
    int EnrolledCount,
    GroupStatus Status,
    uint Version);
