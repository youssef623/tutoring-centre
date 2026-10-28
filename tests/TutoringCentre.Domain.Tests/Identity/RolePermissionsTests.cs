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
    [InlineData(StaffRole.Owner, Permissions.StudentsView, true)]
    [InlineData(StaffRole.Owner, Permissions.StudentsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.TeachersView, true)]
    [InlineData(StaffRole.Owner, Permissions.TeachersManage, true)]
    [InlineData(StaffRole.Owner, Permissions.GroupsView, true)]
    [InlineData(StaffRole.Owner, Permissions.GroupsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.EnrollmentsView, true)]
    [InlineData(StaffRole.Owner, Permissions.EnrollmentsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.ScheduleView, true)]
    [InlineData(StaffRole.Owner, Permissions.ScheduleManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.SubjectsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.SubjectsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.StaffView, false)]
    [InlineData(StaffRole.Secretary, Permissions.StaffManage, false)]
    [InlineData(StaffRole.Secretary, Permissions.AuditView, false)]
    [InlineData(StaffRole.Secretary, Permissions.CentreSettingsManage, false)]
    [InlineData(StaffRole.Secretary, Permissions.StudentsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.StudentsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.TeachersView, true)]
    [InlineData(StaffRole.Secretary, Permissions.TeachersManage, false)]
    [InlineData(StaffRole.Secretary, Permissions.GroupsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.GroupsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.EnrollmentsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.EnrollmentsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.ScheduleView, true)]
    [InlineData(StaffRole.Secretary, Permissions.ScheduleManage, true)]
    [InlineData(StaffRole.Teacher, Permissions.SubjectsView, true)]
    [InlineData(StaffRole.Teacher, Permissions.SubjectsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.StaffView, false)]
    [InlineData(StaffRole.Teacher, Permissions.StaffManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.AuditView, false)]
    [InlineData(StaffRole.Teacher, Permissions.CentreSettingsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.StudentsView, false)]
    [InlineData(StaffRole.Teacher, Permissions.StudentsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.TeachersView, false)]
    [InlineData(StaffRole.Teacher, Permissions.TeachersManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.GroupsView, true)]
    [InlineData(StaffRole.Teacher, Permissions.GroupsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.EnrollmentsView, true)]
    [InlineData(StaffRole.Teacher, Permissions.EnrollmentsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.ScheduleView, true)]
    [InlineData(StaffRole.Teacher, Permissions.ScheduleManage, false)]
    [InlineData(StaffRole.Owner, Permissions.AttendanceView, true)]
    [InlineData(StaffRole.Owner, Permissions.AttendanceMark, true)]
    [InlineData(StaffRole.Owner, Permissions.AttendanceCorrect, true)]
    [InlineData(StaffRole.Secretary, Permissions.AttendanceView, true)]
    [InlineData(StaffRole.Secretary, Permissions.AttendanceMark, true)]
    [InlineData(StaffRole.Secretary, Permissions.AttendanceCorrect, true)]
    [InlineData(StaffRole.Teacher, Permissions.AttendanceView, true)]
    [InlineData(StaffRole.Teacher, Permissions.AttendanceMark, true)]
    [InlineData(StaffRole.Teacher, Permissions.AttendanceCorrect, false)]
    [InlineData(StaffRole.Owner, Permissions.AttendanceCardsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.AttendanceCardsManage, true)]
    [InlineData(StaffRole.Teacher, Permissions.AttendanceCardsManage, false)]
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
