# `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 44.14: ownership, the third check, through the real provider, read service and PostgreSQL. In Nile, G1 belongs to teacher T1 and G2 to teacher T2; three actors look at them: the owner, a Teacher-role actor linked to T1, and a Teacher-role actor with no linked record. The scope is proven on the list, on the list's teacher filter and on the item read, never on the list alone.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 145 lines and 6,099 bytes. Source hash: `f4898a86b25713af3f0215c78fec5cd7f91c0109eb5bda5eb0ce5129cfd2c1d3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups;`
- Line 2: `using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;`
- Line 4: `using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;`
- Line 5: `using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;`
- Line 6: `using TutoringCentre.Application.Common.Cqrs;`
- Line 7: `using TutoringCentre.Application.Common.Security;`
- Line 8: `using TutoringCentre.Domain.Identity;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L19): `public sealed class GroupTeacherScopeTests(PostgresFixture fixture) : GroupTestBase(fixture)`
- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L22): `public async Task Owner_ListsBothGroups()`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L32): `public async Task TeacherLinkedToT1_ListsOnlyG1()`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L42): `public async Task TeacherLinkedToT1_FilteringByT2_GetsAnEmptyListNotAnError()`
- [Line 52](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L52): `public async Task TeacherLinkedToT1_GetG2_ReturnsGroupNotFound()`
- [Line 65](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L65): `public async Task UnlinkedTeacher_ListsNothingAndGetsNothing()`
- [Line 77](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L77): `public async Task AfterReassigningG2ToT1_TheT1ActorListsBoth()`
- [Line 91](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L91): `public async Task AfterT1IsArchived_TheT1ActorListsNothing()`
- [Line 107](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L107): `public async Task TeacherLinkedToT1_IncludingArchived_StillSeesOnlyOwnGroups()`
- [Line 116](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L116): `private async Task<IReadOnlyList<GroupDto>> ListAsync(Actor actor, Guid? teacherId = null, bool includeArchived = false)`
- [Line 124](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L124): `private async Task<World> ArrangeAsync(string tag)`
- [Line 137](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L137): `private sealed record World(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 44.14: ownership, the third check, through the real provider, read service and PostgreSQL. In Nile,
/// G1 belongs to teacher T1 and G2 to teacher T2; three actors look at them: the owner, a Teacher-role actor
/// linked to T1, and a Teacher-role actor with no linked record. The scope is proven on the list, on the
/// list's teacher filter and on the item read, never on the list alone.
/// </summary>
public sealed class GroupTeacherScopeTests(PostgresFixture fixture) : GroupTestBase(fixture)
{
    [Fact]
    public async Task Owner_ListsBothGroups()
    {
        var world = await ArrangeAsync("scope1");

        var groups = await ListAsync(world.Owner.Actor);

        Assert.Equal([world.G1, world.G2], groups.Select(group => group.Id).Order().ToArray());
    }

    [Fact]
    public async Task TeacherLinkedToT1_ListsOnlyG1()
    {
        var world = await ArrangeAsync("scope2");

        var groups = await ListAsync(world.T1Actor.Actor);

        Assert.Equal(world.G1, Assert.Single(groups).Id);
    }

    [Fact]
    public async Task TeacherLinkedToT1_FilteringByT2_GetsAnEmptyListNotAnError()
    {
        var world = await ArrangeAsync("scope3");

        var groups = await ListAsync(world.T1Actor.Actor, teacherId: world.T2);

        Assert.Empty(groups);
    }

    [Fact]
    public async Task TeacherLinkedToT1_GetG2_ReturnsGroupNotFound()
    {
        var world = await ArrangeAsync("scope4");

        var result = await Fixture.QueryAsAsync<GetGroupQuery, GroupDto>(world.T1Actor.Actor, new GetGroupQuery(world.G2));
        var own = await Fixture.QueryAsAsync<GetGroupQuery, GroupDto>(world.T1Actor.Actor, new GetGroupQuery(world.G1));

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.True(own.IsSuccess, own.Error?.Message);
    }

    [Fact]
    public async Task UnlinkedTeacher_ListsNothingAndGetsNothing()
    {
        var world = await ArrangeAsync("scope5");

        var groups = await ListAsync(world.UnlinkedActor.Actor);
        var item = await Fixture.QueryAsAsync<GetGroupQuery, GroupDto>(world.UnlinkedActor.Actor, new GetGroupQuery(world.G1));

        Assert.Empty(groups);
        Assert.Equal("group.not_found", item.Error!.Code);
    }

    [Fact]
    public async Task AfterReassigningG2ToT1_TheT1ActorListsBoth()
    {
        var world = await ArrangeAsync("scope6");
        var version = (await GetGroupAsync(world.Owner.Actor, world.G2)).Version;

        var reassigned = await Fixture.SendAsAsync<AssignGroupTeacherCommand, Unit>(
            world.Owner.Actor, new AssignGroupTeacherCommand(world.G2, world.T1, version));

        Assert.True(reassigned.IsSuccess, reassigned.Error?.Message);
        var groups = await ListAsync(world.T1Actor.Actor);
        Assert.Equal([world.G1, world.G2], groups.Select(group => group.Id).Order().ToArray());
    }

    [Fact]
```

The excerpt stops at line 90; use the original file for the remaining 55 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
