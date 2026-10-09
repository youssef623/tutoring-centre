using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class RolePermissionsTests
{
    // Day 28 contract matrix, written out literally. Never derive these from RolePermissions itself.
    [Theory]
    [InlineData(StaffRole.Owner, Permissions.SubjectsView, true)]
    [InlineData(StaffRole.Owner, Permissions.SubjectsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.StaffView, true)]
    [InlineData(StaffRole.Owner, Permissions.StaffManage, true)]
    [InlineData(StaffRole.Owner, Permissions.AuditView, true)]
    [InlineData(StaffRole.Owner, Permissions.CentreSettingsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.SubjectsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.SubjectsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.StaffView, false)]
    [InlineData(StaffRole.Secretary, Permissions.StaffManage, false)]
    [InlineData(StaffRole.Secretary, Permissions.AuditView, false)]
    [InlineData(StaffRole.Secretary, Permissions.CentreSettingsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.SubjectsView, true)]
    [InlineData(StaffRole.Teacher, Permissions.SubjectsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.StaffView, false)]
    [InlineData(StaffRole.Teacher, Permissions.StaffManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.AuditView, false)]
    [InlineData(StaffRole.Teacher, Permissions.CentreSettingsManage, false)]
    public void Holds_MatchesTheMatrixCellLiterally(StaffRole role, string permission, bool expected)
    {
        Assert.Equal(expected, RolePermissions.Holds(role, permission));
    }

    [Fact]
    public void Owner_HoldsEveryPermission()
    {
        Assert.All(Permissions.All, permission => Assert.True(RolePermissions.Holds(StaffRole.Owner, permission)));
    }

    [Theory]
    [InlineData(StaffRole.Owner)]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public void For_OnlyReturnsPermissionsFromTheCatalogue(StaffRole role)
    {
        Assert.All(RolePermissions.For(role), permission => Assert.Contains(permission, Permissions.All));
    }

    [Theory]
    [InlineData(StaffRole.Owner)]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public void Holds_WithAnUnknownPermission_IsDeniedForEveryRole(StaffRole role)
    {
        Assert.False(RolePermissions.Holds(role, "nonexistent.permission"));
    }
}
