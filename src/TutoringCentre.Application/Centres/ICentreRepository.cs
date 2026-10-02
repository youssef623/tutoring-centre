using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres;

/// <summary>Write-side persistence port for the Centre aggregate. Methods are named by use; nothing here saves — the dispatcher does.</summary>
public interface ICentreRepository
{
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct);

    void Add(Centre centre);
}
