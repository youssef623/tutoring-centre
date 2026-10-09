using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 29.12: with no row-level security and no EF query filter on identity.memberships (the Day 29 contract's
/// documented exemption), these are the only proof that staff data does not leak across centres. Maadi always
/// uses Nile's real membership ID and real version, never an invented one.
/// </summary>
public sealed class StaffCrossTenantTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task ChangeRole_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-role@nile.test", StaffRole.Owner);
        var nileTeacher = await CreateStaffAsync(NileCentreId, "nile-teacher-role@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(nileOwner, nileTeacher.MembershipId);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-role@maadi.test", StaffRole.Owner);

        var result = await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(
            maadiOwner.Actor, new ChangeStaffRoleCommand(nileTeacher.MembershipId, StaffRole.Secretary, version));

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
        Assert.Equal(
            "teacher", await Fixture.ScalarAsync<string>($"select role from identity.memberships where id = '{nileTeacher.MembershipId}'"));
    }

    [Fact]
    public async Task Deactivate_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-deact@nile.test", StaffRole.Owner);
        var nileTeacher = await CreateStaffAsync(NileCentreId, "nile-teacher-deact@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(nileOwner, nileTeacher.MembershipId);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-deact@maadi.test", StaffRole.Owner);

        var result = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(
            maadiOwner.Actor, new DeactivateStaffCommand(nileTeacher.MembershipId, version));

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
        Assert.Equal(
            "active", await Fixture.ScalarAsync<string>($"select status from identity.memberships where id = '{nileTeacher.MembershipId}'"));
    }

    [Fact]
    public async Task Reactivate_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-react@nile.test", StaffRole.Owner);
        var nileTeacher = await CreateStaffAsync(NileCentreId, "nile-teacher-react@nile.test", StaffRole.Teacher);
        var versionBeforeDeactivate = await ReadVersionAsync(nileOwner, nileTeacher.MembershipId);
        var deactivated = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(
            nileOwner.Actor, new DeactivateStaffCommand(nileTeacher.MembershipId, versionBeforeDeactivate));
        Assert.True(deactivated.IsSuccess);
        var version = await ReadVersionAsync(nileOwner, nileTeacher.MembershipId);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-react@maadi.test", StaffRole.Owner);

        var result = await Fixture.SendAsAsync<ReactivateStaffCommand, Unit>(
            maadiOwner.Actor, new ReactivateStaffCommand(nileTeacher.MembershipId, version));

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
        Assert.Equal(
            "inactive", await Fixture.ScalarAsync<string>($"select status from identity.memberships where id = '{nileTeacher.MembershipId}'"));
    }

    [Fact]
    public async Task List_MaadiOwner_ContainsNoNileOnlyUser()
    {
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-list@maadi.test", StaffRole.Owner);
        await CreateStaffAsync(NileCentreId, "nile-only@nile.test", StaffRole.Teacher, "Nile Only Person");

        var result = await Fixture.QueryAsAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(maadiOwner.Actor, new ListStaffQuery());

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Value, member => member.Email == "nile-only@nile.test");
    }

    [Fact]
    public async Task List_TwoCentreTeacher_AppearsOnceInEachCentresListWithThatCentresRole()
    {
        const string sharedEmail = "two-centre-teacher@both.test";
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-shared@nile.test", StaffRole.Owner);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-shared@maadi.test", StaffRole.Owner);
        await CreateStaffAsync(NileCentreId, sharedEmail, StaffRole.Teacher, "Shared Teacher");
        await CreateStaffAsync(MaadiCentreId, sharedEmail, StaffRole.Secretary, "Shared Teacher");

        var nileList = await Fixture.QueryAsAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(nileOwner.Actor, new ListStaffQuery());
        var maadiList = await Fixture.QueryAsAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(maadiOwner.Actor, new ListStaffQuery());

        var nileEntry = Assert.Single(nileList.Value, member => member.Email == sharedEmail);
        var maadiEntry = Assert.Single(maadiList.Value, member => member.Email == sharedEmail);
        Assert.Equal(StaffRole.Teacher, nileEntry.Role);
        Assert.Equal(StaffRole.Secretary, maadiEntry.Role);
        Assert.Equal(nileEntry.UserId, maadiEntry.UserId);
    }
}
