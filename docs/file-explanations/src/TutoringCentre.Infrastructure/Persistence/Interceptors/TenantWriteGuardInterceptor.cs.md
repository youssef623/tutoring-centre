# `src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs`

## Purpose

The save guard rejects changes to existing audit rows and checks every Added, Modified, or Deleted tenant entity against the acting centre. Modified centre properties are forbidden independently of ordinary business edits. It executes after audit generation, so inserted audit entries receive the same ownership check as business rows.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 71 lines and 2,982 bytes. Source hash: `09a8a1b1bff38ab6298520dfc6d2d7b4cefc4b58286b80b80f8761379ce10cca`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Diagnostics;`
- Line 3: `using TutoringCentre.Domain.Common;`
- Line 4: `using TutoringCentre.Infrastructure.Audit;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs#L17): `internal sealed class TenantWriteGuardInterceptor : SaveChangesInterceptor`
- [Line 36](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs#L36): `private static void Guard(DbContext? context)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:88](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L88)
- [src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs:32](../../../../../../src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs#L32)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:171](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L171)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
