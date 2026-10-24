using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class AddScheduleRuleHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFoundAndAddsNothing()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(Guid.CreateVersion7()), CancellationToken.None);

        AssertFailure(result, "group.not_found", ErrorKind.NotFound, sut);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsGroupArchivedAndAddsNothing()
    {
        var sut = CreateSut();
        sut.Group.Archive();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id), CancellationToken.None);

        AssertFailure(result, "group.archived", ErrorKind.Rule, sut);
    }

    [Fact]
    public async Task HandleAsync_EffectiveFromMoreThan366DaysAhead_ReturnsRangeInvalidOnEffectiveFrom()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id, Today.AddDays(367)), CancellationToken.None);

        AssertFailure(result, "schedule.range_invalid", ErrorKind.Rule, sut);
        Assert.Contains("effectiveFrom", result.Error!.Fields!.Keys);
    }

    [Fact]
    public async Task HandleAsync_EffectiveFromExactly366DaysAhead_AddsTheRuleWithNoSessionsYet()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id, Today.AddDays(366)), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Single(sut.Rules.Added);
        Assert.Equal(0, result.Value.SessionsCreated);
        Assert.Empty(sut.Sessions.Added);
    }

    [Fact]
    public async Task HandleAsync_RuleFromToday_AddsTheRuleAndFourWeeksOfSessionsForTheGroupsTeacher()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        var rule = Assert.Single(sut.Rules.Added);
        Assert.Equal(rule.Id, result.Value.ScheduleRuleId);
        Assert.Equal(CentreId, rule.CentreId);
        Assert.Equal(4, result.Value.SessionsCreated);
        Assert.Equal(
            [new DateOnly(2027, 3, 6), new DateOnly(2027, 3, 13), new DateOnly(2027, 3, 20), new DateOnly(2027, 3, 27)],
            sut.Sessions.Added.Select(session => session.LocalDate).ToArray());
        Assert.All(sut.Sessions.Added, session =>
        {
            Assert.Equal(sut.Group.TeacherId, session.TeacherId);
            Assert.Equal(rule.Id, session.ScheduleRuleId);
        });

        // 18:00 in Cairo's winter (UTC+2) is 16:00 UTC; 90 minutes later is 17:30 UTC.
        var first = sut.Sessions.Added[0];
        Assert.Equal(new DateTimeOffset(2027, 3, 6, 16, 0, 0, TimeSpan.Zero), first.StartsAt);
        Assert.Equal(new DateTimeOffset(2027, 3, 6, 17, 30, 0, TimeSpan.Zero), first.EndsAt);
    }

    [Fact]
    public async Task HandleAsync_RuleThatTookEffectInThePast_GeneratesNoDateBeforeToday()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id, Today.AddDays(-30)), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(4, result.Value.SessionsCreated);
        Assert.All(sut.Sessions.Added, session => Assert.True(session.LocalDate >= Today));
    }

    [Fact]
    public async Task HandleAsync_RuleFromLaterInTheHorizon_GeneratesFromItsFirstDateOnly()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(Command(sut.Group.Id, new DateOnly(2027, 3, 15)), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(
            [new DateOnly(2027, 3, 20), new DateOnly(2027, 3, 27)],
            sut.Sessions.Added.Select(session => session.LocalDate).ToArray());
    }

    [Fact]
    public async Task HandleAsync_DurationOffTheFiveMinuteStep_ReturnsTheRulesOwnError()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AddScheduleRuleCommand(sut.Group.Id, DayOfWeek.Saturday, new TimeOnly(18, 0), 47, null, Today), CancellationToken.None);

        AssertFailure(result, "schedule.duration_invalid", ErrorKind.Validation, sut);
    }

    private static AddScheduleRuleCommand Command(Guid groupId, DateOnly? effectiveFrom = null) =>
        new(groupId, DayOfWeek.Saturday, new TimeOnly(18, 0), 90, "Room 1", effectiveFrom ?? Today);

    private static void AssertFailure(Result result, string code, ErrorKind kind, Sut sut)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(kind, result.Error.Kind);
        Assert.Empty(sut.Rules.Added);
        Assert.Empty(sut.Sessions.Added);
    }

    private static Sut CreateSut()
    {
        var group = NewGroup();
        var groups = new FakeGroupRepository();
        groups.Existing.Add(group);
        var rules = new FakeScheduleRuleRepository();
        var sessions = new FakeClassSessionRepository();
        var actor = new FakeCurrentActor { Actor = new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Secretary) };

        var handler = new AddScheduleRuleHandler(actor, Clock(), new FakeCentreTimeZoneProvider(), groups, rules, sessions);
        return new Sut(handler, group, rules, sessions);
    }

    private sealed record Sut(AddScheduleRuleHandler Handler, Group Group, FakeScheduleRuleRepository Rules, FakeClassSessionRepository Sessions);
}
