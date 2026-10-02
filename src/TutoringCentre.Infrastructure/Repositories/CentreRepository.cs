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

    public void Add(Centre centre)
    {
        ArgumentNullException.ThrowIfNull(centre);
        db.Set<Centre>().Add(centre);
    }
}
