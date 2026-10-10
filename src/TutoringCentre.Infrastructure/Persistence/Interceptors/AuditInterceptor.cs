using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Audit;

namespace TutoringCentre.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Builds one append-only <see cref="AuditEntry"/> for each tracked change to an audited entity (Task 31.4's
/// <see cref="AuditPolicy"/>), before the save, so it shares the change's transaction (Day 31 contract). Runs
/// after <see cref="TimestampInterceptor"/> and before <see cref="TenantWriteGuardInterceptor"/>, which therefore
/// also checks the audit rows added here. No handler knows this runs.
/// </summary>
internal sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IClock _clock;
    private readonly ICurrentActor _currentActor;
    private readonly ICorrelationContext _correlationContext;

    public AuditInterceptor(IClock clock, ICurrentActor currentActor, ICorrelationContext correlationContext)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(currentActor);
        ArgumentNullException.ThrowIfNull(correlationContext);
        _clock = clock;
        _currentActor = currentActor;
        _correlationContext = correlationContext;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        RecordAuditEntries(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        RecordAuditEntries(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void RecordAuditEntries(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var actor = _currentActor.Actor;
        var actorCentreId = actor switch
        {
            StaffActor staffActor => staffActor.CentreId,
            SystemActor systemActor => systemActor.CentreId,
            _ => null,
        };

        // Platform bootstrap (centre creation, the development identity seeder) and anything with no actor
        // centre is logged, not audited (the Day 31 contract).
        if (actorCentreId is not { } centreId)
        {
            return;
        }

        var (actorType, actorUserId) = actor switch
        {
            StaffActor staffActor => (AuditActorType.Staff, (Guid?)staffActor.UserId),
            SystemActor => (AuditActorType.System, (Guid?)null),
            _ => throw new InvalidOperationException("An actor with a centre must be staff or system."),
        };

        var now = _clock.UtcNow;
        var correlationId = _correlationContext.CorrelationId;

        // Snapshot first: adding AuditEntry entries below must not disturb the set of entries being examined.
        var trackedEntries = context.ChangeTracker.Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in trackedEntries)
        {
            if (!AuditPolicy.Entries.TryGetValue(entry.Entity.GetType(), out var policy))
            {
                continue;
            }

            // Platform bootstrap aside, an actor only ever writes inside its own centre (the tenant write guard
            // enforces this); this is the other half of "logged, not audited" — the entity's own centre must
            // match too, since for Centre itself that centre is the entity's own id, not the actor's.
            if (policy.ResolveCentreId(entry.Entity) != centreId)
            {
                continue;
            }

            var action = entry.State switch
            {
                EntityState.Added => AuditAction.Created,
                EntityState.Modified => AuditAction.Updated,
                EntityState.Deleted => AuditAction.Deleted,
                _ => throw new InvalidOperationException("Unreachable: filtered to Added, Modified or Deleted above."),
            };

            var propertyChanges = entry.Properties
                .Select(property => new AuditPropertyChange(
                    property.Metadata.Name,
                    property.OriginalValue,
                    property.CurrentValue,
                    property.IsModified))
                .ToList();

            var changes = AuditDiffBuilder.Build(action, policy.AllowedFields, propertyChanges);
            if (changes is null)
            {
                continue;
            }

            context.Add(new AuditEntry
            {
                Id = Guid.CreateVersion7(),
                CentreId = centreId,
                OccurredAt = now,
                ActorType = actorType,
                ActorUserId = actorUserId,
                Action = action,
                EntityType = policy.EntityType,
                EntityId = ((Entity)entry.Entity).Id,
                Changes = changes,
                CorrelationId = correlationId,
            });
        }
    }
}
