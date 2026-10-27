# `src/TutoringCentre.Domain/Identity/Permissions.cs`

## Purpose

Source summary (intent, not independent execution proof): The closed set of permission keys. A key names a capability, never a role.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Identity/Permissions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 1,552 bytes. Source hash: `834dbf668492ac97495a431118ca7958f30cdbba2367b95ef614b2be7cbf2ceb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Domain/Identity/Permissions.cs#L4): `public static class Permissions`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
    };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
