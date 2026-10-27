# `src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Executes one query. Runs inside a read-only transaction and never saves (dispatcher, Day 7).

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 390 bytes. Source hash: `fe0cb41249807b8c10f2db27802731ea6d94dc3ea4b38b38d8c345a17472ee16`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs#L6): `public interface IQueryHandler<in TQuery, TResponse>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs:19](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/GetStudent/GetStudentHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs:8](../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsHandler.cs#L8)
- [src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/ListSubjects/ListSubjectsHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/ListTeachers/ListTeachersHandler.cs#L9)
- [src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs:9](../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogHandler.cs#L9)
- [src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs:10](../../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs#L10)
- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:130](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L130)
- [src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs:35](../../../../../../src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs#L35)
- [src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs:15](../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs#L15)
- [src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs:15](../../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs#L15)
- 27 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>Executes one query. Runs inside a read-only transaction and never saves (dispatcher, Day 7).</summary>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
