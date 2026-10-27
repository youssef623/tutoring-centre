# `src/TutoringCentre.Application/Common/Cqrs/ITenantScoped.cs`

## Purpose

Source summary (intent, not independent execution proof): Marks a command or query that operates on one centre's data. Tenant-scoped requests need an actor with a centre; a request that runs before or outside a tenant (login, centre selection, platform-wide queries) does not implement this.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Cqrs/ITenantScoped.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 8 lines and 358 bytes. Source hash: `661c40fb4679b451ea6013a2cf3a24d44d1eed48092f072b5d3ba7e3c06afea9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../src/TutoringCentre.Application/Common/Cqrs/ITenantScoped.cs#L8): `public interface ITenantScoped;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsQuery.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsQuery.cs#L8)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherCommand.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherCommand.cs#L10)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsQuery.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsQuery.cs#L11)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneQuery.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneQuery.cs#L11)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs:12](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs#L12)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleCommand.cs#L8)
- 39 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>
/// Marks a command or query that operates on one centre's data. Tenant-scoped requests need an actor with
/// a centre; a request that runs before or outside a tenant (login, centre selection, platform-wide
/// queries) does not implement this.
/// </summary>
public interface ITenantScoped;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
