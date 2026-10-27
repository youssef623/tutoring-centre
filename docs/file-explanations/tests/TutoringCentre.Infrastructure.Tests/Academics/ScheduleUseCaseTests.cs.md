# `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 52.11: the schedule use cases end to end — dispatcher, handler, EF, interceptors, the exclusion constraints and their translation, one transaction per command — with the clock fixed at Monday 1 March 2027 (Cairo winter time, UTC+2). In Nile, G1 and G2 are taught by T1 and G3 by T2. Saturday 18:00 for 90 minutes is 16:00-17:30 UTC.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 258 lines and 13,487 bytes. Source hash: `968448a3f9d99ace3c77684b8d81fb010dd4843fdece762b9ab2f591cc48d0b5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;`
- Line 4: `using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;`
- Line 5: `using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;`
- Line 6: `using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;`
- Line 7: `using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;`
- Line 8: `using TutoringCentre.Application.Common.Cqrs;`
- Line 9: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 10: `using TutoringCentre.Domain.Common;`
- Line 11: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L20): `public sealed class ScheduleUseCaseTests(PostgresFixture fixture) : ScheduleTestBase(fixture)`
- [Line 26](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L26): `public async Task AddRule_StoresOneRuleAndFourSessions_AuditsTheRuleAndNoSession()`
- [Line 40](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L40): `public async Task AddRule_SecondGroupOfTheSameTeacherAtAnOverlappingTime_IsTeacherDoubleBookedAndLeavesNoTrace()`
- [Line 55](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L55): `public async Task AddRule_SecondRuleForTheSameGroupOverlappingTheFirst_IsGroupDoubleBooked()`
- [Line 68](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L68): `public async Task AddRule_TwoParallelCollidingRulesForOneTeacher_ExactlyOneSucceeds()`
- [Line 90](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L90): `public async Task ExtendSchedule_RunTwice_TheSecondRunCreatesNothing()`
- [Line 105](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L105): `public async Task ExtendSchedule_TwoRulesThatOnlyNowReachASharedSlot_FailsWholeWithNothingGenerated()`
- [Line 122](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L122): `public async Task EndRule_CancelsLaterSessions_AndTheirSlotCanBeTakenByAnotherGroupOfTheSameTeacher()`
- [Line 144](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L144): `public async Task CancelSession_FreesItsSlot_ForAnotherSessionOfTheSameTeacher()`
- [Line 163](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L163): `public async Task AssignGroupTeacher_MovesFutureSessionsToTheNewTeacher_AndLeavesPastOnes()`
- [Line 181](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L181): `public async Task AssignGroupTeacher_ToATeacherBusyAtThatTime_IsTeacherDoubleBookedAndChangesNothing()`
- [Line 198](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L198): `public async Task ArchiveGroup_WithARuleStillInEffect_IsRefusedWithHasActiveSchedule()`
- [Line 213](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L213): `public async Task CancelSession_ThatHasStarted_IsRefusedAndStaysScheduled()`
- [Line 229](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L229): `private static async Task<string> OutcomeAsync<T>(Task<Result<T>> attempt)`
- [Line 242](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L242): `private static bool IsDeadlock(Exception? exception) =>`
- [Line 245](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L245): `private async Task<ExtendScheduleResult> ExtendAsync(World world, DateOnly through)`
- [Line 252](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L252): `private static void AssertConflict(Result result, string code)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Npgsql;
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;
using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: the schedule use cases end to end — dispatcher, handler, EF, interceptors, the exclusion constraints
/// and their translation, one transaction per command — with the clock fixed at Monday 1 March 2027 (Cairo winter
/// time, UTC+2). In Nile, G1 and G2 are taught by T1 and G3 by T2. Saturday 18:00 for 90 minutes is 16:00-17:30 UTC.
/// </summary>
public sealed class ScheduleUseCaseTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private const string Success = "success";
    private const string Deadlock = "deadlock";

    [Fact]
    public async Task AddRule_StoresOneRuleAndFourSessions_AuditsTheRuleAndNoSession()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");

        var added = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);

        Assert.Equal(4, added.SessionsCreated);
        Assert.Equal(1, await CountAsync("select count(*) from academics.schedule_rules"));
        Assert.Equal(4, await SessionCountAsync($"schedule_rule_id = '{added.ScheduleRuleId}' and teacher_id = '{world.T1}' and status = 'scheduled'"));
        Assert.Equal(1, await AuditCountAsync("schedule_rule", "created"));
        Assert.Equal(0, await CountAsync("select count(*) from audit.audit_entries where entity_type = 'class_session'"));
    }

    [Fact]
    public async Task AddRule_SecondGroupOfTheSameTeacherAtAnOverlappingTime_IsTeacherDoubleBookedAndLeavesNoTrace()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var auditRows = await CountAsync("select count(*) from audit.audit_entries");

        var result = await AddRuleAsync(world.Owner.Actor, world.G2, DayOfWeek.Saturday, new TimeOnly(18, 30));

        AssertConflict(result, "schedule.teacher_double_booked");
        Assert.Equal(1, await CountAsync("select count(*) from academics.schedule_rules"));
        Assert.Equal(4, await SessionCountAsync());
        Assert.Equal(auditRows, await CountAsync("select count(*) from audit.audit_entries"));
    }

    [Fact]
    public async Task AddRule_SecondRuleForTheSameGroupOverlappingTheFirst_IsGroupDoubleBooked()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);

        var result = await AddRuleAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, new TimeOnly(19, 0));

        AssertConflict(result, "schedule.group_double_booked");
        Assert.Equal(1, await CountAsync("select count(*) from academics.schedule_rules"));
        Assert.Equal(4, await SessionCountAsync());
    }

    [Fact]
    public async Task AddRule_TwoParallelCollidingRulesForOneTeacher_ExactlyOneSucceeds()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");

        var outcomes = await RunTogetherAsync(
        [
            () => OutcomeAsync(AddRuleAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm)),
            () => OutcomeAsync(AddRuleAsync(world.Owner.Actor, world.G2, DayOfWeek.Saturday, SixPm)),
        ]);

        Assert.Single(outcomes, outcome => outcome == Success);

        // Each transaction inserts into the GiST index before checking it, so without serialisation two truly
        // simultaneous inserts could each wait on the other's uncommitted row and end in a deadlock (Day 52). Since
        // Day 53 the handler takes the teacher's advisory lock first, so the loser always waits, then meets the
        // committed row and is refused by the constraint: never a deadlock.
        Assert.Equal("schedule.teacher_double_booked", Assert.Single(outcomes, outcome => outcome != Success));
        Assert.Equal(1, await CountAsync("select count(*) from academics.schedule_rules"));
        Assert.Equal(4, await SessionCountAsync());
    }

    [Fact]
    public async Task ExtendSchedule_RunTwice_TheSecondRunCreatesNothing()
```

The excerpt stops at line 90; use the original file for the remaining 168 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
