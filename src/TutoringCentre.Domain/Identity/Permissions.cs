namespace TutoringCentre.Domain.Identity;

/// <summary>The closed set of permission keys. A key names a capability, never a role.</summary>
public static class Permissions
{
    public const string SubjectsView = "subjects.view";
    public const string SubjectsManage = "subjects.manage";
    public const string StaffView = "staff.view";
    public const string StaffManage = "staff.manage";
    public const string AuditView = "audit.view";
    public const string CentreSettingsManage = "centre.settings.manage";

    public static IReadOnlyCollection<string> All { get; } = new[]
    {
        SubjectsView,
        SubjectsManage,
        StaffView,
        StaffManage,
        AuditView,
        CentreSettingsManage,
    };
}
