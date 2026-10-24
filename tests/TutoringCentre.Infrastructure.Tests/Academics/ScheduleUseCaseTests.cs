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

        // Each transaction inserts into the GiST index before checking it, so two truly simultaneous inserts can
        // each see the other's uncommitted row and wait on it; PostgreSQL then aborts one as a deadlock instead of
        // reporting the exclusion violation. Either way the loser stores nothing (later.md, Day 52).
        Assert.Contains(Assert.Single(outcomes, outcome => outcome != Success), new[] { "schedule.teacher_double_booked", Deadlock });
        Assert.Equal(1, await CountAsync("select count(*) from academics.schedule_rules"));
        Assert.Equal(4, await SessionCountAsync());
    }

    [Fact]
    public async Task ExtendSchedule_RunTwice_TheSecondRunCreatesNothing()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var through = DefaultToday.AddDays(55);

        var first = await ExtendAsync(world, through);
        var second = await ExtendAsync(world, through);

        Assert.Equal(4, first.SessionsCreated);
        Assert.Equal(0, second.SessionsCreated);
        Assert.Equal(8, await SessionCountAsync());
    }

    [Fact]
    public async Task ExtendSchedule_TwoRulesThatOnlyNowReachASharedSlot_FailsWholeWithNothingGenerated()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);

        // G2 (also T1) at the same time, but taking effect beyond the four-week horizon: nothing to collide yet.
        var later = await AddRuleOkAsync(world.Owner.Actor, world.G2, DayOfWeek.Saturday, SixPm, effectiveFrom: DefaultToday.AddDays(35));
        Assert.Equal(0, later.SessionsCreated);

        var result = await Clocked.SendAsAsync<ExtendScheduleCommand, ExtendScheduleResult>(
            world.Owner.Actor, new ExtendScheduleCommand(DefaultToday.AddDays(55)));

        AssertConflict(result, "schedule.teacher_double_booked");
        Assert.Equal(4, await SessionCountAsync());
    }

    [Fact]
    public async Task EndRule_CancelsLaterSessions_AndTheirSlotCanBeTakenByAnotherGroupOfTheSameTeacher()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        var added = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var rule = await GetRuleAsync(world.Owner.Actor, world.G1, added.ScheduleRuleId);
        var lastDate = new DateOnly(2027, 3, 13);

        var ended = await Clocked.SendAsAsync<EndScheduleRuleCommand, Unit>(
            world.Owner.Actor, new EndScheduleRuleCommand(added.ScheduleRuleId, lastDate, rule.Version));

        Assert.True(ended.IsSuccess, ended.Error?.Message);
        Assert.Equal(2, await SessionCountAsync($"local_date <= '{lastDate:yyyy-MM-dd}' and status = 'scheduled'"));
        Assert.Equal(2, await SessionCountAsync($"local_date > '{lastDate:yyyy-MM-dd}' and status = 'cancelled'"));
        Assert.Equal(1, await AuditCountAsync("schedule_rule", "updated"));
        Assert.Equal(2, await AuditCountAsync("class_session", "updated"));

        // G2 (also T1) from the day after the last date takes the freed Saturdays.
        var taken = await AddRuleOkAsync(world.Owner.Actor, world.G2, DayOfWeek.Saturday, SixPm, effectiveFrom: lastDate.AddDays(1));
        Assert.Equal(2, taken.SessionsCreated);
    }

    [Fact]
    public async Task CancelSession_FreesItsSlot_ForAnotherSessionOfTheSameTeacher()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var timetable = await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 3, 6));
        var session = Assert.Single(timetable.Sessions);

        var cancelled = await Clocked.SendAsAsync<CancelSessionCommand, Unit>(world.Owner.Actor, new CancelSessionCommand(session.Id, session.Version));

        Assert.True(cancelled.IsSuccess, cancelled.Error?.Message);
        Assert.Equal(1, await AuditCountAsync("class_session", "updated"));

        // Another group of the same teacher, in exactly the cancelled slot.
        var g2RuleId = await SeedRuleAsync(NileCentreId, world.G2);
        await InsertSessionAsync(new SessionRow(NileCentreId, world.G2, world.T1, g2RuleId, session.LocalDate, session.StartsAt, session.EndsAt));
        Assert.Equal(1, await SessionCountAsync($"group_id = '{world.G2}' and status = 'scheduled'"));
    }

    [Fact]
    public async Task AssignGroupTeacher_MovesFutureSessionsToTheNewTeacher_AndLeavesPastOnes()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);

        // Two weeks later (15 March): the 6 and 13 March sessions are in the past.
        Time.SetUtcNow(DefaultNow.AddDays(14));
        var group = await GetGroupAsync(world.Owner.Actor, world.G1);
        var assigned = await Clocked.SendAsAsync<AssignGroupTeacherCommand, Unit>(
            world.Owner.Actor, new AssignGroupTeacherCommand(world.G1, world.T2, group.Version));

        Assert.True(assigned.IsSuccess, assigned.Error?.Message);
        Assert.Equal(2, await SessionCountAsync($"local_date < '2027-03-15' and teacher_id = '{world.T1}'"));
        Assert.Equal(2, await SessionCountAsync($"local_date > '2027-03-15' and teacher_id = '{world.T2}'"));
        Assert.Equal(2, await AuditCountAsync("class_session", "updated"));
    }

    [Fact]
    public async Task AssignGroupTeacher_ToATeacherBusyAtThatTime_IsTeacherDoubleBookedAndChangesNothing()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        await AddRuleOkAsync(world.Owner.Actor, world.G3, DayOfWeek.Saturday, new TimeOnly(18, 30));
        var group = await GetGroupAsync(world.Owner.Actor, world.G1);

        var result = await Clocked.SendAsAsync<AssignGroupTeacherCommand, Unit>(
            world.Owner.Actor, new AssignGroupTeacherCommand(world.G1, world.T2, group.Version));

        AssertConflict(result, "schedule.teacher_double_booked");
        Assert.Equal(world.T1, (await GetGroupAsync(world.Owner.Actor, world.G1)).TeacherId);
        Assert.Equal(4, await SessionCountAsync($"group_id = '{world.G1}' and teacher_id = '{world.T1}'"));
        Assert.Equal(0, await AuditCountAsync("class_session", "updated"));
    }

    [Fact]
    public async Task ArchiveGroup_WithARuleStillInEffect_IsRefusedWithHasActiveSchedule()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var group = await GetGroupAsync(world.Owner.Actor, world.G1);

        var result = await Clocked.SendAsAsync<ArchiveGroupCommand, Unit>(world.Owner.Actor, new ArchiveGroupCommand(world.G1, group.Version));

        Assert.True(result.IsFailure);
        Assert.Equal("group.has_active_schedule", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(1, await CountAsync($"select count(*) from academics.groups where id = '{world.G1}' and status = 'active'"));
    }

    [Fact]
    public async Task CancelSession_ThatHasStarted_IsRefusedAndStaysScheduled()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        var session = Assert.Single((await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 3, 6))).Sessions);

        // 6 March 16:30 UTC: the 16:00-17:30 UTC session is under way.
        Time.SetUtcNow(session.StartsAt.AddMinutes(30));
        var result = await Clocked.SendAsAsync<CancelSessionCommand, Unit>(world.Owner.Actor, new CancelSessionCommand(session.Id, session.Version));

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.session_in_past", result.Error!.Code);
        Assert.Equal(ClassSessionStatus.Scheduled, Assert.Single((await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 3, 6))).Sessions).Status);
    }

    /// <summary>"success", the error code, or <see cref="Deadlock"/> when PostgreSQL aborted the save as a deadlock (40P01).</summary>
    private static async Task<string> OutcomeAsync<T>(Task<Result<T>> attempt)
    {
        try
        {
            var result = await attempt;
            return result.IsSuccess ? Success : result.Error!.Code;
        }
        catch (Exception exception) when (IsDeadlock(exception))
        {
            return Deadlock;
        }
    }

    private static bool IsDeadlock(Exception? exception) =>
        exception is not null && (exception is PostgresException { SqlState: "40P01" } || IsDeadlock(exception.InnerException));

    private async Task<ExtendScheduleResult> ExtendAsync(World world, DateOnly through)
    {
        var result = await Clocked.SendAsAsync<ExtendScheduleCommand, ExtendScheduleResult>(world.Owner.Actor, new ExtendScheduleCommand(through));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private static void AssertConflict(Result result, string code)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }
}
