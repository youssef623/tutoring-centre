# `src/TutoringCentre.Application/Common/Cqrs/IQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): A read that never changes state, producing . Handled by exactly one query handler.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Cqrs/IQuery.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 4 lines and 225 bytes. Source hash: `3481313cdb3bdfb5da7fe8188c457198a69cb462269b4a66ebc56f77d1eb8960`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../src/TutoringCentre.Application/Common/Cqrs/IQuery.cs#L4): `public interface IQuery<TResponse>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsQuery.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsQuery.cs#L8)
- [src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsQuery.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsQuery.cs#L11)
- [src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneQuery.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneQuery.cs#L11)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableQuery.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableQuery.cs#L11)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesQuery.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesQuery.cs#L8)
- [src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsQuery.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsQuery.cs#L8)
- [src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherQuery.cs#L7)
- [src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersQuery.cs:7](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersQuery.cs#L7)
- [src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogQuery.cs:15](../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogQuery.cs#L15)
- [src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsQuery.cs:7](../../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsQuery.cs#L7)
- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:101](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L101)
- [src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs:7](../../../../../../src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs#L7)
- [src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs:6](../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs#L6)
- [src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsQuery.cs:6](../../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsQuery.cs#L6)
- 15 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>A read that never changes state, producing <typeparamref name="TResponse"/>. Handled by exactly one query handler.</summary>
public interface IQuery<TResponse>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
