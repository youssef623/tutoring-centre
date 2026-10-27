# `tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for schedule reads. Each group's rules carry the group's teacher and each session its own teacher, so the scope is applied the way the real adapter applies it; records the last scope and filters asked for.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 45 lines and 2,025 bytes. Source hash: `69f4071a745f27ebcc812fbe587faf772ac2726ecb08a2b32c9f4c18860f1162`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 2: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleReadService.cs#L10): `public sealed class FakeScheduleReadService : IScheduleReadService`
- [Line 20](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleReadService.cs#L20): `public Task<IReadOnlyList<ScheduleRuleDto>?> ListRulesAsync(Guid groupId, TeacherScope scope, CancellationToken ct)`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleReadService.cs#L30): `public Task<IReadOnlyList<TimetableSessionDto>> GetTimetableAsync(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:148](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L148)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs:19](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for schedule reads. Each group's rules carry the group's teacher and each session its own
/// teacher, so the scope is applied the way the real adapter applies it; records the last scope and filters asked for.
/// </summary>
public sealed class FakeScheduleReadService : IScheduleReadService
{
    public List<(Guid GroupId, Guid TeacherId, IReadOnlyList<ScheduleRuleDto> Rules)> GroupRules { get; } = [];

    public List<TimetableSessionDto> Sessions { get; } = [];

    public TeacherScope? LastScope { get; private set; }

    public (DateOnly WeekStart, Guid? GroupId, Guid? TeacherId)? LastTimetableRequest { get; private set; }

    public Task<IReadOnlyList<ScheduleRuleDto>?> ListRulesAsync(Guid groupId, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        var match = GroupRules
            .Where(row => row.GroupId == groupId && scope.Allows(row.TeacherId))
            .Select(row => row.Rules)
            .FirstOrDefault();
        return Task.FromResult(match);
    }

    public Task<IReadOnlyList<TimetableSessionDto>> GetTimetableAsync(
        DateOnly weekStart, Guid? groupId, Guid? teacherId, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        LastTimetableRequest = (weekStart, groupId, teacherId);
        var weekEnd = weekStart.AddDays(6);
        IReadOnlyList<TimetableSessionDto> visible = Sessions
            .Where(session => session.LocalDate >= weekStart && session.LocalDate <= weekEnd)
            .Where(session => scope.Allows(session.TeacherId))
            .Where(session => groupId is null || session.GroupId == groupId)
            .Where(session => teacherId is null || session.TeacherId == teacherId)
            .OrderBy(session => session.StartsAt)
            .ToList();
        return Task.FromResult(visible);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
