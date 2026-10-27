# `src/TutoringCentre.Application/Academics/Groups/IGroupRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for the Group aggregate. Methods are named by use; nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and row-level security, so this interface cannot be called with the wrong tenant.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Groups/IGroupRepository.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 52 lines and 3,014 bytes. Source hash: `8fe2dfeb07d60bed73420a27ce99d4721d8d2cb4cad093f903703bbe4e1be8f6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../src/TutoringCentre.Application/Academics/Groups/IGroupRepository.cs#L8): `/// filter and row-level security, so this interface cannot be called with the wrong tenant.`
- [Line 10](../../../../../../src/TutoringCentre.Application/Academics/Groups/IGroupRepository.cs#L10): `public interface IGroupRepository`
- [Line 52](../../../../../../src/TutoringCentre.Application/Academics/Groups/IGroupRepository.cs#L52): `public sealed record GroupForEnrollment(Group Group, int ActiveEnrollmentCount);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:22](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:19](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs:12](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs#L12)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:25](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L25)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:22](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectHandler.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectHandler.cs#L10)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs#L10)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:133](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L133)
- [src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs:15](../../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L15)
- [src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs:34](../../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L34)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:151](../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L151)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:157](../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L157)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:145](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L145)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:180](../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L180)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:228](../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L228)
- 2 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups;

/// <summary>
/// Write-side persistence port for the Group aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query
/// filter and row-level security, so this interface cannot be called with the wrong tenant.
/// </summary>
public interface IGroupRepository
{
    /// <summary>
    /// Loads one group for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such group is visible to the acting
    /// actor's centre.
    /// </summary>
    Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one group without a version check. Returns null when it does not exist or is not visible to the acting centre.</summary>
    Task<Group?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// The only load for a decision that depends on the group's active enrollment count (ADR 0011): locks the
    /// group's row, then counts its active enrollments, both inside the dispatcher's open transaction. The lock
    /// is held until that transaction ends, so the count stays true for the rest of the command. No version
    /// parameter: the lock, not the row version, protects this path. Meaningless outside a dispatched command,
    /// where no transaction holds the lock. Returns null when no such group is visible to the acting centre.
    /// </summary>
    Task<GroupForEnrollment?> GetForEnrollmentAsync(Guid groupId, CancellationToken ct);

    /// <summary>
    /// Whether a group loaded by this repository is still at <paramref name="expectedVersion"/>. For a group
    /// locked by <see cref="GetForEnrollmentAsync"/>, where the row cannot change until the transaction ends, so
    /// the handler can refuse a stale client before any rule runs.
    /// </summary>
    bool IsAtVersion(Group group, uint expectedVersion);

    /// <summary>Whether a group with this normalized name already exists in the acting centre, optionally excluding one group (for rename).</summary>
    Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct);

    /// <summary>Whether the acting centre has an active group of this subject.</summary>
    Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct);

    /// <summary>Whether the acting centre has an active group taught by this teacher.</summary>
    Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct);

    /// <summary>Tracks a new group; the dispatcher saves it.</summary>
    void Add(Group group);
}

/// <summary>A tracked, row-locked group and the count of its active enrollments, read after the lock was taken.</summary>
public sealed record GroupForEnrollment(Group Group, int ActiveEnrollmentCount);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
