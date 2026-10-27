# `src/TutoringCentre.Application/Academics/Subjects/ISubjectRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for the Subject aggregate. Methods are named by use; nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and row-level security (layers 2 and 3), so this interface cannot be called with the wrong tenant.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Subjects/ISubjectRepository.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,473 bytes. Source hash: `654192145567219af8f9e248fec6657f5ee940eb72aacab8f7b53c258bc3d02d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../src/TutoringCentre.Application/Academics/Subjects/ISubjectRepository.cs#L8): `/// filter and row-level security (layers 2 and 3), so this interface cannot be called with the wrong tenant.`
- [Line 10](../../../../../../src/TutoringCentre.Application/Academics/Subjects/ISubjectRepository.cs#L10): `public interface ISubjectRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectHandler.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectHandler.cs#L10)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectHandler.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RestoreSubject/RestoreSubjectHandler.cs#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:128](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L128)
- [src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs:15](../../../../../../src/TutoringCentre.Infrastructure/Repositories/SubjectRepository.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:155](../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L155)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:181](../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L181)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:113](../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L113)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs:7](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs#L7)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Subjects;

/// <summary>
/// Write-side persistence port for the Subject aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query
/// filter and row-level security (layers 2 and 3), so this interface cannot be called with the wrong tenant.
/// </summary>
public interface ISubjectRepository
{
    /// <summary>
    /// Loads one subject for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such subject is visible to the
    /// acting actor's centre.
    /// </summary>
    Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one subject without a version check, for commands that only reference it. Returns null when it does not exist or is not visible to the acting centre.</summary>
    Task<Subject?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>Whether a subject with this normalized name already exists in the acting centre, optionally excluding one subject (for rename).</summary>
    Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct);

    /// <summary>Tracks a new subject; the dispatcher saves it.</summary>
    void Add(Subject subject);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
