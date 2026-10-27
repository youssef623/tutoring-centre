# `src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs`

## Purpose

Provides tracked subject lookup, normalized-name duplicate checks excluding a supplied entity, and deferred insertion. Mutation lookup places the client version in EF OriginalValue; a stale write is detected by the eventual database UPDATE rather than an early in-memory comparison.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 43 lines and 1,941 bytes. Source hash: `448685fa61b672705bbdcd2a034332c73c98f085730f2f4d3c7a8adc832d5f4b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Subjects;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs#L15): `internal sealed class SubjectRepository(AppDbContext db) : ISubjectRepository`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs#L17): `public async Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 32](../../../../../src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs#L32): `public Task<Subject?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 35](../../../../../src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs#L35): `public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>`
- [Line 38](../../../../../src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs#L38): `public void Add(Subject subject)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:22](../../../../onboarding-work/extend_backend_purposes.py#L22)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:128](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L128)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:51](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L51)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:37](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L37)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Subject write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security (Week 1, Days 20-21) already scope
/// every query to the acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SubjectRepository(AppDbContext db) : ISubjectRepository
{
    public async Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var subject = await db.Set<Subject>().SingleOrDefaultAsync(subject => subject.Id == id, ct);
        if (subject is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time (Task 23.2's shadow
        // "Version" property, mapped to xmin): a stale client is refused by zero rows affected, not by a
        // comparison here.
        db.Entry(subject).Property("Version").OriginalValue = expectedVersion;
        return subject;
    }

    public Task<Subject?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Subject>().SingleOrDefaultAsync(subject => subject.Id == id, ct);

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        db.Set<Subject>().AnyAsync(subject => subject.NormalizedName == normalizedName && subject.Id != excludingId, ct);

    public void Add(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);
        db.Set<Subject>().Add(subject);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
