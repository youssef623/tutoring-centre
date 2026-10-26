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

        Assert.Equal("teacher.not_found", result.Error!.Code);
    }

    private async Task<Error?> SendAsync(CreatedStaff actor, string request, Guid groupId, Guid ownTeacherId)
    {
        // Version 0 is deliberately meaningless: the lookup must refuse before any version comparison happens.
        Result result = request switch
        {
            nameof(RenameGroupCommand) => await Fixture.SendAsAsync<RenameGroupCommand, Unit>(
                actor.Actor, new RenameGroupCommand(groupId, "Hijacked", 0)),
            nameof(ChangeGroupCapacityCommand) => await Fixture.SendAsAsync<ChangeGroupCapacityCommand, Unit>(
                actor.Actor, new ChangeGroupCapacityCommand(groupId, 99, 0)),
            nameof(AssignGroupTeacherCommand) => await Fixture.SendAsAsync<AssignGroupTeacherCommand, Unit>(
                actor.Actor, new AssignGroupTeacherCommand(groupId, ownTeacherId, 0)),
            nameof(ArchiveGroupCommand) => await Fixture.SendAsAsync<ArchiveGroupCommand, Unit>(
                actor.Actor, new ArchiveGroupCommand(groupId, 0)),
            nameof(RestoreGroupCommand) => await Fixture.SendAsAsync<RestoreGroupCommand, Unit>(
                actor.Actor, new RestoreGroupCommand(groupId, 0)),
            nameof(GetGroupQuery) => await Fixture.QueryAsAsync<GetGroupQuery, GroupDto>(
                actor.Actor, new GetGroupQuery(groupId)),
            _ => throw new ArgumentOutOfRangeException(nameof(request), request, null),
        };

        return result.Error;
    }

    private static string RowSnapshotSql(Guid groupId) =>
        $"select concat_ws('|', name, capacity, teacher_id, status, xmin) from academics.groups where id = '{groupId}'";

    private async Task<World> ArrangeAsync()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "owner-iso@nile.test", StaffRole.Owner);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "owner-iso@maadi.test", StaffRole.Owner);
        var nileSubjectId = await CreateSubjectAsync(nileOwner.Actor, "Mathematics");
        var maadiSubjectId = await CreateSubjectAsync(maadiOwner.Actor, "Mathematics");
        var nileTeacherId = await CreateTeacherAsync(nileOwner.Actor, "Amina Hassan");
        var maadiTeacherId = await CreateTeacherAsync(maadiOwner.Actor, "Mona Said");
        var nileGroupId = await CreateGroupAsync(nileOwner.Actor, "Grade 10 A", nileSubjectId, nileTeacherId);
        var maadiGroupId = await CreateGroupAsync(maadiOwner.Actor, "Grade 10 A", maadiSubjectId, maadiTeacherId);
        return new World(nileOwner, maadiOwner, nileSubjectId, maadiSubjectId, nileTeacherId, maadiTeacherId, nileGroupId, maadiGroupId);
    }

    private sealed record World(
        CreatedStaff NileOwner,
        CreatedStaff MaadiOwner,
        Guid NileSubjectId,
        Guid MaadiSubjectId,
        Guid NileTeacherId,
        Guid MaadiTeacherId,
        Guid NileGroupId,
        Guid MaadiGroupId);
}
