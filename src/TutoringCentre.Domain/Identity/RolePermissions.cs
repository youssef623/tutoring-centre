namespace TutoringCentre.Domain.Identity;

/// <summary>Which permissions each staff role holds. The only implementation of the Day 28 matrix.</summary>
public static class RolePermissions
{
    private static readonly Dictionary<StaffRole, IReadOnlySet<string>> Map = new()
    {
        [StaffRole.Owner] = new HashSet<string>
        {
            Permissions.SubjectsView,
            Permissions.SubjectsManage,
            Permissions.StaffView,
            Permissions.StaffManage,
            Permissions.AuditView,
            Permissions.CentreSettingsManage,
            Permissions.StudentsView,
            Permissions.StudentsManage,
            Permissions.TeachersView,
            Permissions.TeachersManage,
            Permissions.GroupsView,
            Permissions.GroupsManage,
            Permissions.EnrollmentsView,
            Permissions.EnrollmentsManage,
        },
        [StaffRole.Secretary] = new HashSet<string>
        {
            Permissions.SubjectsView,
            Permissions.SubjectsManage,
            Permissions.StudentsView,
            Permissions.StudentsManage,
            Permissions.TeachersView,
            Permissions.GroupsView,
            Permissions.GroupsManage,
            Permissions.EnrollmentsView,
            Permissions.EnrollmentsManage,
        },
        [StaffRole.Teacher] = new HashSet<string>
        {
            Permissions.SubjectsView,
            Permissions.GroupsView,
            // Own groups only: the read services' teacher scope narrows it (Day 48); not a separate key.
            Permissions.EnrollmentsView,
        },
    };

    /// <summary>The permission keys the role holds. Empty for an unrecognised role.</summary>
    public static IReadOnlySet<string> For(StaffRole role) =>
        Map.TryGetValue(role, out var permissions) ? permissions : new HashSet<string>();

    /// <summary>Whether the role holds the given permission. Denies unknown roles and unknown permissions.</summary>
    public static bool Holds(StaffRole role, string permission) => For(role).Contains(permission);
}
