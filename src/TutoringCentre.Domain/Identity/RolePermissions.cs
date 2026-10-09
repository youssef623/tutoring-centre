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
        },
        [StaffRole.Secretary] = new HashSet<string>
        {
            Permissions.SubjectsView,
            Permissions.SubjectsManage,
        },
        [StaffRole.Teacher] = new HashSet<string>
        {
            Permissions.SubjectsView,
        },
    };

    /// <summary>The permission keys the role holds. Empty for an unrecognised role.</summary>
    public static IReadOnlySet<string> For(StaffRole role) =>
        Map.TryGetValue(role, out var permissions) ? permissions : new HashSet<string>();

    /// <summary>Whether the role holds the given permission. Denies unknown roles and unknown permissions.</summary>
    public static bool Holds(StaffRole role, string permission) => For(role).Contains(permission);
}
