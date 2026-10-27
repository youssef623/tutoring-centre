# `src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs`

## Purpose

Tracks parent roots, sets expected xmin on mutation loads, supports exact PhoneNumber equality lookup, and excludes a chosen root during duplicate-phone checks. Add only enrolls a row in tracking; unique constraints and UnitOfWork determine the final write outcome.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 46 lines and 2,014 bytes. Source hash: `8d92e0bcc2213b734eb7dda83abf86880cea42f37d9d7b7d8b8fe7ecedac61e9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Parents;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs#L16): `internal sealed class ParentRepository(AppDbContext db) : IParentRepository`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs#L18): `public async Task<Parent?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 32](../../../../../src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs#L32): `public Task<Parent?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 35](../../../../../src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs#L35): `public Task<Parent?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>`
- [Line 38](../../../../../src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs#L38): `public Task<bool> PhoneExistsAsync(PhoneNumber phone, Guid? excludingId, CancellationToken ct) =>`
- [Line 41](../../../../../src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs#L41): `public void Add(Parent parent)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:19](../../../../onboarding-work/extend_backend_purposes.py#L19)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:136](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L136)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:37](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L37)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Parent write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ParentRepository(AppDbContext db) : IParentRepository
{
    public async Task<Parent?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var parent = await db.Set<Parent>().SingleOrDefaultAsync(parent => parent.Id == id, ct);
        if (parent is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(parent).Property("Version").OriginalValue = expectedVersion;
        return parent;
    }

    public Task<Parent?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Parent>().SingleOrDefaultAsync(parent => parent.Id == id, ct);

    public Task<Parent?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>
        db.Set<Parent>().SingleOrDefaultAsync(parent => parent.Phone == phone, ct);

    public Task<bool> PhoneExistsAsync(PhoneNumber phone, Guid? excludingId, CancellationToken ct) =>
        db.Set<Parent>().AnyAsync(parent => parent.Phone == phone && parent.Id != excludingId, ct);

    public void Add(Parent parent)
    {
        ArgumentNullException.ThrowIfNull(parent);
        db.Set<Parent>().Add(parent);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
