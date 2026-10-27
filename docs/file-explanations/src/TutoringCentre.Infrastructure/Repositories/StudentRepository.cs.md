# `src/TutoringCentre.Infrastructure/Repositories/StudentRepository.cs`

## Purpose

Always eagerly loads parent links for both mutation and ordinary tracked lookup, preserving aggregate collection invariants during unlink and primary-parent operations. Mutation lookup also sets the client xmin OriginalValue. Omitting Include would make valid links appear absent to the aggregate.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/StudentRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 40 lines and 1,847 bytes. Source hash: `33cf07ee063fe3d3404f5a879c0bfaff64ee03de44d8e67a80936db960988f34`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Students;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/Repositories/StudentRepository.cs#L16): `internal sealed class StudentRepository(AppDbContext db) : IStudentRepository`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/Repositories/StudentRepository.cs#L18): `public async Task<Student?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 32](../../../../../src/TutoringCentre.Infrastructure/Repositories/StudentRepository.cs#L32): `public Task<Student?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 35](../../../../../src/TutoringCentre.Infrastructure/Repositories/StudentRepository.cs#L35): `public void Add(Student student)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:21](../../../../onboarding-work/extend_backend_purposes.py#L21)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:139](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L139)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:109](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L109)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Student write-side port. Always includes the parent links: without them,
/// a handler calling RemoveParent or SetPrimaryParent would see an empty collection and the aggregate would
/// wrongly report student.parent_not_linked. No centre predicate anywhere: the EF query filter and row-level
/// security already scope every query to the acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class StudentRepository(AppDbContext db) : IStudentRepository
{
    public async Task<Student?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var student = await db.Set<Student>().Include(student => student.Parents).SingleOrDefaultAsync(student => student.Id == id, ct);
        if (student is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(student).Property("Version").OriginalValue = expectedVersion;
        return student;
    }

    public Task<Student?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Student>().Include(student => student.Parents).SingleOrDefaultAsync(student => student.Id == id, ct);

    public void Add(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        db.Set<Student>().Add(student);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
