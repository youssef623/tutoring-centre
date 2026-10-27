# `src/TutoringCentre.Application/Academics/Students/IStudentRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for the Student aggregate. Every load includes the parent links — the invariant can only be checked on the whole aggregate, never on the root alone. Methods are named by use; nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and row-level security, so this interface cannot be called with the wrong tenant. A student of another centre is simply not found by either load.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentRepository.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 25 lines and 1,304 bytes. Source hash: `d6e52bcdc887045e3d56f0c2b3f43d170ebdd9e5964c77566998f1e2f8058fc1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentRepository.cs#L9): `/// port by the EF query filter and row-level security, so this interface cannot be called with the wrong`
- [Line 12](../../../../../../src/TutoringCentre.Application/Academics/Students/IStudentRepository.cs#L12): `public interface IStudentRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs#L10)
- [src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:17](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UnlinkParent/UnlinkParentHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs#L13)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:139](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L139)
- [src/TutoringCentre.Infrastructure/Repositories/StudentRepository.cs:16](../../../../../../src/TutoringCentre.Infrastructure/Repositories/StudentRepository.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:152](../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L152)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:164](../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L164)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs:7](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs#L7)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Students;

/// <summary>
/// Write-side persistence port for the Student aggregate. Every load includes the parent links — the
/// invariant can only be checked on the whole aggregate, never on the root alone. Methods are named by use;
/// nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this
/// port by the EF query filter and row-level security, so this interface cannot be called with the wrong
/// tenant. A student of another centre is simply not found by either load.
/// </summary>
public interface IStudentRepository
{
    /// <summary>
    /// Loads one student with its parent links for modification, expecting it to still be at
    /// <paramref name="expectedVersion"/>; the save, not the handler, detects a stale version.
    /// </summary>
    Task<Student?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one student with its parent links for modification with no version expectation — enrollment uses this on Day 48.</summary>
    Task<Student?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>Tracks a new student; the dispatcher saves it.</summary>
    void Add(Student student);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
