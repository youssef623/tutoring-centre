using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class EndScheduleRuleHandlerTests
{
    private static readonly DateOnly Saturday13March = new(2027, 3, 13);

    [Fact]
    public async Task HandleAsync_UnknownRule_ReturnsRuleNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(Guid.CreateVersion7(), Saturday13March, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_EndsTheRuleAndCancelsOnlyTheSessionsAfterTheLastDate()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(sut.Rule.Id, Saturday13March, 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(Saturday13March, sut.Rule.EffectiveTo);
        Assert.All(sut.Sessions.Existing.Where(s => s.LocalDate <= Saturday13March), s => Assert.Equal(ClassSessionStatus.Scheduled, s.Status));
        Assert.All(sut.Sessions.Existing.Where(s => s.LocalDate > Saturday13March), s => Assert.Equal(ClassSessionStatus.Cancelled, s.Status));
        Assert.Equal(2, sut.Sessions.Existing.Count(s => s.Status == ClassSessionStatus.Cancelled));
        Assert.Empty(sut.Sessions.Added);
    }

    [Fact]
    public async Task HandleAsync_ASessionAfterTheLastDateThatHasStarted_IsLeftScheduled()
    {
        // Now is 6 March 16:30 UTC: the 6 March session (16:00-17:30 UTC) is under way.
        var sut = CreateSut(new DateTimeOffset(2027, 3, 6, 16, 30, 0, TimeSpan.Zero));

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(sut.Rule.Id, new DateOnly(2027, 3, 5), 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(ClassSessionStatus.Scheduled, sut.Sessions.Existing.Single(s => s.LocalDate == new DateOnly(2027, 3, 6)).Status);
        Assert.Equal(3, sut.Sessions.Existing.Count(s => s.Status == ClassSessionStatus.Cancelled));
    }

    [Fact]
    public async Task HandleAsync_AlreadyEndedRule_ReturnsRuleAlreadyEndedAndCancelsNothing()
    {
        var sut = CreateSut();
        sut.Rule.End(new DateOnly(2027, 3, 27));

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(sut.Rule.Id, Saturday13March, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_already_ended", result.Error!.Code);
        Assert.All(sut.Sessions.Existing, s => Assert.Equal(ClassSessionStatus.Scheduled, s.Status));
    }

    [Fact]
    public async Task HandleAsync_LastDateBeforeTheRuleTakesEffect_ReturnsRuleEndInvalid()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new EndScheduleRuleCommand(sut.Rule.Id, Today.AddDays(-1), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.rule_end_invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Null(sut.Rule.EffectiveTo);
    }

    private static Sut CreateSut(DateTimeOffset? now = null)
    {
        var teacherId = Guid.CreateVersion7();
        var rule = SaturdayRule(Guid.CreateVersion7());
        var rules = new FakeScheduleRuleRepository();
        rules.Existing.Add(rule);
        var sessions = new FakeClassSessionRepository();
        sessions.Existing.AddRange(Sessions(rule, teacherId, Today, Today.AddDays(27)));

        var clock = Clock();
        clock.UtcNow = now ?? Now;
        return new Sut(new EndScheduleRuleHandler(clock, rules, sessions), rule, sessions);
    }

    private sealed record Sut(EndScheduleRuleHandler Handler, ScheduleRule Rule, FakeClassSessionRepository Sessions);
}
