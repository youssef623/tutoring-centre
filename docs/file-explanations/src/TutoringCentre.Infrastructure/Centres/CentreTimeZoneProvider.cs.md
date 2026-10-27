# `src/TutoringCentre.Infrastructure/Centres/CentreTimeZoneProvider.cs`

## Purpose

Source summary (intent, not independent execution proof): EF Core implementation of `ICentreTimeZoneProvider`. The centre predicate is written by hand, like `TutoringCentre.Infrastructure.ReadServices.CentreReadService`'s: platform.centres is the tenant table itself and has no EF query filter or row-level security, so the only thing narrowing this query to one centre is the explicit centre.Id == centreId, with the id taken from the current actor. The zone ID was validated by Centre.Create, so an unknown ID here is a bug and is allowed to throw. Registered scoped and cached in a field, so the row is read at most once per request.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Centres/CentreTimeZoneProvider.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 1,933 bytes. Source hash: `b6f3aad4e6e0ec78a707cd5e47c6ca4dd5a7a8eb0b242855378dcebe9295d4ce`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Centres.Contracts;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Centres;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../src/TutoringCentre.Infrastructure/Centres/CentreTimeZoneProvider.cs#L19): `internal sealed class CentreTimeZoneProvider(ICurrentActor currentActor, AppDbContext db) : ICentreTimeZoneProvider`
- [Line 23](../../../../../src/TutoringCentre.Infrastructure/Centres/CentreTimeZoneProvider.cs#L23): `public async Task<TimeZoneInfo> GetForCurrentCentreAsync(CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:126](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L126)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Centres;

/// <summary>
/// EF Core implementation of <see cref="ICentreTimeZoneProvider"/>. The centre predicate is written by hand, like
/// <see cref="TutoringCentre.Infrastructure.ReadServices.CentreReadService"/>'s: platform.centres is the tenant
/// table itself and has no EF query filter or row-level security, so the only thing narrowing this query to one
/// centre is the explicit <c>centre.Id == centreId</c>, with the id taken from the current actor. The zone ID was
/// validated by <c>Centre.Create</c>, so an unknown ID here is a bug and is allowed to throw. Registered scoped
/// and cached in a field, so the row is read at most once per request.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CentreTimeZoneProvider(ICurrentActor currentActor, AppDbContext db) : ICentreTimeZoneProvider
{
    private TimeZoneInfo? _cached;

    public async Task<TimeZoneInfo> GetForCurrentCentreAsync(CancellationToken ct)
    {
        if (_cached is not null)
        {
            return _cached;
        }

        var centreId = currentActor.Actor.CentreId
            ?? throw new InvalidOperationException("The current actor has no centre selected, so there is no centre time zone.");

        var timeZoneId = await db.Set<Centre>()
            .AsNoTracking()
            .Where(centre => centre.Id == centreId)
            .Select(centre => centre.TimeZoneId)
            .SingleAsync(ct);

        _cached = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return _cached;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
