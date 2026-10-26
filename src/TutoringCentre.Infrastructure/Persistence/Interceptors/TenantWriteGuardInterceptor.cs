using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Audit;

namespace TutoringCentre.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Refuses to save a tracked <see cref="ITenantOwned"/> entry (Added, Modified or Deleted) unless it belongs to
/// the acting actor's centre: the actor has no centre, the entity's centre differs from the actor's centre, or
/// (Modified only) the centre value itself was changed. Also refuses any Modified or Deleted <see cref="AuditEntry"/>
/// outright — an audit row is append-only, so no actor's own centre matching it makes a change to it legitimate.
/// Reads <see cref="AppDbContext.CurrentCentreId"/> off the context being saved, so it needs no dependency of its
/// own and runs wherever that context is constructed, including the migration path. Runs after
/// <see cref="TimestampInterceptor"/> and <see cref="AuditInterceptor"/>, so it also checks the audit rows added there.
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

        foreach (var auditEntry in appDbContext.ChangeTracker.Entries<AuditEntry>())
        {
            if (auditEntry.State is EntityState.Modified or EntityState.Deleted)
            {
                throw new TenantViolationException(auditEntry.Entity.GetType());
            }
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
