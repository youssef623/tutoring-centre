# `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 44.14: the six group commands through the real dispatcher, real adapters and real PostgreSQL, plus the two places the database, not the handler, must arbitrate: a name that differs only by hamza form, and ten parallel creates racing past the handler's own uniqueness check.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 220 lines and 11,883 bytes. Source hash: `d5490cc19258c82f734501f0f650358538f2d1dad63eea97a3a4f177d457f665`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups;`
- Line 2: `using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;`
- Line 4: `using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;`
- Line 5: `using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;`
- Line 6: `using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;`
- Line 7: `using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;`
- Line 8: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 9: `using TutoringCentre.Application.Academics.Teachers;`
- Line 10: `using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;`
- Line 11: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 12: `using TutoringCentre.Application.Common.Cqrs;`
- Line 13: `using TutoringCentre.Domain.Academics;`
- Line 14: `using TutoringCentre.Domain.Common;`
- Line 15: `using TutoringCentre.Domain.Identity;`
- Line 16: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L25): `public sealed class GroupUseCaseTests(PostgresFixture fixture) : GroupTestBase(fixture)`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L30): `public async Task Create_ThenGet_ReturnsTheGroupWithSubjectAndTeacherNames()`
- [Line 50](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L50): `public async Task Rename_PersistsTheNewName()`
- [Line 62](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L62): `public async Task Rename_WithAStaleVersion_ReturnsConcurrencyStale()`
- [Line 77](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L77): `public async Task ChangeCapacity_PersistsInRangeAndRefusesOutOfRange()`
- [Line 92](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L92): `public async Task AssignTeacher_MovesTheGroupAndAuditsTeacherIdBeforeAndAfter()`
- [Line 109](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L109): `public async Task ArchiveThenRestore_KeepsTheRowAndTogglesStatus()`
- [Line 126](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L126): `public async Task Restore_UnderAnArchivedSubject_ReturnsSubjectArchived()`
- [Line 144](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L144): `public async Task Create_NameDifferingOnlyByHamzaForm_ReturnsNameTaken()`
- [Line 158](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L158): `public async Task Create_TenParallelCreatesWithOneName_LeavesExactlyOneRow()`
- [Line 182](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L182): `public async Task TeacherRead_ActiveGroupCount_CountsOnlyActiveGroups()`
- [Line 196](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L196): `public async Task ArchiveSubjectAndTeacher_WithAnActiveGroup_AreRefused()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 44.14: the six group commands through the real dispatcher, real adapters and real PostgreSQL, plus the
/// two places the database, not the handler, must arbitrate: a name that differs only by hamza form, and ten
/// parallel creates racing past the handler's own uniqueness check.
/// </summary>
public sealed class GroupUseCaseTests(PostgresFixture fixture) : GroupTestBase(fixture)
{
    private const int Attempts = 10;

    [Fact]
    public async Task Create_ThenGet_ReturnsTheGroupWithSubjectAndTeacherNames()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-guc1@nile.test", StaffRole.Owner);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var teacherId = await CreateTeacherAsync(owner.Actor, "Amina Hassan");

        var groupId = await CreateGroupAsync(owner.Actor, "  Grade   10 A ", subjectId, teacherId, 25);

        var group = await GetGroupAsync(owner.Actor, groupId);
        Assert.Equal("Grade 10 A", group.Name);
        Assert.Equal(subjectId, group.SubjectId);
        Assert.Equal("Mathematics", group.SubjectName);
        Assert.Equal(teacherId, group.TeacherId);
        Assert.Equal("Amina Hassan", group.TeacherName);
        Assert.Equal(25, group.Capacity);
        Assert.Equal(0, group.EnrolledCount);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public async Task Rename_PersistsTheNewName()
    {
        var (owner, groupId, _, _) = await CreateOneGroupAsync("owner-guc2@nile.test");
        var version = (await GetGroupAsync(owner.Actor, groupId)).Version;

        var result = await Fixture.SendAsAsync<RenameGroupCommand, Unit>(owner.Actor, new RenameGroupCommand(groupId, "Grade 10 B", version));

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal("Grade 10 B", (await GetGroupAsync(owner.Actor, groupId)).Name);
    }

    [Fact]
    public async Task Rename_WithAStaleVersion_ReturnsConcurrencyStale()
    {
        var (owner, groupId, _, _) = await CreateOneGroupAsync("owner-guc3@nile.test");
        var staleVersion = (await GetGroupAsync(owner.Actor, groupId)).Version;
        var first = await Fixture.SendAsAsync<RenameGroupCommand, Unit>(owner.Actor, new RenameGroupCommand(groupId, "Grade 10 B", staleVersion));
        Assert.True(first.IsSuccess, first.Error?.Message);

        var result = await Fixture.SendAsAsync<RenameGroupCommand, Unit>(owner.Actor, new RenameGroupCommand(groupId, "Grade 10 C", staleVersion));

        Assert.True(result.IsFailure);
        Assert.Equal("concurrency.stale", result.Error!.Code);
        Assert.Equal("Grade 10 B", (await GetGroupAsync(owner.Actor, groupId)).Name);
    }

    [Fact]
    public async Task ChangeCapacity_PersistsInRangeAndRefusesOutOfRange()
    {
        var (owner, groupId, _, _) = await CreateOneGroupAsync("owner-guc4@nile.test");
        var version = (await GetGroupAsync(owner.Actor, groupId)).Version;

        var changed = await Fixture.SendAsAsync<ChangeGroupCapacityCommand, Unit>(owner.Actor, new ChangeGroupCapacityCommand(groupId, 200, version));
        var refused = await Fixture.SendAsAsync<ChangeGroupCapacityCommand, Unit>(
            owner.Actor, new ChangeGroupCapacityCommand(groupId, 201, (await GetGroupAsync(owner.Actor, groupId)).Version));

        Assert.True(changed.IsSuccess, changed.Error?.Message);
        Assert.Equal("group.capacity_invalid", refused.Error!.Code);
        Assert.Equal(200, (await GetGroupAsync(owner.Actor, groupId)).Capacity);
    }

```

The excerpt stops at line 90; use the original file for the remaining 130 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
