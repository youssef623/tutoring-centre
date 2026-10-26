using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Teachers;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes the Teacher entity or EF types.</summary>
public interface ITeacherReadService
{
    /// <summary>Teachers ordered by search name, scoped to the acting centre. Capped at 500 rows; there is no paging.</summary>
    Task<IReadOnlyList<TeacherDto>> ListAsync(bool includeArchived, CancellationToken ct);

    /// <summary>One teacher by ID, or null when it does not exist or is not visible to the acting centre.</summary>
    Task<TeacherDto?> GetAsync(Guid id, CancellationToken ct);

    /// <summary>The ID of the active teacher linked to this user in the acting centre, or null when none is linked or the link belongs to an archived teacher.</summary>
    Task<Guid?> FindIdByUserAsync(Guid userId, CancellationToken ct);
}

/// <summary>
/// The one shape every teacher read returns. <see cref="ActiveGroupCount"/> counts the teacher's active groups. The
/// linked account, if any, is shown by display name only — never email, which stays behind <c>staff.view</c>.
/// </summary>
public sealed record TeacherDto(
    Guid Id,
    string FullName,
    string? Phone,
    TeacherStatus Status,
    Guid? UserId,
    string? UserDisplayName,
    int ActiveGroupCount,
    uint Version);
