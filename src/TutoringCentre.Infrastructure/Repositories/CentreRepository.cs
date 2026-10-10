using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Centres;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>EF Core implementation of the Centre write-side port. It tracks entities; the unit of work saves them.</summary>
internal sealed class CentreRepository(AppDbContext db) : ICentreRepository
{
    public Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct) =>
        db.Set<Centre>().AnyAsync(centre => centre.Slug == slug, ct);

    public async Task<Centre?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var centre = await db.Set<Centre>().SingleOrDefaultAsync(centre => centre.Id == id, ct);
        if (centre is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time (mapped to xmin):
        // a stale client is refused by zero rows affected, not by a comparison here (same as SubjectRepository).
        db.Entry(centre).Property("Version").OriginalValue = expectedVersion;
        return centre;
    }

    public void Add(Centre centre)
    {
        ArgumentNullException.ThrowIfNull(centre);
        db.Set<Centre>().Add(centre);
    }
}
