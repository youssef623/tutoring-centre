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
    public const string StudentsView = "students.view";
    public const string StudentsManage = "students.manage";
    public const string TeachersView = "teachers.view";
    public const string TeachersManage = "teachers.manage";
    public const string GroupsView = "groups.view";
    public const string GroupsManage = "groups.manage";
    public const string EnrollmentsView = "enrollments.view";
    public const string EnrollmentsManage = "enrollments.manage";
    public const string ScheduleView = "schedule.view";
    public const string ScheduleManage = "schedule.manage";

    public const string AttendanceView = "attendance.view";
    public const string AttendanceMark = "attendance.mark";
    public const string AttendanceCorrect = "attendance.correct";

    public static IReadOnlyCollection<string> All { get; } = new[]
    {
        SubjectsView,
        SubjectsManage,
        StaffView,
        StaffManage,
        AuditView,
        CentreSettingsManage,
        StudentsView,
        StudentsManage,
        TeachersView,
        TeachersManage,
        GroupsView,
        GroupsManage,
        EnrollmentsView,
        EnrollmentsManage,
        ScheduleView,
        ScheduleManage,
        AttendanceView,
        AttendanceMark,
        AttendanceCorrect,
    };
}
