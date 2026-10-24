using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Domain.Tests.Academics.Scheduling;

/// <summary>
/// Task 51.4. Every expected instant is worked out by hand and written as a UTC literal; none is derived with the
/// conversion calls the generator itself makes.
///
/// Egypt (Africa/Cairo) since 2023: UTC+2 in winter, UTC+3 in summer. Clocks go forward at 00:00 on the last
/// Friday of April and back at 24:00 on the last Thursday of October. For 2027 that is Friday 30 April (00:00
/// becomes 01:00) and Thursday 28 October (24:00 becomes 23:00, so 23:00-23:59 happens twice). If Egypt's law
/// changes, these dates change and the generator does not. <see cref="TimeZoneData_HasEgypts2027Transitions"/>
/// checks the machine's time-zone data agrees before any failure below is trusted.
/// </summary>
public sealed class SessionGeneratorTests
{
    private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly Guid TeacherId = Guid.NewGuid();

    private static readonly DateOnly SpringForward2027 = new(2027, 4, 30);
    private static readonly DateOnly FallBack2027 = new(2027, 10, 28);

    [Fact]
    public void TimeZoneData_HasEgypts2027Transitions()
    {
        Assert.True(Cairo.IsInvalidTime(new DateTime(2027, 4, 30, 0, 30, 0)), "Cairo time-zone data lacks the 30 April 2027 spring-forward gap.");
        Assert.True(Cairo.IsAmbiguousTime(new DateTime(2027, 10, 28, 23, 30, 0)), "Cairo time-zone data lacks the 28 October 2027 repeated hour.");
        Assert.Equal(DayOfWeek.Friday, SpringForward2027.DayOfWeek);
        Assert.Equal(DayOfWeek.Thursday, FallBack2027.DayOfWeek);
    }

    // Case 1
    [Fact]
    public void Generate_WinterSaturdays_ProducesFourSessionsAtSixteenUtc()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2026, 12, 5));

        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2026, 12, 5), new DateOnly(2027, 1, 1));

        Assert.Equal(
            [new DateOnly(2026, 12, 5), new DateOnly(2026, 12, 12), new DateOnly(2026, 12, 19), new DateOnly(2026, 12, 26)],
            sessions.Select(session => session.LocalDate));
        Assert.Equal(
            [Utc(2026, 12, 5, 16, 0), Utc(2026, 12, 12, 16, 0), Utc(2026, 12, 19, 16, 0), Utc(2026, 12, 26, 16, 0)],
            sessions.Select(session => session.StartsAt));
        Assert.Equal(
            [Utc(2026, 12, 5, 17, 30), Utc(2026, 12, 12, 17, 30), Utc(2026, 12, 19, 17, 30), Utc(2026, 12, 26, 17, 30)],
            sessions.Select(session => session.EndsAt));
        Assert.All(sessions, session =>
        {
            Assert.Equal(TimeSpan.Zero, session.StartsAt.Offset);
            Assert.Equal(TeacherId, session.TeacherId);
            Assert.Equal(rule.Id, session.ScheduleRuleId);
            Assert.Equal(GroupId, session.GroupId);
            Assert.Equal(CentreId, session.CentreId);
            Assert.Equal(ClassSessionStatus.Scheduled, session.Status);
        });
    }

    // Case 2
    [Fact]
    public void Generate_RangeStartingMidWeek_FirstSessionIsOnTheNextMatchingWeekday()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2026, 11, 1));

        // Wednesday 2 December to Tuesday 15 December.
        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2026, 12, 2), new DateOnly(2026, 12, 15));

        Assert.Equal([new DateOnly(2026, 12, 5), new DateOnly(2026, 12, 12)], sessions.Select(session => session.LocalDate));
        Assert.Equal(Utc(2026, 12, 5, 16, 0), sessions[0].StartsAt);
    }

    // Case 3
    [Fact]
    public void Generate_EffectiveFromAndToInsideTheRange_BoundTheOutput()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2026, 12, 12));
        Assert.True(rule.End(new DateOnly(2026, 12, 19)).IsSuccess);

        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2026, 12, 1), new DateOnly(2026, 12, 31));

        Assert.Equal([new DateOnly(2026, 12, 12), new DateOnly(2026, 12, 19)], sessions.Select(session => session.LocalDate));
        Assert.Equal([Utc(2026, 12, 12, 16, 0), Utc(2026, 12, 19, 16, 0)], sessions.Select(session => session.StartsAt));
    }

    // Case 4
    [Fact]
    public void Generate_AcrossSpringForward_WallClockStaysEighteenAndUtcMovesAnHourEarlier()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2027, 1, 2));

        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2027, 4, 24), new DateOnly(2027, 5, 1));

        Assert.Equal([new DateOnly(2027, 4, 24), new DateOnly(2027, 5, 1)], sessions.Select(session => session.LocalDate));
        Assert.Equal(Utc(2027, 4, 24, 16, 0), sessions[0].StartsAt); // 18:00 at UTC+2
        Assert.Equal(Utc(2027, 5, 1, 15, 0), sessions[1].StartsAt);  // 18:00 at UTC+3
        Assert.Equal(new TimeSpan(6, 23, 0, 0), sessions[1].StartsAt - sessions[0].StartsAt);
    }

    // Case 5
    [Fact]
    public void Generate_AcrossFallBack_WallClockStaysEighteenAndUtcMovesAnHourLater()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2027, 1, 2));

        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2027, 10, 23), new DateOnly(2027, 10, 30));

        Assert.Equal([new DateOnly(2027, 10, 23), new DateOnly(2027, 10, 30)], sessions.Select(session => session.LocalDate));
        Assert.Equal(Utc(2027, 10, 23, 15, 0), sessions[0].StartsAt); // 18:00 at UTC+3
        Assert.Equal(Utc(2027, 10, 30, 16, 0), sessions[1].StartsAt); // 18:00 at UTC+2
        Assert.Equal(new TimeSpan(7, 1, 0, 0), sessions[1].StartsAt - sessions[0].StartsAt);
    }

    // Case 6
    [Fact]
    public void Generate_StartInTheSkippedHour_MovesForwardByTheGap()
    {
        var rule = Rule(DayOfWeek.Friday, new TimeOnly(0, 30), 60, new DateOnly(2027, 1, 1));

        var session = Assert.Single(SessionGenerator.Generate(rule, TeacherId, Cairo, SpringForward2027, SpringForward2027));

        // 00:30 does not exist that night; it becomes 01:30 local at UTC+3.
        Assert.Equal(SpringForward2027, session.LocalDate);
        Assert.Equal(Utc(2027, 4, 29, 22, 30), session.StartsAt);
        Assert.Equal(Utc(2027, 4, 29, 23, 30), session.EndsAt);
    }

    // Case 7
    [Fact]
    public void Generate_StartInTheRepeatedHour_TakesTheFirstOccurrence()
    {
        var rule = Rule(DayOfWeek.Thursday, new TimeOnly(23, 30), 60, new DateOnly(2027, 1, 7));

        var session = Assert.Single(SessionGenerator.Generate(rule, TeacherId, Cairo, FallBack2027, FallBack2027));

        // The first 23:30 is still at UTC+3; the second, an hour later, would be 21:30 UTC.
        Assert.Equal(FallBack2027, session.LocalDate);
        Assert.Equal(Utc(2027, 10, 28, 20, 30), session.StartsAt);
        Assert.Equal(Utc(2027, 10, 28, 21, 30), session.EndsAt);
    }

    // Case 8
    [Fact]
    public void Generate_SessionCrossingFallBack_LastsExactlyItsDurationInElapsedTime()
    {
        var rule = Rule(DayOfWeek.Thursday, new TimeOnly(22, 30), 120, new DateOnly(2027, 1, 7));

        var session = Assert.Single(SessionGenerator.Generate(rule, TeacherId, Cairo, FallBack2027, FallBack2027));

        // Starts 22:30 at UTC+3; ends at the second 23:30 (UTC+2) on the wall clock, 120 elapsed minutes later.
        Assert.Equal(Utc(2027, 10, 28, 19, 30), session.StartsAt);
        Assert.Equal(Utc(2027, 10, 28, 21, 30), session.EndsAt);
        Assert.Equal(TimeSpan.FromMinutes(120), session.EndsAt - session.StartsAt);
    }

    // Case 9
    [Fact]
    public void Generate_JustAfterLocalMidnight_LocalDateIsNotTheUtcDate()
    {
        var rule = Rule(DayOfWeek.Sunday, new TimeOnly(0, 30), 60, new DateOnly(2027, 1, 3));
        var sunday = new DateOnly(2027, 1, 10);

        var session = Assert.Single(SessionGenerator.Generate(rule, TeacherId, Cairo, sunday, sunday));

        Assert.Equal(sunday, session.LocalDate);
        Assert.Equal(Utc(2027, 1, 9, 22, 30), session.StartsAt);
        Assert.Equal(DayOfWeek.Saturday, session.StartsAt.UtcDateTime.DayOfWeek);
    }

    // Case 10
    [Fact]
    public void Generate_ToBeforeFrom_ReturnsEmpty()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2026, 12, 5));

        var sessions = SessionGenerator.Generate(rule, TeacherId, Cairo, new DateOnly(2026, 12, 31), new DateOnly(2026, 12, 1));

        Assert.Empty(sessions);
    }

    // Case 11
    [Fact]
    public void Generate_Twice_GivesTheSameDatesAndInstantsWithNewIds()
    {
        var rule = Rule(DayOfWeek.Saturday, new TimeOnly(18, 0), 90, new DateOnly(2027, 1, 2));
        var from = new DateOnly(2027, 4, 1);
        var to = new DateOnly(2027, 11, 30);

        var first = SessionGenerator.Generate(rule, TeacherId, Cairo, from, to);
        var second = SessionGenerator.Generate(rule, TeacherId, Cairo, from, to);

        Assert.NotEmpty(first);
        Assert.Equal(first.Select(session => (session.LocalDate, session.StartsAt, session.EndsAt)), second.Select(session => (session.LocalDate, session.StartsAt, session.EndsAt)));
        Assert.Empty(first.Select(session => session.Id).Intersect(second.Select(session => session.Id)));
    }

    private static ScheduleRule Rule(DayOfWeek dayOfWeek, TimeOnly startTime, int durationMinutes, DateOnly effectiveFrom) =>
        ScheduleRule.Create(CentreId, GroupId, dayOfWeek, startTime, durationMinutes, room: null, effectiveFrom).Value;

    private static DateTimeOffset Utc(int year, int month, int day, int hour, int minute) =>
        new(year, month, day, hour, minute, 0, TimeSpan.Zero);
}
