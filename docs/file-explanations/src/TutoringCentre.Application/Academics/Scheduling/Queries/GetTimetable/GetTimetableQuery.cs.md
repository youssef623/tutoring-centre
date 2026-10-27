# `src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): One week of sessions from , optionally for one group or one teacher. No centre: tenancy comes from the actor; ownership from the actor's teacher scope, which the filters only narrow.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 637 bytes. Source hash: `694acd96934e2e9dd3852f47ba0f68ae68e35744b177a45ab10a6f8e6772dba6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableQuery.cs#L10): `public sealed record GetTimetableQuery(DateOnly WeekStart, Guid? GroupId, Guid? TeacherId)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:120](../../../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L120)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs:19](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:87](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L87)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs:23](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L23)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs:82](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L82)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;

/// <summary>
/// One week of sessions from <paramref name="WeekStart"/>, optionally for one group or one teacher. No centre:
/// tenancy comes from the actor; ownership from the actor's teacher scope, which the filters only narrow.
/// </summary>
public sealed record GetTimetableQuery(DateOnly WeekStart, Guid? GroupId, Guid? TeacherId)
    : IQuery<TimetableDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
