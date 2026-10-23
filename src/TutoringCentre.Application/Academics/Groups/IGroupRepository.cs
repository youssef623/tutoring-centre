using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups;

/// <summary>
/// Write-side persistence port for the Group aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query
/// filter and row-level security, so this interface cannot be called with the wrong tenant.
/// </summary>
public interface IGroupRepository
{
    /// <summary>
    /// Loads one group for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such group is visible to the acting
    /// actor's centre.
    /// </summary>
    Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one group without a version check. Returns null when it does not exist or is not visible to the acting centre.</summary>
    Task<Group?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>Whether a group with this normalized name already exists in the acting centre, optionally excluding one group (for rename).</summary>
    Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct);

    /// <summary>Whether the acting centre has an active group of this subject.</summary>
    Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct);

    /// <summary>Whether the acting centre has an active group taught by this teacher.</summary>
    Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct);

    /// <summary>Tracks a new group; the dispatcher saves it.</summary>
    void Add(Group group);
}
