using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Shared setup for Task 29.12's staff integration tests. The first owner of a freshly created centre is
/// bootstrapped as a SystemActor — the same actor kind the seed CLI uses — since CreateStaffCommand itself
/// requires staff.manage, which no one holds in a brand-new centre yet.
/// </summary>
public abstract class StaffProbeTestBase(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private protected async Task<CreatedStaff> CreateStaffAsync(Guid centreId, string email, StaffRole role, string? displayName = null)
    {
        var result = await Fixture.SendAsAsync<CreateStaffCommand, CreateStaffResult>(
            new SystemActor(centreId), new CreateStaffCommand(email, displayName ?? email, role, "en"));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return new CreatedStaff(result.Value.MembershipId, result.Value.UserId, centreId, role);
    }

    private protected async Task<uint> ReadVersionAsync(CreatedStaff actingOwner, Guid membershipId)
    {
        var list = await Fixture.QueryAsAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(actingOwner.Actor, new ListStaffQuery());
        Assert.True(list.IsSuccess, list.Error?.Message);
        return list.Value.Single(member => member.MembershipId == membershipId).Version;
    }

    /// <summary>A staff member created during test setup, carrying enough to both act as a StaffActor and be a target.</summary>
    private protected sealed record CreatedStaff(Guid MembershipId, Guid UserId, Guid CentreId, StaffRole Role)
    {
        public StaffActor Actor => new(UserId, CentreId, Role);
    }
}
