using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff;

/// <summary>
/// Write-side persistence port for <see cref="Membership"/>. identity.memberships is Month 1's documented
/// exemption from tenant filtering — no EF query filter, no row-level security, because a membership is read
/// at login, before a tenant is selected. Every capability here therefore takes the centre explicitly and
/// applies it itself; there is deliberately no overload that omits it.
/// </summary>
public interface IMembershipRepository
{
    /// <summary>
    /// Loads one membership for modification, scoped to <paramref name="centreId"/> and expecting it to still
    /// be at <paramref name="expectedVersion"/> — the save, not this call, detects a stale version. Null when
    /// the ID is unknown or belongs to another centre; the two are indistinguishable on purpose.
    /// </summary>
    Task<Membership?> GetForUpdateAsync(Guid id, Guid centreId, uint expectedVersion, CancellationToken ct);

    /// <summary>Whether the user already has a membership in the centre, active or inactive.</summary>
    Task<bool> ExistsForUserAsync(Guid userId, Guid centreId, CancellationToken ct);

    /// <summary>
    /// Locks the centre's active owner rows (<c>SELECT ... FOR UPDATE</c>) for the rest of the current
    /// transaction and returns how many there are. A concurrent transaction demoting another owner in the same
    /// centre waits here, then sees this transaction's committed result — the one way to protect an invariant
    /// ("at least one active owner") that spans rows a single row's version cannot.
    /// </summary>
    Task<int> LockActiveOwnersAsync(Guid centreId, CancellationToken ct);

    /// <summary>Tracks a new membership; the dispatcher saves it.</summary>
    void Add(Membership membership);
}
