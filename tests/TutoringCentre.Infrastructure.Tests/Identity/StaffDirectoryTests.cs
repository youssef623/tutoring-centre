using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 43.3: proves the adapter writes its own centre predicate — identity.memberships has no EF query filter
/// and no row-level security, so nothing underneath this port narrows it automatically — and answers true only
/// for an active member of that exact centre with that exact role.
/// </summary>
public sealed class StaffDirectoryTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task IsActiveMemberWithRoleAsync_TwoCentreTeacherAskedAboutTheirTeachingCentre_ReturnsTrue()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-sd1@nile.test", StaffRole.Owner);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-sd1@maadi.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "two-centre-sd1@both.test", StaffRole.Teacher);
        await CreateStaffForExistingUserAsync(maadiOwner, teacher.UserId, "two-centre-sd1@both.test", StaffRole.Secretary);

        var result = await ResolveAsync(directory => directory.IsActiveMemberWithRoleAsync(NileCentreId, teacher.UserId, StaffRole.Teacher, CancellationToken.None));

        Assert.True(result);
    }

    [Fact]
    public async Task IsActiveMemberWithRoleAsync_SecretaryAskedAboutTeacherRole_ReturnsFalse()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-sd2@nile.test", StaffRole.Secretary);

        var result = await ResolveAsync(directory => directory.IsActiveMemberWithRoleAsync(NileCentreId, secretary.UserId, StaffRole.Teacher, CancellationToken.None));

        Assert.False(result);
    }

    [Fact]
    public async Task IsActiveMemberWithRoleAsync_InactiveNileTeacher_ReturnsFalse()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-sd3@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "nile-teacher-sd3@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(nileOwner, teacher.MembershipId);
        var deactivated = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(nileOwner.Actor, new DeactivateStaffCommand(teacher.MembershipId, version));
        Assert.True(deactivated.IsSuccess);

        var result = await ResolveAsync(directory => directory.IsActiveMemberWithRoleAsync(NileCentreId, teacher.UserId, StaffRole.Teacher, CancellationToken.None));

        Assert.False(result);
    }

    [Fact]
    public async Task IsActiveMemberWithRoleAsync_MaadiOnlyTeacherAskedAboutNile_ReturnsFalse()
    {
        var teacher = await CreateStaffAsync(MaadiCentreId, "maadi-only-teacher-sd4@maadi.test", StaffRole.Teacher);

        var result = await ResolveAsync(directory => directory.IsActiveMemberWithRoleAsync(NileCentreId, teacher.UserId, StaffRole.Teacher, CancellationToken.None));

        Assert.False(result);
    }

    private async Task<bool> ResolveAsync(Func<IStaffDirectory, Task<bool>> call)
    {
        using var scope = Fixture.CreateScope();
        var directory = scope.ServiceProvider.GetRequiredService<IStaffDirectory>();
        return await call(directory);
    }

    /// <summary>Links a second centre's membership to a user who already has an account, exactly as CreateStaffCommand does when the email already exists.</summary>
    private async Task CreateStaffForExistingUserAsync(CreatedStaff owner, Guid existingUserId, string email, StaffRole role)
    {
        var result = await Fixture.SendAsAsync<CreateStaffCommand, CreateStaffResult>(
            owner.Actor, new CreateStaffCommand(email, email, role, "en"));
        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(existingUserId, result.Value.UserId);
    }
}
