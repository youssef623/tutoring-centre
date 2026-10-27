# `src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs`

## Purpose

Tracks platform centres, checks slug uniqueness, and sets the caller version as EF OriginalValue for subsequent optimistic UPDATE checks. It contains no save call: dispatcher-owned UnitOfWork controls persistence and audit. Centre scoping remains the handler responsibility because this platform entity has no tenant filter.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,318 bytes. Source hash: `53d6e1d4f0acbc0d2c30d6f45acf0f91ae0aeecf728a576a7bb17ce337ab89bd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Centres;`
- Line 3: `using TutoringCentre.Domain.Centres;`
- Line 4: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs#L9): `internal sealed class CentreRepository(AppDbContext db) : ICentreRepository`
- [Line 11](../../../../../src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs#L11): `public Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct) =>`
- [Line 14](../../../../../src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs#L14): `public async Task<Centre?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 28](../../../../../src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs#L28): `public void Add(Centre centre)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:17](../../../../onboarding-work/extend_backend_purposes.py#L17)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:124](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L124)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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

    public async Task<Centre?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var centre = await db.Set<Centre>().SingleOrDefaultAsync(centre => centre.Id == id, ct);
        if (centre is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time (mapped to xmin):
        // a stale client is refused by zero rows affected, not by a comparison here (same as SubjectRepository).
        db.Entry(centre).Property("Version").OriginalValue = expectedVersion;
        return centre;
    }

    public void Add(Centre centre)
    {
        ArgumentNullException.ThrowIfNull(centre);
        db.Set<Centre>().Add(centre);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
