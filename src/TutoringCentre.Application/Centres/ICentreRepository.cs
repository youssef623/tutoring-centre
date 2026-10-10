using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres;

/// <summary>Write-side persistence port for the Centre aggregate. Methods are named by use; nothing here saves — the dispatcher does.</summary>
public interface ICentreRepository
{
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct);

    /// <summary>
    /// Loads one centre for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no centre with this id exists —
    /// callers that pass the acting actor's own centre id treat that as a bug, not a business outcome.
    /// </summary>
    Task<Centre?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    void Add(Centre centre);
}
