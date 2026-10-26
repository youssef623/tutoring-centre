using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Teachers;

/// <summary>
/// Write-side persistence port for the Teacher aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query
/// filter and row-level security, so this interface cannot be called with the wrong tenant.
/// </summary>
public interface ITeacherRepository
{
    /// <summary>
    /// Loads one teacher for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such teacher is visible to the
    /// acting actor's centre.
    /// </summary>
    Task<Teacher?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one teacher without a version check. Returns null when it does not exist or is not visible to the acting centre.</summary>
    Task<Teacher?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>Whether a user is already linked to a teacher in the acting centre, optionally excluding one teacher (for re-linking the same teacher).</summary>
    Task<bool> UserLinkedAsync(Guid userId, Guid? excludingTeacherId, CancellationToken ct);

    /// <summary>Tracks a new teacher; the dispatcher saves it.</summary>
    void Add(Teacher teacher);
}
