# `src/TutoringCentre.Application/Common/Security/ITeacherScopeProvider.cs`

## Purpose

Source summary (intent, not independent execution proof): Resolves the acting actor's `TeacherScope` for a group-returning request. The scope derives only from the server-side actor and the database, never from the request. Registered scoped: the result is cached for the lifetime of one request.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Security/ITeacherScopeProvider.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 445 bytes. Source hash: `8fce0aa778213cb139afb7bf9b7a6758d404e9e5c433bc6bf832c4f5a7c0c638`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../src/TutoringCentre.Application/Common/Security/ITeacherScopeProvider.cs#L8): `public interface ITeacherScopeProvider`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs:10](../../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs#L10)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs:16](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs#L14)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:152](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L152)
- [src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs:18](../../../../../../src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:155](../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L155)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:159](../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L159)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:149](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L149)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherScopeProvider.cs:6](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherScopeProvider.cs#L6)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs:124](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L124)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Resolves the acting actor's <see cref="TeacherScope"/> for a group-returning request. The scope derives
/// only from the server-side actor and the database, never from the request. Registered scoped: the result is
/// cached for the lifetime of one request.
/// </summary>
public interface ITeacherScopeProvider
{
    Task<TeacherScope> GetAsync(CancellationToken ct);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
