# `src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs`

## Purpose

Source summary (intent, not independent execution proof): Stamps the shadow properties "CreatedAt" (Added entries) and "UpdatedAt" (Modified entries) from the clock. The properties are declared in entity configurations, so Domain entities never contain persistence metadata.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 61 lines and 2,097 bytes. Source hash: `583a501be1041a0af96f872228dd00cc8542e63b9777c3d95056333ee092d320`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Diagnostics;`
- Line 3: `using TutoringCentre.Application.Common.Ports;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs#L11): `internal sealed class TimestampInterceptor : SaveChangesInterceptor`
- [Line 18](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs#L18): `public TimestampInterceptor(IClock clock)`
- [Line 41](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs#L41): `private void Stamp(DbContext? context)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:86](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L86)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:170](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L170)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Stamps the shadow properties "CreatedAt" (Added entries) and "UpdatedAt" (Modified entries) from the clock.
/// The properties are declared in entity configurations, so Domain entities never contain persistence metadata.
/// </summary>
internal sealed class TimestampInterceptor : SaveChangesInterceptor
{
    private const string CreatedAt = "CreatedAt";
    private const string UpdatedAt = "UpdatedAt";

    private readonly IClock _clock;

    public TimestampInterceptor(IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Stamp(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = _clock.UtcNow;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added && entry.Metadata.FindProperty(CreatedAt) is not null)
            {
                entry.Property(CreatedAt).CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified && entry.Metadata.FindProperty(UpdatedAt) is not null)
            {
                entry.Property(UpdatedAt).CurrentValue = now;
            }
        }
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
