using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: daylight saving time as stored and as selected. Cairo moves from UTC+2 to UTC+3 at 00:00 on Friday
/// 30 April 2027 (the clocks jump to 01:00). "Today" is Tuesday 20 April 2027, before the change.
/// </summary>
public sealed class ScheduleDstPersistenceTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private static readonly DateTimeOffset TwentiethOfApril = new(2027, 4, 20, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task SaturdayAtSixPm_IsStoredAtSixteenUtcBeforeTheChangeAndFifteenUtcAfterIt()
    {
        Time.SetUtcNow(TwentiethOfApril);
        var world = await ArrangeWorldAsync(NileCentreId, "nile");

        var added = await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm, effectiveFrom: new DateOnly(2027, 4, 20));

        Assert.Equal(
            new DateTimeOffset(2027, 4, 24, 16, 0, 0, TimeSpan.Zero),
            await StartsAtAsync(added.ScheduleRuleId, new DateOnly(2027, 4, 24)));
        Assert.Equal(
            new DateTimeOffset(2027, 5, 1, 15, 0, 0, TimeSpan.Zero),
            await StartsAtAsync(added.ScheduleRuleId, new DateOnly(2027, 5, 1)));
    }

    [Fact]
    public async Task FridayAtHalfPastMidnight_OnTheChangeDay_IsInTheTimetableUnderItsLocalDateNotItsUtcDate()
    {
        Time.SetUtcNow(TwentiethOfApril);
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        var added = await AddRuleOkAsync(
            world.Owner.Actor, world.G1, DayOfWeek.Friday, new TimeOnly(0, 30), effectiveFrom: new DateOnly(2027, 4, 20));
        var thirtiethOfApril = new DateOnly(2027, 4, 30);

        // 00:30 does not exist on 30 April (00:00 jumps to 01:00), so the session starts at 01:30 UTC+3: 22:30 UTC
        // on 29 April. Its UTC date is the 29th; its local date is the 30th.
        var startsAt = await StartsAtAsync(added.ScheduleRuleId, thirtiethOfApril);
        Assert.Equal(new DateTimeOffset(2027, 4, 29, 22, 30, 0, TimeSpan.Zero), startsAt);

        var weekWithThe30th = await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 4, 24));
        var weekEndingThe29th = await GetTimetableAsync(world.Owner.Actor, new DateOnly(2027, 4, 23));

        var session = Assert.Single(weekWithThe30th.Sessions);
        Assert.Equal(thirtiethOfApril, session.LocalDate);
        Assert.Equal(startsAt, session.StartsAt);
        Assert.Equal("Africa/Cairo", weekWithThe30th.TimeZoneId);
        Assert.DoesNotContain(weekEndingThe29th.Sessions, other => other.LocalDate == thirtiethOfApril);
    }

    private async Task<DateTimeOffset> StartsAtAsync(Guid ruleId, DateOnly localDate)
    {
        var startsAt = await Fixture.ScalarAsync<DateTime>(
            $"select starts_at from academics.class_sessions where schedule_rule_id = '{ruleId}' and local_date = '{localDate:yyyy-MM-dd}'");
        return new DateTimeOffset(DateTime.SpecifyKind(startsAt, DateTimeKind.Utc));
    }
}
