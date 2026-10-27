# `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 44.14: tenancy, the first check, in both directions. Each centre's owner aims every group request at the other centre's group and gets group.not_found, the same answer as an unknown ID; the other centre's row is unchanged afterwards. Creating a group with the other centre's subject or teacher is resolved inside the actor's centre and answers subject.not_found or teacher.not_found, never a foreign-key 500.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 142 lines and 7,068 bytes. Source hash: `ef8692221ce214342e40a133e30f448839c0c1801ac35194232d72d6742b45e2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups;`
- Line 2: `using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;`
- Line 4: `using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;`
- Line 5: `using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;`
- Line 6: `using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;`
- Line 7: `using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;`
- Line 8: `using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;`
- Line 9: `using TutoringCentre.Application.Common.Cqrs;`
- Line 10: `using TutoringCentre.Domain.Common;`
- Line 11: `using TutoringCentre.Domain.Identity;`
- Line 12: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L22): `public sealed class GroupTenantIsolationTests(PostgresFixture fixture) : GroupTestBase(fixture)`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L42): `public async Task RequestAimedAtTheOtherCentresGroup_ReturnsGroupNotFoundAndChangesNothing(string request, bool nileActsOnMaadi)`
- [Line 59](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L59): `public async Task CreateNileGroup_WithAMaadiSubject_ReturnsSubjectNotFound()`
- [Line 71](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L71): `public async Task CreateNileGroup_WithAMaadiTeacher_ReturnsTeacherNotFound()`
- [Line 83](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L83): `public async Task AssignNileGroup_ToAMaadiTeacher_ReturnsTeacherNotFound()`
- [Line 94](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L94): `private async Task<Error?> SendAsync(CreatedStaff actor, string request, Guid groupId, Guid ownTeacherId)`
- [Line 117](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L117): `private static string RowSnapshotSql(Guid groupId) =>`
- [Line 120](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L120): `private async Task<World> ArrangeAsync()`
- [Line 133](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L133): `private sealed record World(`

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
using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 44.14: tenancy, the first check, in both directions. Each centre's owner aims every group request at
/// the other centre's group and gets group.not_found, the same answer as an unknown ID; the other centre's row
/// is unchanged afterwards. Creating a group with the other centre's subject or teacher is resolved inside the
/// actor's centre and answers subject.not_found or teacher.not_found, never a foreign-key 500.
/// </summary>
public sealed class GroupTenantIsolationTests(PostgresFixture fixture) : GroupTestBase(fixture)
{
    public static TheoryData<string, bool> RequestsByDirection()
    {
        var data = new TheoryData<string, bool>();
        foreach (var request in new[]
        {
            nameof(RenameGroupCommand), nameof(ChangeGroupCapacityCommand), nameof(AssignGroupTeacherCommand),
            nameof(ArchiveGroupCommand), nameof(RestoreGroupCommand), nameof(GetGroupQuery),
        })
        {
            data.Add(request, true);
            data.Add(request, false);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(RequestsByDirection))]
    public async Task RequestAimedAtTheOtherCentresGroup_ReturnsGroupNotFoundAndChangesNothing(string request, bool nileActsOnMaadi)
    {
        var world = await ArrangeAsync();
        var (actor, ownTeacherId, foreignGroupId) = nileActsOnMaadi
            ? (world.NileOwner, world.NileTeacherId, world.MaadiGroupId)
            : (world.MaadiOwner, world.MaadiTeacherId, world.NileGroupId);
        var before = await Fixture.ScalarAsync<string>(RowSnapshotSql(foreignGroupId));

        var error = await SendAsync(actor, request, foreignGroupId, ownTeacherId);

        Assert.NotNull(error);
        Assert.Equal("group.not_found", error.Code);
        Assert.Equal(ErrorKind.NotFound, error.Kind);
        Assert.Equal(before, await Fixture.ScalarAsync<string>(RowSnapshotSql(foreignGroupId)));
    }

    [Fact]
    public async Task CreateNileGroup_WithAMaadiSubject_ReturnsSubjectNotFound()
    {
        var world = await ArrangeAsync();

        var result = await Fixture.SendAsAsync<CreateGroupCommand, CreateGroupResult>(
            world.NileOwner.Actor, new CreateGroupCommand("Smuggled", world.MaadiSubjectId, world.NileTeacherId, 20));

        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task CreateNileGroup_WithAMaadiTeacher_ReturnsTeacherNotFound()
    {
        var world = await ArrangeAsync();

        var result = await Fixture.SendAsAsync<CreateGroupCommand, CreateGroupResult>(
            world.NileOwner.Actor, new CreateGroupCommand("Smuggled", world.NileSubjectId, world.MaadiTeacherId, 20));

        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task AssignNileGroup_ToAMaadiTeacher_ReturnsTeacherNotFound()
    {
        var world = await ArrangeAsync();
        var version = (await GetGroupAsync(world.NileOwner.Actor, world.NileGroupId)).Version;

        var result = await Fixture.SendAsAsync<AssignGroupTeacherCommand, Unit>(
            world.NileOwner.Actor, new AssignGroupTeacherCommand(world.NileGroupId, world.MaadiTeacherId, version));

```

The excerpt stops at line 90; use the original file for the remaining 52 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
