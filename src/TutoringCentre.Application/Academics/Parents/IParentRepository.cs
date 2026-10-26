using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents;

/// <summary>
/// Write-side persistence port for the Parent aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query
/// filter and row-level security, so this interface cannot be called with the wrong tenant. A parent of
/// another centre is simply not found by any of these.
/// </summary>
public interface IParentRepository
{
    /// <summary>
    /// Loads one parent for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version.
    /// </summary>
    Task<Parent?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one parent for modification with no version expectation — for when a student references it (Day 40).</summary>
    Task<Parent?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>The parent holding this phone in the acting centre, or null.</summary>
    Task<Parent?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct);

    /// <summary>Whether a parent with this phone already exists in the acting centre, optionally excluding one parent (for update).</summary>
    Task<bool> PhoneExistsAsync(PhoneNumber phone, Guid? excludingId, CancellationToken ct);

    /// <summary>Tracks a new parent; the dispatcher saves it.</summary>
    void Add(Parent parent);
}
