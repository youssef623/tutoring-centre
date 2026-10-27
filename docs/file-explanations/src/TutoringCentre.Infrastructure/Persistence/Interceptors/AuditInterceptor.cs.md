# `src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs`

## Purpose

The scoped interceptor snapshots tracked changes, consults explicit audit policy, verifies centre ownership, builds allowed property diffs, and adds audit rows to the same context. It uses one clock instant plus actor and correlation context. Platform bootstrap without a centre is logged rather than audited. Bulk writes that bypass tracking do not receive this behavior.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 143 lines and 5,584 bytes. Source hash: `cfdee416ed6f138a34ead459d5d283aa437938aa07b270cc24d307352e77f30b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Diagnostics;`
- Line 3: `using TutoringCentre.Application.Common.Ports;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Infrastructure.Audit;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L16): `internal sealed class AuditInterceptor : SaveChangesInterceptor`
- [Line 22](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L22): `public AuditInterceptor(IClock clock, ICurrentActor currentActor, ICorrelationContext correlationContext)`
- [Line 49](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L49): `private void RecordAuditEntries(DbContext? context)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:87](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L87)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 53 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
