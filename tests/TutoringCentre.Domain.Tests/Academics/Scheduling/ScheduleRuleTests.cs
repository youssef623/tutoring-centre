using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics.Scheduling;

public sealed class ScheduleRuleTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly TimeOnly SixPm = new(18, 0);

    // Saturday 5 December 2026.
    private static readonly DateOnly EffectiveFrom = new(2026, 12, 5);

    [Fact]
    public void Create_WithValidInput_StoresLocalValuesAndIsOpenEnded()
    {
        var result = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, "  Room 2  ", EffectiveFrom);

        Assert.True(result.IsSuccess);
        var rule = result.Value;
        Assert.Equal(CentreId, rule.CentreId);
        Assert.Equal(GroupId, rule.GroupId);
        Assert.Equal(DayOfWeek.Saturday, rule.DayOfWeek);
        Assert.Equal(SixPm, rule.StartTime);
        Assert.Equal(90, rule.DurationMinutes);
        Assert.Equal("Room 2", rule.Room);
        Assert.Equal(EffectiveFrom, rule.EffectiveFrom);
        Assert.Null(rule.EffectiveTo);
        Assert.NotEqual(Guid.Empty, rule.Id);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(
            ScheduleRule.Create(Guid.Empty, GroupId, DayOfWeek.Saturday, SixPm, 90, null, EffectiveFrom), "schedule.centre_required", field: null);

    [Fact]
    public void Create_WithEmptyGroup_ReturnsGroupRequired() =>
        AssertValidationFailure(
            ScheduleRule.Create(CentreId, Guid.Empty, DayOfWeek.Saturday, SixPm, 90, null, EffectiveFrom), "schedule.group_required", field: null);

    [Theory]
    [InlineData(25, false)]
    [InlineData(30, true)]
    [InlineData(92, false)]
    [InlineData(240, true)]
    [InlineData(245, false)]
    public void Create_DurationBoundaries(int durationMinutes, bool valid)
    {
        var result = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, durationMinutes, null, EffectiveFrom);

        if (valid)
        {
            Assert.True(result.IsSuccess);
            Assert.Equal(durationMinutes, result.Value.DurationMinutes);
        }
        else
        {
            AssertValidationFailure(result, "schedule.duration_invalid", "durationMinutes");
        }
    }

    [Theory]
    [InlineData(18, 2, 0)]
    [InlineData(18, 0, 30)]
    public void Create_WithStartTimeOffTheFiveMinuteGrid_ReturnsStartTimeInvalid(int hour, int minute, int second) =>
        AssertValidationFailure(
            ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, new TimeOnly(hour, minute, second), 90, null, EffectiveFrom),
            "schedule.start_time_invalid",
            "startTime");

    [Fact]
    public void Create_With41CharacterRoom_ReturnsRoomInvalid() =>
        AssertValidationFailure(
            ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, new string('r', 41), EffectiveFrom),
            "schedule.room_invalid",
            "room");

    [Fact]
    public void Create_With40CharacterRoom_Succeeds()
    {
        var room = new string('r', 40);

        var result = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, room, EffectiveFrom);

        Assert.True(result.IsSuccess);
        Assert.Equal(room, result.Value.Room);
        Assert.Equal(40, ScheduleRule.RoomMaxLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithBlankRoom_StoresNull(string? room)
    {
        var result = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, room, EffectiveFrom);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Room);
    }

    // Rule effective Saturday 12 December to Saturday 26 December 2026.
    [Theory]
    [InlineData(2026, 12, 5, false)]  // the weekday before the range
    [InlineData(2026, 12, 12, true)]  // first day of the range
    [InlineData(2026, 12, 13, false)] // in range, wrong weekday
    [InlineData(2026, 12, 19, true)]
    [InlineData(2026, 12, 26, true)]  // last day of the range
    [InlineData(2027, 1, 2, false)]   // the weekday after the range
    public void OccursOn_MatchesTheWeekdayInsideTheEffectiveRange(int year, int month, int day, bool expected)
    {
        var rule = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, null, new DateOnly(2026, 12, 12)).Value;
        Assert.True(rule.End(new DateOnly(2026, 12, 26)).IsSuccess);

        Assert.Equal(expected, rule.OccursOn(new DateOnly(year, month, day)));
    }

    [Fact]
    public void OccursOn_OpenEndedRule_KeepsOccurringOnTheWeekday()
    {
        var rule = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, null, EffectiveFrom).Value;

        Assert.True(rule.OccursOn(new DateOnly(2030, 1, 5)));
    }

    [Fact]
    public void End_OnTheEffectiveFromDate_Succeeds()
    {
        var rule = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, null, EffectiveFrom).Value;

        Assert.True(rule.End(EffectiveFrom).IsSuccess);
        Assert.Equal(EffectiveFrom, rule.EffectiveTo);
    }

    [Fact]
    public void End_BeforeEffectiveFrom_ReturnsRuleEndInvalidAndStaysOpen()
    {
        var rule = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, null, EffectiveFrom).Value;

        var result = rule.End(EffectiveFrom.AddDays(-1));

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_end_invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Null(rule.EffectiveTo);
    }

    [Fact]
    public void End_Twice_ReturnsRuleAlreadyEndedAndKeepsTheFirstEnd()
    {
        var rule = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, SixPm, 90, null, EffectiveFrom).Value;
        var firstEnd = EffectiveFrom.AddDays(14);
        Assert.True(rule.End(firstEnd).IsSuccess);

        var result = rule.End(EffectiveFrom.AddDays(28));

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_already_ended", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(firstEnd, rule.EffectiveTo);
    }

    private static void AssertValidationFailure(Result<ScheduleRule> result, string expectedCode, string? field)
    {
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        if (field is not null)
        {
            Assert.NotNull(result.Error.Fields);
            Assert.True(result.Error.Fields.ContainsKey(field));
        }
    }
}
