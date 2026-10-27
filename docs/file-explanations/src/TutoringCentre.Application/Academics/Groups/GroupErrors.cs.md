# `src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs`

## Purpose

Source summary (intent, not independent execution proof): Errors the group handlers raise themselves; the entity raises its own.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 35 lines and 1,941 bytes. Source hash: `dbda862e12adb628c7774c808f90c4058b7568f69569f97a30455e63a2f3be0f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L6): `internal static class GroupErrors`
- [Line 9](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L9): `public static Error NotFound() => Error.NotFound("group.not_found", "This group could not be found.");`
- [Line 12](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L12): `public static Error NameTaken() => new(`
- [Line 19](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L19): `public static Error Stale() =>`
- [Line 20](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L20): `Error.Conflict("concurrency.stale", "This record was changed by someone else. Reload and try again.");`
- [Line 22](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L22): `public static Error HasActiveEnrollments() =>`
- [Line 25](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L25): `public static Error HasActiveSchedule() =>`
- [Line 28](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L28): `public static Error SubjectArchived() => Error.Rule("group.subject_archived", "This subject is archived.");`
- [Line 30](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L30): `public static Error TeacherArchived() => Error.Rule("group.teacher_archived", "This teacher is archived.");`
- [Line 32](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L32): `public static Error SubjectNotFound() => Error.NotFound("subject.not_found", "This subject could not be found.");`
- [Line 34](../../../../../../src/TutoringCentre.Application/Academics/Groups/GroupErrors.cs#L34): `public static Error TeacherNotFound() => Error.NotFound("teacher.not_found", "This teacher could not be found.");`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:35](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L35)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:52](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L52)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs:29](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L29)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs:31](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L31)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:32](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L32)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs:19](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:38](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L38)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs:22](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups;

/// <summary>Errors the group handlers raise themselves; the entity raises its own.</summary>
internal static class GroupErrors
{
    /// <summary>The one answer for an unknown ID, another centre's group and a group outside the actor's teacher scope.</summary>
    public static Error NotFound() => Error.NotFound("group.not_found", "This group could not be found.");

    /// <summary>Matches the Conflict the unique-constraint catalogue reports for ux_groups_centre_normalized_name, so a race loser gets the same answer.</summary>
    public static Error NameTaken() => new(
        "group.name_taken",
        "A group with this name already exists.",
        ErrorKind.Conflict,
        new Dictionary<string, string[]> { ["name"] = ["A group with this name already exists."] });

    /// <summary>Matches the Conflict the save reports for a stale row version, for the locked loads that compare it in the handler.</summary>
    public static Error Stale() =>
        Error.Conflict("concurrency.stale", "This record was changed by someone else. Reload and try again.");

    public static Error HasActiveEnrollments() =>
        Error.Rule("group.has_active_enrollments", "This group still has enrolled students.");

    public static Error HasActiveSchedule() =>
        Error.Rule("group.has_active_schedule", "This group still has a schedule rule in effect. End it first.");

    public static Error SubjectArchived() => Error.Rule("group.subject_archived", "This subject is archived.");

    public static Error TeacherArchived() => Error.Rule("group.teacher_archived", "This teacher is archived.");

    public static Error SubjectNotFound() => Error.NotFound("subject.not_found", "This subject could not be found.");

    public static Error TeacherNotFound() => Error.NotFound("teacher.not_found", "This teacher could not be found.");
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
