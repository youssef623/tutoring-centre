using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Refuses to save a tracked <see cref="ITenantOwned"/> entry (Added, Modified or Deleted) unless it belongs to
/// the acting actor's centre: the actor has no centre, the entity's centre differs from the actor's centre, or
/// (Modified only) the centre value itself was changed. Reads <see cref="AppDbContext.CurrentCentreId"/> off the
/// context being saved, so it needs no dependency of its own and runs wherever that context is constructed,
/// including the migration path. Runs after <see cref="TimestampInterceptor"/>.
/// </summary>
internal sealed class TenantWriteGuardInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        Guard(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        Guard(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void Guard(DbContext? context)
    {
        if (context is not AppDbContext appDbContext)
        {
            return;
        }

        var currentCentreId = appDbContext.CurrentCentreId;

        foreach (var entry in appDbContext.ChangeTracker.Entries<ITenantOwned>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
            {
                continue;
            }

            if (currentCentreId is null || entry.Entity.CentreId != currentCentreId.Value)
            {
                throw new TenantViolationException(entry.Entity.GetType());
            }

            if (entry.State == EntityState.Modified && entry.Property(nameof(ITenantOwned.CentreId)).IsModified)
            {
                throw new TenantViolationException(entry.Entity.GetType());
            }
        }
    }
}
