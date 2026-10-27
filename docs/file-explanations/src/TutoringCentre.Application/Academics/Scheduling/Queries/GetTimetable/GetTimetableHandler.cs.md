# `src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): The week is selected by local date, and the instants go out as UTC with the centre's zone ID beside them: the client formats them in the centre's zone, never the browser's, and the server never turns them into local strings.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,436 bytes. Source hash: `214f702856f77b2bd3d926a2d34bd7156f62aa20c74a566dbec7af1aa8ad049d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Centres.Contracts;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs#L15): `internal sealed class GetTimetableHandler(`
- [Line 21](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs#L21): `public async Task<Result<TimetableDto>> HandleAsync(GetTimetableQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:154](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L154)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs:21](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;

/// <summary>
/// The week is selected by local date, and the instants go out as UTC with the centre's zone ID beside them: the
/// client formats them in the centre's zone, never the browser's, and the server never turns them into local
/// strings.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetTimetableHandler(
    ITeacherScopeProvider scopeProvider,
    ICentreTimeZoneProvider timeZones,
    IScheduleReadService readService)
    : IQueryHandler<GetTimetableQuery, TimetableDto>
{
    public async Task<Result<TimetableDto>> HandleAsync(GetTimetableQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var zone = await timeZones.GetForCurrentCentreAsync(cancellationToken);
        var sessions = await readService.GetTimetableAsync(query.WeekStart, query.GroupId, query.TeacherId, scope, cancellationToken);
        return Result<TimetableDto>.Success(new TimetableDto(zone.Id, query.WeekStart, sessions));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
