# `src/TutoringCentre.Application/Academics/Teachers/ITeacherRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for the Teacher aggregate. Methods are named by use; nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and row-level security, so this interface cannot be called with the wrong tenant.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Teachers/ITeacherRepository.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,424 bytes. Source hash: `abb7cf0451927ca998db95c4abf1702c1479df8c5cf4d77b8c8bb1c0763aa864`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../src/TutoringCentre.Application/Academics/Teachers/ITeacherRepository.cs#L8): `/// filter and row-level security, so this interface cannot be called with the wrong tenant.`
- [Line 10](../../../../../../src/TutoringCentre.Application/Academics/Teachers/ITeacherRepository.cs#L10): `public interface ITeacherRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs:22](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs#L10)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/RestoreTeacher/RestoreTeacherHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs:19](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs#L13)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:131](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L131)
- [src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs:15](../../../../../../src/TutoringCentre.Infrastructure/Repositories/TeacherRepository.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:156](../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L156)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:229](../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L229)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs:7](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs#L7)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Teachers;

/// <summary>
/// Write-side persistence port for the Teacher aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query
/// filter and row-level security, so this interface cannot be called with the wrong tenant.
/// </summary>
public interface ITeacherRepository
{
    /// <summary>
    /// Loads one teacher for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such teacher is visible to the
    /// acting actor's centre.
    /// </summary>
    Task<Teacher?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one teacher without a version check. Returns null when it does not exist or is not visible to the acting centre.</summary>
    Task<Teacher?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>Whether a user is already linked to a teacher in the acting centre, optionally excluding one teacher (for re-linking the same teacher).</summary>
    Task<bool> UserLinkedAsync(Guid userId, Guid? excludingTeacherId, CancellationToken ct);

    /// <summary>Tracks a new teacher; the dispatcher saves it.</summary>
    void Add(Teacher teacher);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
