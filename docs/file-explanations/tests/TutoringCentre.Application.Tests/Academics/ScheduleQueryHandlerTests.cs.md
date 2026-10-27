# `tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs`

## Purpose

Declares `ScheduleQueryHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 92 lines and 4,047 bytes. Source hash: `40a1ef381ef2f02466d2ccc58e7c4ba98fb2539a273313276b11da26f5619ef7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 2: `using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;`
- Line 3: `using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Application.Tests.Fakes;`
- Line 6: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 7: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L11): `public sealed class ScheduleQueryHandlerTests`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L16): `public async Task GetTimetable_ReturnsTheCentresZoneIdAndWeekStart_AndPassesTheScopeAndFilters()`
- [Line 33](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L33): `public async Task GetTimetable_OwnScope_ReturnsOnlyTheTeachersOwnSessions()`
- [Line 48](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L48): `public async Task ListGroupScheduleRules_GroupInScope_ReturnsItsRules()`
- [Line 64](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L64): `public async Task ListGroupScheduleRules_GroupOutOfScope_ReturnsGroupNotFound()`
- [Line 79](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs#L79): `private static TimetableSessionDto Session(Guid teacherId) => new(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ScheduleQueryHandlerTests
{
    private static readonly DateOnly WeekStart = new(2027, 3, 6);

    [Fact]
    public async Task GetTimetable_ReturnsTheCentresZoneIdAndWeekStart_AndPassesTheScopeAndFilters()
    {
        var scope = TeacherScope.All;
        var reads = new FakeScheduleReadService();
        var groupId = Guid.CreateVersion7();
        var handler = new GetTimetableHandler(new FakeTeacherScopeProvider { Scope = scope }, new FakeCentreTimeZoneProvider(), reads);

        var result = await handler.HandleAsync(new GetTimetableQuery(WeekStart, groupId, null), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal("Africa/Cairo", result.Value.TimeZoneId);
        Assert.Equal(WeekStart, result.Value.WeekStart);
        Assert.Same(scope, reads.LastScope);
        Assert.Equal((WeekStart, (Guid?)groupId, (Guid?)null), reads.LastTimetableRequest);
    }

    [Fact]
    public async Task GetTimetable_OwnScope_ReturnsOnlyTheTeachersOwnSessions()
    {
        var ownTeacherId = Guid.CreateVersion7();
        var reads = new FakeScheduleReadService();
        reads.Sessions.Add(Session(ownTeacherId));
        reads.Sessions.Add(Session(Guid.CreateVersion7()));
        var handler = new GetTimetableHandler(
            new FakeTeacherScopeProvider { Scope = TeacherScope.Own(ownTeacherId) }, new FakeCentreTimeZoneProvider(), reads);

        var result = await handler.HandleAsync(new GetTimetableQuery(WeekStart, null, null), CancellationToken.None);

        Assert.Equal(ownTeacherId, Assert.Single(result.Value.Sessions).TeacherId);
    }

    [Fact]
    public async Task ListGroupScheduleRules_GroupInScope_ReturnsItsRules()
    {
        var groupId = Guid.CreateVersion7();
        var teacherId = Guid.CreateVersion7();
        var rule = new ScheduleRuleDto(Guid.CreateVersion7(), DayOfWeek.Saturday, new TimeOnly(18, 0), 90, null, WeekStart, null, 1);
        var reads = new FakeScheduleReadService();
        reads.GroupRules.Add((groupId, teacherId, [rule]));
        var handler = new ListGroupScheduleRulesHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(teacherId) }, reads);

        var result = await handler.HandleAsync(new ListGroupScheduleRulesQuery(groupId), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(rule, Assert.Single(result.Value));
    }

    [Fact]
    public async Task ListGroupScheduleRules_GroupOutOfScope_ReturnsGroupNotFound()
    {
        var groupId = Guid.CreateVersion7();
        var reads = new FakeScheduleReadService();
        reads.GroupRules.Add((groupId, Guid.CreateVersion7(), []));
        var handler = new ListGroupScheduleRulesHandler(
            new FakeTeacherScopeProvider { Scope = TeacherScope.Own(Guid.CreateVersion7()) }, reads);

        var result = await handler.HandleAsync(new ListGroupScheduleRulesQuery(groupId), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    private static TimetableSessionDto Session(Guid teacherId) => new(
        Guid.CreateVersion7(),
        Guid.CreateVersion7(),
        "Grade 10 A",
        "Mathematics",
        teacherId,
        "Teacher",
        WeekStart,
        new DateTimeOffset(2027, 3, 6, 16, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2027, 3, 6, 17, 30, 0, TimeSpan.Zero),
        null,
        ClassSessionStatus.Scheduled,
```

The excerpt stops at line 90; use the original file for the remaining 2 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
