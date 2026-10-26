using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Task 29.12: the five staff use cases exercised through the real dispatcher, the real Infrastructure adapters and real PostgreSQL.</summary>
public sealed class StaffUseCaseTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task Create_NewEmailAsSystemActor_CreatesAnAccountAndAnActiveMembershipWithATemporaryPassword()
    {
        var result = await Fixture.SendAsAsync<CreateStaffCommand, CreateStaffResult>(
            new SystemActor(NileCentreId), new CreateStaffCommand("new-teacher@nile.test", "New Teacher", StaffRole.Teacher, "en"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.TemporaryPassword);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"""
                select count(*) from identity.memberships
                where id = '{result.Value.MembershipId}' and user_id = '{result.Value.UserId}'
                  and centre_id = '{NileCentreId}' and role = 'teacher' and status = 'active'
                """));
    }

    [Fact]
    public async Task ChangeRole_ActiveTeacherToSecretary_Succeeds()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner1@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "teacher1@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(owner, teacher.MembershipId);

        var result = await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(
            owner.Actor, new ChangeStaffRoleCommand(teacher.MembershipId, StaffRole.Secretary, version));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            "secretary",
            await Fixture.ScalarAsync<string>($"select role from identity.memberships where id = '{teacher.MembershipId}'"));
    }

    [Fact]
    public async Task Deactivate_ActiveTeacher_Succeeds()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner2@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "teacher2@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(owner, teacher.MembershipId);

        var result = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(owner.Actor, new DeactivateStaffCommand(teacher.MembershipId, version));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            "inactive",
            await Fixture.ScalarAsync<string>($"select status from identity.memberships where id = '{teacher.MembershipId}'"));
    }

    [Fact]
    public async Task Reactivate_DeactivatedTeacher_Succeeds()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner3@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "teacher3@nile.test", StaffRole.Teacher);
        var versionBeforeDeactivate = await ReadVersionAsync(owner, teacher.MembershipId);
        var deactivated = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(
            owner.Actor, new DeactivateStaffCommand(teacher.MembershipId, versionBeforeDeactivate));
        Assert.True(deactivated.IsSuccess);
        var versionAfterDeactivate = await ReadVersionAsync(owner, teacher.MembershipId);

        var result = await Fixture.SendAsAsync<ReactivateStaffCommand, Unit>(
            owner.Actor, new ReactivateStaffCommand(teacher.MembershipId, versionAfterDeactivate));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            "active",
            await Fixture.ScalarAsync<string>($"select status from identity.memberships where id = '{teacher.MembershipId}'"));
    }

    [Fact]
    public async Task List_AsNileOwner_ReturnsEveryNileMemberOrderedByDisplayName()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner4@nile.test", StaffRole.Owner, "Zoe Owner");
        await CreateStaffAsync(NileCentreId, "teacher4@nile.test", StaffRole.Teacher, "Amir Teacher");

        var result = await Fixture.QueryAsAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(owner.Actor, new ListStaffQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(["Amir Teacher", "Zoe Owner"], result.Value.Select(member => member.DisplayName));
        Assert.True(result.Value.Single(member => member.UserId == owner.UserId).IsCurrentUser);
    }

    [Fact]
    public async Task ChangeRole_AgainWithTheSameStaleVersion_ReturnsConcurrencyStale()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner5@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "teacher5@nile.test", StaffRole.Teacher);
        var v1 = await ReadVersionAsync(owner, teacher.MembershipId);
        var firstChange = await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(
            owner.Actor, new ChangeStaffRoleCommand(teacher.MembershipId, StaffRole.Secretary, v1));
        Assert.True(firstChange.IsSuccess);

        // Still v1 — the version first read, not a freshly re-read one (the anomaly under test).
        var secondChange = await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(
            owner.Actor, new ChangeStaffRoleCommand(teacher.MembershipId, StaffRole.Teacher, v1));

        Assert.True(secondChange.IsFailure);
        Assert.Equal("concurrency.stale", secondChange.Error!.Code);
        Assert.Equal(
            "secretary",
            await Fixture.ScalarAsync<string>($"select role from identity.memberships where id = '{teacher.MembershipId}'"));
    }

    [Fact]
    public async Task EnsureAccountThenFail_NewEmail_RollsBackTheAccountLeavingNoUser()
    {
        const string email = "rollback-victim@nile.test";

        var result = await Fixture.SendAsAsync<EnsureAccountThenFailCommand, Unit>(
            new SystemActor(NileCentreId), new EnsureAccountThenFailCommand(email, "Rollback Victim", "en"));

        Assert.True(result.IsFailure);
        Assert.Equal("test.deliberate_failure", result.Error!.Code);
        Assert.Equal(0, await Fixture.ScalarAsync<long>($"select count(*) from identity.users where email = '{email}'"));
    }
}
