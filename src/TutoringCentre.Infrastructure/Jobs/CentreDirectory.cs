using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Jobs;

/// <summary>platform.centres has no RLS or query filter. Select only IDs, independently of the actor.</summary>
internal sealed class CentreDirectory(AppDbContext db) : ICentreDirectory
{
    public async Task<IReadOnlyList<Guid>> ListCentreIdsAsync(CancellationToken cancellationToken) =>
        await db.Set<Centre>().AsNoTracking().OrderBy(centre => centre.Id)
            .Select(centre => centre.Id).ToListAsync(cancellationToken);
}
