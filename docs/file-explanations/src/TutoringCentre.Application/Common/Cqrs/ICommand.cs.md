# `src/TutoringCentre.Application/Common/Cqrs/ICommand.cs`

## Purpose

Source summary (intent, not independent execution proof): An intention to change state, producing on success (use `Unit` when there is nothing to return). Handled by exactly one command handler.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Cqrs/ICommand.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 7 lines and 308 bytes. Source hash: `13c9aedc103982b53ac4de24ce215152af7131ff470486d7ec26d9b78fbb0d46`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../src/TutoringCentre.Application/Common/Cqrs/ICommand.cs#L7): `public interface ICommand<TResponse>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherCommand.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherCommand.cs#L10)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs:12](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs#L12)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleCommand.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleCommand.cs#L8)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleCommand.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleCommand.cs#L10)
- [src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentCommand.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentCommand.cs#L9)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs#L14)
- [src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentCommand.cs#L7)
- [src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentCommand.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentCommand.cs#L7)
- 30 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>
/// An intention to change state, producing <typeparamref name="TResponse"/> on success
/// (use <see cref="Unit"/> when there is nothing to return). Handled by exactly one command handler.
/// </summary>
public interface ICommand<TResponse>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
