# `src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for the Enrollment aggregate. Methods are named by use; nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and row-level security. Deliberately no way to count active enrollments: a count read without the group's row lock would reopen the overbooking race, so the only count lives in `Groups.IGroupRepository.GetForEnrollmentAsync`.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentRepository.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,639 bytes. Source hash: `eef553b5a0caa344e1d3c6313214f83502c1edada4a40a7094ca093735f83f23`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentRepository.cs#L12): `public interface IEnrollmentRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:24](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L24)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:24](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L24)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs#L10)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:142](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L142)
- [src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs:15](../../../../../../src/TutoringCentre.Infrastructure/Repositories/EnrollmentRepository.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:153](../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L153)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs:7](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs#L7)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Enrollments;

/// <summary>
/// Write-side persistence port for the Enrollment aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter
/// and row-level security. Deliberately no way to count active enrollments: a count read without the group's row
/// lock would reopen the overbooking race, so the only count lives in
/// <see cref="Groups.IGroupRepository.GetForEnrollmentAsync"/>.
/// </summary>
public interface IEnrollmentRepository
{
    /// <summary>
    /// Loads one enrollment for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such enrollment is visible to the
    /// acting actor's centre.
    /// </summary>
    Task<Enrollment?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Whether this student already holds an active enrollment in this group.</summary>
    Task<bool> ActiveExistsAsync(Guid studentId, Guid groupId, CancellationToken ct);

    /// <summary>Whether this group has any active enrollment.</summary>
    Task<bool> AnyActiveForGroupAsync(Guid groupId, CancellationToken ct);

    /// <summary>Whether this student has any active enrollment.</summary>
    Task<bool> AnyActiveForStudentAsync(Guid studentId, CancellationToken ct);

    /// <summary>Tracks a new enrollment; the dispatcher saves it.</summary>
    void Add(Enrollment enrollment);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
