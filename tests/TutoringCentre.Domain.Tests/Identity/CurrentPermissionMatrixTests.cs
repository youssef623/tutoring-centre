using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Domain.Tests.Identity;
public sealed class CurrentPermissionMatrixTests
{
    // Literal acceptance matrix: Owner, Secretary, Teacher, Parent, Student. SystemActor is a separate dispatcher path.
    public static TheoryData<string, bool, bool, bool, bool, bool> Rows => new()
    {
        { "subjects.view", true, true, true, false, false }, { "subjects.manage", true, true, false, false, false },
        { "staff.view", true, false, false, false, false }, { "staff.manage", true, false, false, false, false },
        { "audit.view", true, false, false, false, false }, { "centre.settings.manage", true, false, false, false, false },
        { "students.view", true, true, false, false, false }, { "students.manage", true, true, false, false, false },
        { "teachers.view", true, true, false, false, false }, { "teachers.manage", true, false, false, false, false },
        { "groups.view", true, true, true, false, false }, { "groups.manage", true, true, false, false, false },
        { "enrollments.view", true, true, true, false, false }, { "enrollments.manage", true, true, false, false, false },
        { "schedule.view", true, true, true, false, false }, { "schedule.manage", true, true, false, false, false },
        { "attendance.view", true, true, true, true, true }, { "attendance.mark", true, true, true, false, false },
        { "attendance.correct", true, true, false, false, false }, { "attendance.cards.manage", true, true, false, false, false },
        { "conversations.view", true, true, false, false, false }, { "conversations.ingest", false, false, false, false, false },
        { "portal.accounts.manage", true, true, false, false, false }, { "portal.chat", true, true, false, true, true },
        { "assistant.view", true, true, false, false, false }, { "billing.view", true, true, false, false, false },
        { "billing.generate", false, false, false, false, false }
    };
    [Theory][MemberData(nameof(Rows))]
    public void All135CellsAreExplicit(string permission, bool owner, bool secretary, bool teacher, bool parent, bool student)
    {
        Assert.Equal(owner, RolePermissions.Holds(StaffRole.Owner, permission)); Assert.Equal(secretary, RolePermissions.Holds(StaffRole.Secretary, permission));
        Assert.Equal(teacher, RolePermissions.Holds(StaffRole.Teacher, permission)); Assert.Equal(parent, RolePermissions.Holds(StaffRole.Parent, permission)); Assert.Equal(student, RolePermissions.Holds(StaffRole.Student, permission));
    }
    [Fact] public void MatrixCoversEveryKeyOnce() => Assert.Equal(Permissions.All.Order(StringComparer.Ordinal), Rows.Select(row => (string)row[0]).Order(StringComparer.Ordinal));
}
