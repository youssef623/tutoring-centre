# `src/TutoringCentre.Application/Common/Security/ICurrentActor.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-only access to the actor of the current scope (HTTP request or job).

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Security/ICurrentActor.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 7 lines and 212 bytes. Source hash: `b02915dd56d902ddf9720cc8bc7bb6f9a176c9c3d5529b40dacf195b11b71423`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../src/TutoringCentre.Application/Common/Security/ICurrentActor.cs#L4): `public interface ICurrentActor`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:153](../../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L153)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:18](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L18)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:22](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L22)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:17](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs:19](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L19)
- [src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs:9](../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs#L9)
- [src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs:14](../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs#L14)
- [src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs:9](../../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs#L9)
- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:159](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L159)
- [src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs:7](../../../../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs#L7)
- [src/TutoringCentre.Application/DependencyInjection.cs:16](../../../../../../src/TutoringCentre.Application/DependencyInjection.cs#L16)
- [src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs:14](../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs#L14)
- [src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs:14](../../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs#L14)
- [src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs:15](../../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs#L15)
- [src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs:15](../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs#L15)
- 24 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Security;

/// <summary>Read-only access to the actor of the current scope (HTTP request or job).</summary>
public interface ICurrentActor
{
    Actor Actor { get; }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
