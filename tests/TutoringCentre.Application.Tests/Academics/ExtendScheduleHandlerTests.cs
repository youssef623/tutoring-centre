using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ExtendScheduleHandlerTests
{
    private static readonly DateOnly Saturday10April = new(2027, 4, 10);

    [Fact]
    public async Task HandleAsync_ThroughBeforeToday_ReturnsRangeInvalidOnThrough()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Today.AddDays(-1)), CancellationToken.None);

        AssertRangeInvalid(result, sut);
    }

    [Fact]
    public async Task HandleAsync_ThroughMoreThan92DaysAhead_ReturnsRangeInvalidOnThrough()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Today.AddDays(93)), CancellationToken.None);

        AssertRangeInvalid(result, sut);
    }

    [Fact]
    public async Task HandleAsync_ThroughExactly92DaysAhead_IsAccepted()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Today.AddDays(92)), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    [Fact]
    public async Task HandleAsync_AddsOnlyTheDatesThatHaveNoSessionYet()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(2, result.Value.SessionsCreated);
        Assert.Equal([new DateOnly(2027, 4, 3), Saturday10April], sut.Sessions.Added.Select(s => s.LocalDate).ToArray());
    }

    [Fact]
    public async Task HandleAsync_RunTwice_TheSecondRunCreatesNothing()
    {
        var sut = CreateSut();
        await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);
        sut.Sessions.Existing.AddRange(sut.Sessions.Added);
        sut.Sessions.Added.Clear();

        var second = await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);

        Assert.True(second.IsSuccess, second.Error?.Message);
        Assert.Equal(0, second.Value.SessionsCreated);
        Assert.Empty(sut.Sessions.Added);
    }

    [Fact]
    public async Task HandleAsync_NewSessionsGoToTheGroupsCurrentTeacher()
    {
        var sut = CreateSut();
        var newTeacherId = Guid.CreateVersion7();
        sut.Group.AssignTeacher(newTeacherId);

        await sut.Handler.HandleAsync(new ExtendScheduleCommand(Saturday10April), CancellationToken.None);

        Assert.All(sut.Sessions.Added, session => Assert.Equal(newTeacherId, session.TeacherId));
    }

    private static void AssertRangeInvalid(Result result, Sut sut)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("schedule.range_invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Contains("through", result.Error.Fields!.Keys);
        Assert.Empty(sut.Sessions.Added);
    }

    /// <summary>One Saturday rule whose first four weeks (6-27 March) already exist, as after adding it.</summary>
    private static Sut CreateSut()
    {
        var group = NewGroup();
        var groups = new FakeGroupRepository();
        groups.Existing.Add(group);
        var rule = SaturdayRule(group.Id);
        var rules = new FakeScheduleRuleRepository();
        rules.Existing.Add(rule);
        rules.ActiveGroupIds.Add(group.Id);
        var sessions = new FakeClassSessionRepository();
        sessions.Existing.AddRange(Sessions(rule, group.TeacherId, Today, Today.AddDays(27)));

        var handler = new ExtendScheduleHandler(Clock(), new FakeCentreTimeZoneProvider(), groups, rules, sessions);
        return new Sut(handler, group, sessions);
    }

    private sealed record Sut(ExtendScheduleHandler Handler, Group Group, FakeClassSessionRepository Sessions);
}
