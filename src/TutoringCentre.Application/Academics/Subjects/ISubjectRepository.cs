using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Subjects;

/// <summary>
/// Write-side persistence port for the Subject aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query
/// filter and row-level security (layers 2 and 3), so this interface cannot be called with the wrong tenant.
/// </summary>
public interface ISubjectRepository
{
    /// <summary>
    /// Loads one subject for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such subject is visible to the
    /// acting actor's centre.
    /// </summary>
    Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Whether a subject with this normalized name already exists in the acting centre, optionally excluding one subject (for rename).</summary>
    Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct);

    /// <summary>Tracks a new subject; the dispatcher saves it.</summary>
    void Add(Subject subject);
}
