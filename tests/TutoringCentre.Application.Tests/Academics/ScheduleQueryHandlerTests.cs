using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ScheduleQueryHandlerTests
{
    private static readonly DateOnly WeekStart = new(2027, 3, 6);

    [Fact]
    public async Task GetTimetable_ReturnsTheCentresZoneIdAndWeekStart_AndPassesTheScopeAndFilters()
    {
        var scope = TeacherScope.All;
        var reads = new FakeScheduleReadService();
        var groupId = Guid.CreateVersion7();
        var handler = new GetTimetableHandler(new FakeTeacherScopeProvider { Scope = scope }, new FakeCentreTimeZoneProvider(), reads);

        var result = await handler.HandleAsync(new GetTimetableQuery(WeekStart, groupId, null), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal("Africa/Cairo", result.Value.TimeZoneId);
        Assert.Equal(WeekStart, result.Value.WeekStart);
        Assert.Same(scope, reads.LastScope);
        Assert.Equal((WeekStart, (Guid?)groupId, (Guid?)null), reads.LastTimetableRequest);
    }

    [Fact]
    public async Task GetTimetable_OwnScope_ReturnsOnlyTheTeachersOwnSessions()
    {
        var ownTeacherId = Guid.CreateVersion7();
        var reads = new FakeScheduleReadService();
        reads.Sessions.Add(Session(ownTeacherId));
        reads.Sessions.Add(Session(Guid.CreateVersion7()));
        var handler = new GetTimetableHandler(
            new FakeTeacherScopeProvider { Scope = TeacherScope.Own(ownTeacherId) }, new FakeCentreTimeZoneProvider(), reads);

        var result = await handler.HandleAsync(new GetTimetableQuery(WeekStart, null, null), CancellationToken.None);

        Assert.Equal(ownTeacherId, Assert.Single(result.Value.Sessions).TeacherId);
    }

    [Fact]
    public async Task ListGroupScheduleRules_GroupInScope_ReturnsItsRules()
    {
        var groupId = Guid.CreateVersion7();
        var teacherId = Guid.CreateVersion7();
        var rule = new ScheduleRuleDto(Guid.CreateVersion7(), DayOfWeek.Saturday, new TimeOnly(18, 0), 90, null, WeekStart, null, 1);
        var reads = new FakeScheduleReadService();
        reads.GroupRules.Add((groupId, teacherId, [rule]));
        var handler = new ListGroupScheduleRulesHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(teacherId) }, reads);

        var result = await handler.HandleAsync(new ListGroupScheduleRulesQuery(groupId), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(rule, Assert.Single(result.Value));
    }

    [Fact]
    public async Task ListGroupScheduleRules_GroupOutOfScope_ReturnsGroupNotFound()
    {
        var groupId = Guid.CreateVersion7();
        var reads = new FakeScheduleReadService();
        reads.GroupRules.Add((groupId, Guid.CreateVersion7(), []));
        var handler = new ListGroupScheduleRulesHandler(
            new FakeTeacherScopeProvider { Scope = TeacherScope.Own(Guid.CreateVersion7()) }, reads);

        var result = await handler.HandleAsync(new ListGroupScheduleRulesQuery(groupId), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    private static TimetableSessionDto Session(Guid teacherId) => new(
        Guid.CreateVersion7(),
        Guid.CreateVersion7(),
        "Grade 10 A",
        "Mathematics",
        teacherId,
        "Teacher",
        WeekStart,
        new DateTimeOffset(2027, 3, 6, 16, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2027, 3, 6, 17, 30, 0, TimeSpan.Zero),
        null,
        ClassSessionStatus.Scheduled,
        1);
}
