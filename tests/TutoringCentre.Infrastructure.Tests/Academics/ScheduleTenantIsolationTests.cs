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
            4,
            await CountAsync(
                $"select count(*) from academics.class_sessions n join academics.class_sessions m on n.starts_at = m.starts_at "
                + $"where n.centre_id = '{NileCentreId}' and m.centre_id = '{MaadiCentreId}'"));
    }

    private async Task<(World Nile, World Maadi)> ArrangeBothAsync()
    {
        var nile = await ArrangeWorldAsync(NileCentreId, "nile");
        var maadi = await ArrangeWorldAsync(MaadiCentreId, "maadi");
        await AddRuleOkAsync(nile.Owner.Actor, nile.G1, DayOfWeek.Saturday, SixPm);
        await AddRuleOkAsync(maadi.Owner.Actor, maadi.G1, DayOfWeek.Saturday, SixPm);
        return (nile, maadi);
    }

    private async Task<IReadOnlyList<ScheduleRuleDto>> RulesAsync(World world, Guid groupId)
    {
        var result = await Clocked.QueryAsAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            world.Owner.Actor, new ListGroupScheduleRulesQuery(groupId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }
}
