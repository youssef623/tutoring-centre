# `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 52.11: tenant isolation in both directions, through the real pipeline. Each centre has its own owner, teachers, groups and a Saturday 18:00 rule; another centre's IDs answer exactly like unknown IDs.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 113 lines and 5,093 bytes. Source hash: `43cf54509d9cc2809920541e23862c67d7db56cd306ad0edba35a9b65d575432`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 2: `using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;`
- Line 3: `using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;`
- Line 4: `using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;`
- Line 5: `using TutoringCentre.Application.Common.Cqrs;`
- Line 6: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L15): `public sealed class ScheduleTenantIsolationTests(PostgresFixture fixture) : ScheduleTestBase(fixture)`
- [Line 20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L20): `public async Task AddRule_ForAnotherCentresGroup_IsGroupNotFound()`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L32): `public async Task EndRule_OfAnotherCentre_IsRuleNotFoundAndTheRuleStaysOpen()`
- [Line 46](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L46): `public async Task CancelSession_OfAnotherCentre_IsSessionNotFoundAndTheSessionStaysScheduled()`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L60): `public async Task Timetable_NeverContainsAnotherCentresSessions_InEitherDirection()`
- [Line 72](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L72): `public async Task ListRules_OfAnotherCentresGroup_IsGroupNotFound()`
- [Line 84](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L84): `public async Task ANileTeacherAndAMaadiTeacher_MayHoldSessionsAtTheSameInstant()`
- [Line 106](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L106): `private async Task<IReadOnlyList<ScheduleRuleDto>> RulesAsync(World world, Guid groupId)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;
using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: tenant isolation in both directions, through the real pipeline. Each centre has its own owner,
/// teachers, groups and a Saturday 18:00 rule; another centre's IDs answer exactly like unknown IDs.
/// </summary>
public sealed class ScheduleTenantIsolationTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private static readonly DateOnly FirstSaturdayWeek = new(2027, 3, 6);

    [Fact]
    public async Task AddRule_ForAnotherCentresGroup_IsGroupNotFound()
    {
        var (nile, maadi) = await ArrangeBothAsync();

        var result = await AddRuleAsync(nile.Owner.Actor, maadi.G2, DayOfWeek.Sunday, SixPm);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(0, await CountAsync($"select count(*) from academics.schedule_rules where group_id = '{maadi.G2}'"));
    }

    [Fact]
    public async Task EndRule_OfAnotherCentre_IsRuleNotFoundAndTheRuleStaysOpen()
    {
        var (nile, maadi) = await ArrangeBothAsync();
        var maadiRule = (await RulesAsync(maadi, maadi.G1)).Single();

        var result = await Clocked.SendAsAsync<EndScheduleRuleCommand, Unit>(
            nile.Owner.Actor, new EndScheduleRuleCommand(maadiRule.Id, DefaultToday, maadiRule.Version));

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_not_found", result.Error!.Code);
        Assert.Equal(0, await CountAsync($"select count(*) from academics.schedule_rules where id = '{maadiRule.Id}' and effective_to is not null"));
    }

    [Fact]
    public async Task CancelSession_OfAnotherCentre_IsSessionNotFoundAndTheSessionStaysScheduled()
    {
        var (nile, maadi) = await ArrangeBothAsync();
        var maadiSession = (await GetTimetableAsync(maadi.Owner.Actor, FirstSaturdayWeek)).Sessions.Single();

        var result = await Clocked.SendAsAsync<CancelSessionCommand, Unit>(
            nile.Owner.Actor, new CancelSessionCommand(maadiSession.Id, maadiSession.Version));

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.session_not_found", result.Error!.Code);
        Assert.Equal(1, await SessionCountAsync($"id = '{maadiSession.Id}' and status = 'scheduled'"));
    }

    [Fact]
    public async Task Timetable_NeverContainsAnotherCentresSessions_InEitherDirection()
    {
        var (nile, maadi) = await ArrangeBothAsync();

        var nileWeek = await GetTimetableAsync(nile.Owner.Actor, FirstSaturdayWeek);
        var maadiWeek = await GetTimetableAsync(maadi.Owner.Actor, FirstSaturdayWeek);

        Assert.Equal(nile.G1, Assert.Single(nileWeek.Sessions).GroupId);
        Assert.Equal(maadi.G1, Assert.Single(maadiWeek.Sessions).GroupId);
    }

    [Fact]
    public async Task ListRules_OfAnotherCentresGroup_IsGroupNotFound()
    {
        var (nile, maadi) = await ArrangeBothAsync();

        var result = await Clocked.QueryAsAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            maadi.Owner.Actor, new ListGroupScheduleRulesQuery(nile.G1));

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task ANileTeacherAndAMaadiTeacher_MayHoldSessionsAtTheSameInstant()
    {
        // ArrangeBothAsync already gave both centres a Saturday 18:00 rule; both succeeded at the same instants.
        await ArrangeBothAsync();

        Assert.Equal(8, await SessionCountAsync("status = 'scheduled'"));
        Assert.Equal(
```

The excerpt stops at line 90; use the original file for the remaining 23 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
