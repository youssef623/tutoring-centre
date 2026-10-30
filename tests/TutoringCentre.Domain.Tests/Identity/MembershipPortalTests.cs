using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class MembershipPortalTests
{
    [Theory]
    [InlineData(PortalRole.Parent, "parent")]
    [InlineData(PortalRole.Student, "student")]
    public void PortalRoleIsFixedAndAccessCanBeRestored(PortalRole role, string key)
    {
        var membership = Membership.CreatePortal(Guid.NewGuid(), Guid.NewGuid(), role).Value;
        Assert.True(membership.IsPortal);
        Assert.Equal(key, membership.RoleKey);
        Assert.Equal(MembershipStatus.Active, membership.Status);
        Assert.Equal("membership.portal_role_fixed", membership.ChangeRole(StaffRole.Owner).Error!.Code);
        Assert.True(membership.Deactivate().IsSuccess);
        Assert.Equal("membership.portal_role_fixed", membership.ChangeRole(StaffRole.Teacher).Error!.Code);
        Assert.True(membership.Activate().IsSuccess);
    }

    [Fact]
    public void InvalidFactoryArgumentsAreRejected()
    {
        Assert.True(Membership.CreatePortal(Guid.Empty, Guid.NewGuid(), PortalRole.Parent).IsFailure);
        Assert.True(Membership.CreatePortal(Guid.NewGuid(), Guid.Empty, PortalRole.Student).IsFailure);
        Assert.True(Membership.CreatePortal(Guid.NewGuid(), Guid.NewGuid(), (PortalRole)99).IsFailure);
        Assert.True(Membership.Create(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Parent).IsFailure);
    }
}
