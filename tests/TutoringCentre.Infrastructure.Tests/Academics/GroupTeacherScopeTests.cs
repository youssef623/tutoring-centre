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
    public async Task AfterT1IsArchived_TheT1ActorListsNothing()
    {
        var world = await ArrangeAsync("scope7");
        var moved = await Fixture.SendAsAsync<AssignGroupTeacherCommand, Unit>(
            world.Owner.Actor, new AssignGroupTeacherCommand(world.G1, world.T2, (await GetGroupAsync(world.Owner.Actor, world.G1)).Version));
        Assert.True(moved.IsSuccess, moved.Error?.Message);

        var archived = await Fixture.SendAsAsync<ArchiveTeacherCommand, Unit>(
            world.Owner.Actor, new ArchiveTeacherCommand(world.T1, await TeacherVersionAsync(world.Owner.Actor, world.T1)));

        Assert.True(archived.IsSuccess, archived.Error?.Message);
        Assert.Empty(await ListAsync(world.T1Actor.Actor));
        Assert.Empty(await ListAsync(world.T1Actor.Actor, includeArchived: true));
    }

    [Fact]
    public async Task TeacherLinkedToT1_IncludingArchived_StillSeesOnlyOwnGroups()
    {
        var world = await ArrangeAsync("scope8");

        var groups = await ListAsync(world.T1Actor.Actor, includeArchived: true);

        Assert.Equal(world.G1, Assert.Single(groups).Id);
    }

    private async Task<IReadOnlyList<GroupDto>> ListAsync(Actor actor, Guid? teacherId = null, bool includeArchived = false)
    {
        var result = await Fixture.QueryAsAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(
            actor, new ListGroupsQuery(null, teacherId, includeArchived));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private async Task<World> ArrangeAsync(string tag)
    {
        var owner = await CreateStaffAsync(NileCentreId, $"owner-{tag}@nile.test", StaffRole.Owner);
        var t1Member = await CreateStaffAsync(NileCentreId, $"teacher1-{tag}@nile.test", StaffRole.Teacher);
        var unlinked = await CreateStaffAsync(NileCentreId, $"teacher-unlinked-{tag}@nile.test", StaffRole.Teacher);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var t1 = await CreateTeacherAsync(owner.Actor, "Amina Hassan", t1Member.UserId);
        var t2 = await CreateTeacherAsync(owner.Actor, "Youssef Kamal");
        var g1 = await CreateGroupAsync(owner.Actor, "Grade 10 A", subjectId, t1);
        var g2 = await CreateGroupAsync(owner.Actor, "Grade 10 B", subjectId, t2);
        return new World(owner, t1Member, unlinked, t1, t2, g1, g2);
    }

    private sealed record World(
        CreatedStaff Owner,
        CreatedStaff T1Actor,
        CreatedStaff UnlinkedActor,
        Guid T1,
        Guid T2,
        Guid G1,
        Guid G2);
}
