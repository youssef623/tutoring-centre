# `src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): EF Core implementation of the Enrollment write-side port. It tracks entities; the unit of work saves them. No centre predicate anywhere: the EF query filter and row-level security already scope every query to the acting actor's centre.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 49 lines and 2,300 bytes. Source hash: `d59e25941e25f933d1d17749d0b8694c6b5117540d82adbaaeb0ed5420aede58`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs#L15): `internal sealed class EnrollmentRepository(AppDbContext db) : IEnrollmentRepository`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs#L17): `public async Task<Enrollment?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 31](../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs#L31): `public Task<bool> ActiveExistsAsync(Guid studentId, Guid groupId, CancellationToken ct) =>`
- [Line 38](../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs#L38): `public Task<bool> AnyActiveForGroupAsync(Guid groupId, CancellationToken ct) =>`
- [Line 41](../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs#L41): `public Task<bool> AnyActiveForStudentAsync(Guid studentId, CancellationToken ct) =>`
- [Line 44](../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs#L44): `public void Add(Enrollment enrollment)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:142](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L142)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Enrollment write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class EnrollmentRepository(AppDbContext db) : IEnrollmentRepository
{
    public async Task<Enrollment?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var enrollment = await db.Set<Enrollment>().SingleOrDefaultAsync(enrollment => enrollment.Id == id, ct);
        if (enrollment is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(enrollment).Property("Version").OriginalValue = expectedVersion;
        return enrollment;
    }

    public Task<bool> ActiveExistsAsync(Guid studentId, Guid groupId, CancellationToken ct) =>
        db.Set<Enrollment>().AnyAsync(
            enrollment => enrollment.StudentId == studentId
                && enrollment.GroupId == groupId
                && enrollment.Status == EnrollmentStatus.Active,
            ct);

    public Task<bool> AnyActiveForGroupAsync(Guid groupId, CancellationToken ct) =>
        db.Set<Enrollment>().AnyAsync(enrollment => enrollment.GroupId == groupId && enrollment.Status == EnrollmentStatus.Active, ct);

    public Task<bool> AnyActiveForStudentAsync(Guid studentId, CancellationToken ct) =>
        db.Set<Enrollment>().AnyAsync(enrollment => enrollment.StudentId == studentId && enrollment.Status == EnrollmentStatus.Active, ct);

    public void Add(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        db.Set<Enrollment>().Add(enrollment);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
