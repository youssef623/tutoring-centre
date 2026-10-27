# `src/TutoringCentre.Domain/Identity/RolePermissions.cs`

## Purpose

The central matrix maps per-centre staff roles to capability strings for subjects, students, teachers, groups, enrollments, schedule, staff, audit, and settings. Dispatcher authorization and profile permission projection use this map. Unknown roles and keys deny access; frontend visibility is a presentation of this server policy, not its enforcement.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Identity/RolePermissions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 58 lines and 2,373 bytes. Source hash: `c6dd2cdb578cec9f664f08720eec5fb05d38732f82f34fc2c019f09a6df19222`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Domain/Identity/RolePermissions.cs#L4): `public static class RolePermissions`
- [Line 53](../../../../../src/TutoringCentre.Domain/Identity/RolePermissions.cs#L53): `public static IReadOnlySet<string> For(StaffRole role) =>`
- [Line 57](../../../../../src/TutoringCentre.Domain/Identity/RolePermissions.cs#L57): `public static bool Holds(StaffRole role, string permission) => For(role).Contains(permission);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:132](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L132)
- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:186](../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L186)
- [src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs:37](../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs#L37)
- [tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs:47](../../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L47)
- [tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs:59](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs#L59)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
            Permissions.ScheduleView,
            Permissions.ScheduleManage,
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
            Permissions.ScheduleView,
            Permissions.ScheduleManage,
        },
        [StaffRole.Teacher] = new HashSet<string>
        {
            Permissions.SubjectsView,
            Permissions.GroupsView,
            // Own groups only: the read services' teacher scope narrows it (Day 48); not a separate key.
            Permissions.EnrollmentsView,
            // Own groups only, by the same ownership scope; read-only, no schedule.manage.
            Permissions.ScheduleView,
        },
    };

    /// <summary>The permission keys the role holds. Empty for an unrecognised role.</summary>
    public static IReadOnlySet<string> For(StaffRole role) =>
        Map.TryGetValue(role, out var permissions) ? permissions : new HashSet<string>();

    /// <summary>Whether the role holds the given permission. Denies unknown roles and unknown permissions.</summary>
    public static bool Holds(StaffRole role, string permission) => For(role).Contains(permission);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
