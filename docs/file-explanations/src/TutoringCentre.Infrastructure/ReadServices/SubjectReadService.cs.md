# `src/TutoringCentre.Infrastructure/ReadServices/SubjectReadService.cs`

## Purpose

Projects subject IDs, names, status, and xmin versions with no tracking. Entity filters run before projection; the list excludes archived subjects by default, sorts names, and caps results at five hundred. The cap is truncation rather than cursor pagination.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SubjectReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 39 lines and 1,836 bytes. Source hash: `6ab99a4147f15d4cd78917962c4a8d76c08145fbb6808e3c34137688fb402f8b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using System.Linq.Expressions;`
- Line 3: `using Microsoft.EntityFrameworkCore;`
- Line 4: `using TutoringCentre.Application.Academics.Subjects;`
- Line 5: `using TutoringCentre.Domain.Academics;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SubjectReadService.cs#L17): `internal sealed class SubjectReadService(AppDbContext db) : ISubjectReadService`
- [Line 24](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SubjectReadService.cs#L24): `public async Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct) =>`
- [Line 32](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SubjectReadService.cs#L32): `public Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct) =>`
- [Line 38](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SubjectReadService.cs#L38): `private IQueryable<Subject> Source() => db.Set<Subject>().AsNoTracking();`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:14](../../../../onboarding-work/extend_backend_purposes.py#L14)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:129](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L129)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Subject: flat, no-tracking projections straight to <see cref="SubjectDto"/>. No tenant
/// predicate written by hand — the EF query filter and row-level security (Week 1) scope these the same as any
/// other query. Filtering happens on the entity before projecting (projecting first and filtering the DTO
/// afterward does not reliably translate); ordering and the 500-row cap run in SQL, not in memory.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SubjectReadService(AppDbContext db) : ISubjectReadService
{
    private const int ListCap = 500;

    private static readonly Expression<Func<Subject, SubjectDto>> ToDto =
        subject => new SubjectDto(subject.Id, subject.Name, subject.Status, EF.Property<uint>(subject, "Version"));

    public async Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct) =>
        await Source()
            .Where(subject => includeArchived || subject.Status == SubjectStatus.Active)
            .OrderBy(subject => subject.Name)
            .Take(ListCap)
            .Select(ToDto)
            .ToListAsync(ct);

    public Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct) =>
        Source()
            .Where(subject => subject.Id == id)
            .Select(ToDto)
            .SingleOrDefaultAsync(ct);

    private IQueryable<Subject> Source() => db.Set<Subject>().AsNoTracking();
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
