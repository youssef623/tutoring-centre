# `src/TutoringCentre.Infrastructure/ReadServices/CentreReadService.cs`

## Purpose

Projects settings and the xmin-backed client version without tracking. Centre rows deliberately have neither tenant query filters nor RLS, so its supplied centre ID is the security boundary; callers must take that ID from the actor rather than untrusted request input.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/CentreReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 1,242 bytes. Source hash: `bbf3a08fc2d1239bead9df48da8ca15f41db7e44ee4715bdeca88685935ec891`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Centres;`
- Line 4: `using TutoringCentre.Domain.Centres;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Infrastructure/ReadServices/CentreReadService.cs#L15): `internal sealed class CentreReadService(AppDbContext db) : ICentreReadService`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/ReadServices/CentreReadService.cs#L17): `public Task<CentreSettingsDto?> GetSettingsAsync(Guid centreId, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:8](../../../../onboarding-work/extend_backend_purposes.py#L8)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:125](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L125)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Centres;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Centre: a flat, no-tracking projection straight to <see cref="CentreSettingsDto"/>. No tenant
/// predicate — platform.centres has no EF query filter or row-level security, so safety here comes only from
/// the caller always passing the acting actor's own centre id, never one taken from a route or body.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CentreReadService(AppDbContext db) : ICentreReadService
{
    public Task<CentreSettingsDto?> GetSettingsAsync(Guid centreId, CancellationToken ct) =>
        db.Set<Centre>()
            .AsNoTracking()
            .Where(centre => centre.Id == centreId)
            .Select(centre => new CentreSettingsDto(
                centre.Name, centre.Slug, centre.TimeZoneId, centre.DefaultLocale, EF.Property<uint>(centre, "Version")))
            .SingleOrDefaultAsync(ct);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
