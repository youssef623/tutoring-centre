# `tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory group repository. `Existing` simulates stored rows; `Added` records Add calls; `ActiveEnrollmentCounts` is the count `GetForEnrollmentAsync` reports (0 when absent). No lock: concurrency is proven against a real database, not here.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 50 lines and 2,357 bytes. Source hash: `92b64672a89704366e2a5c36bcb572d1c39bdb8f2dffb09259ee0a6fb513f190`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L11): `public sealed class FakeGroupRepository : IGroupRepository`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L19): `public Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 22](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L22): `public Task<Group?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L25): `public Task<GroupForEnrollment?> GetForEnrollmentAsync(Guid groupId, CancellationToken ct)`
- [Line 36](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L36): `public bool IsAtVersion(Group group, uint expectedVersion) =>`
- [Line 39](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L39): `public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L43): `public Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct) =>`
- [Line 46](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L46): `public Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct) =>`
- [Line 49](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs#L49): `public void Add(Group group) => Added.Add(group);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs:147](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L147)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs:16](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs:16](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs:98](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L98)
- [tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs:130](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L130)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:133](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L133)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:130](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L130)
- [tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs:105](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L105)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:143](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L143)
- [tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs:88](../../../../../tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs#L88)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:134](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L134)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:180](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L180)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:228](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L228)
- [tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs:135](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L135)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory group repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls;
/// <see cref="ActiveEnrollmentCounts"/> is the count <see cref="GetForEnrollmentAsync"/> reports (0 when absent).
/// No lock: concurrency is proven against a real database, not here.
/// </summary>
public sealed class FakeGroupRepository : IGroupRepository
{
    public List<Group> Existing { get; } = [];

    public List<Group> Added { get; } = [];

    public Dictionary<Guid, int> ActiveEnrollmentCounts { get; } = [];

    public Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(group => group.Id == id));

    public Task<Group?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(group => group.Id == id));

    public Task<GroupForEnrollment?> GetForEnrollmentAsync(Guid groupId, CancellationToken ct)
    {
        var group = Existing.FirstOrDefault(group => group.Id == groupId);
        return Task.FromResult(group is null
            ? null
            : new GroupForEnrollment(group, ActiveEnrollmentCounts.GetValueOrDefault(groupId)));
    }

    /// <summary>The stored version of each group; a group not listed is at version 0.</summary>
    public Dictionary<Guid, uint> Versions { get; } = [];

    public bool IsAtVersion(Group group, uint expectedVersion) =>
        Versions.GetValueOrDefault(group.Id) == expectedVersion;

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(group =>
            group.NormalizedName == normalizedName && group.Id != excludingId));

    public Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct) =>
        Task.FromResult(Existing.Any(group => group.SubjectId == subjectId && group.Status == GroupStatus.Active));

    public Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct) =>
        Task.FromResult(Existing.Any(group => group.TeacherId == teacherId && group.Status == GroupStatus.Active));

    public void Add(Group group) => Added.Add(group);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
