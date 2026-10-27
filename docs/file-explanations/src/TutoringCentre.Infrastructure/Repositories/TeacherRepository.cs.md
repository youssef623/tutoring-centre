# `src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs`

## Purpose

Provides tracked teacher loads, expected xmin assignment, account-link uniqueness checks excluding the current teacher, and deferred insertion. Its account check is tenant-filtered through the entity query filter; eligibility of the identity account belongs to the application staff-directory port.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 1,841 bytes. Source hash: `0854641ad4feaae4fd0977cb84c3cb5abe2ae4df81bc7296afe841a066699c73`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs#L15): `internal sealed class TeacherRepository(AppDbContext db) : ITeacherRepository`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs#L17): `public async Task<Teacher?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 31](../../../../../src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs#L31): `public Task<Teacher?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 34](../../../../../src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs#L34): `public Task<bool> UserLinkedAsync(Guid userId, Guid? excludingTeacherId, CancellationToken ct) =>`
- [Line 37](../../../../../src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs#L37): `public void Add(Teacher teacher)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:23](../../../../onboarding-work/extend_backend_purposes.py#L23)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:131](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L131)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:54](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L54)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Teacher write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TeacherRepository(AppDbContext db) : ITeacherRepository
{
    public async Task<Teacher?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var teacher = await db.Set<Teacher>().SingleOrDefaultAsync(teacher => teacher.Id == id, ct);
        if (teacher is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(teacher).Property("Version").OriginalValue = expectedVersion;
        return teacher;
    }

    public Task<Teacher?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Teacher>().SingleOrDefaultAsync(teacher => teacher.Id == id, ct);

    public Task<bool> UserLinkedAsync(Guid userId, Guid? excludingTeacherId, CancellationToken ct) =>
        db.Set<Teacher>().AnyAsync(teacher => teacher.UserId == userId && teacher.Id != excludingTeacherId, ct);

    public void Add(Teacher teacher)
    {
        ArgumentNullException.ThrowIfNull(teacher);
        db.Set<Teacher>().Add(teacher);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
