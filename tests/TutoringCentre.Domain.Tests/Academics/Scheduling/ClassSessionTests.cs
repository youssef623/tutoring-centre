using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics.Scheduling;

/// <summary>Task 51.5. Every session comes from <see cref="SessionGenerator"/>: it is the only way to obtain one.</summary>
public sealed class ClassSessionTests
{
    private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly Guid TeacherId = Guid.NewGuid();
    private static readonly Guid OtherTeacherId = Guid.NewGuid();

    // Saturday 12 December 2026, 18:00 Cairo = 16:00 UTC.
    private static readonly DateTimeOffset StartsAt = new(2026, 12, 12, 16, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset BeforeStart = StartsAt.AddMinutes(-1);

    [Fact]
    public void Cancel_FutureSession_IsCancelled()
    {
        var session = NewSession();

        var result = session.Cancel(BeforeStart);

        Assert.True(result.IsSuccess);
        Assert.Equal(ClassSessionStatus.Cancelled, session.Status);
    }

    [Fact]
    public void Cancel_Twice_ReturnsSessionAlreadyCancelled()
    {
        var session = NewSession();
        Assert.True(session.Cancel(BeforeStart).IsSuccess);

        var result = session.Cancel(BeforeStart);

        AssertRuleFailure(result, "schedule.session_already_cancelled");
        Assert.Equal(ClassSessionStatus.Cancelled, session.Status);
    }

    [Fact]
    public void Cancel_AtTheStartInstant_ReturnsSessionInPastAndStaysScheduled()
    {
        var session = NewSession();

        var result = session.Cancel(StartsAt);

        AssertRuleFailure(result, "schedule.session_in_past");
        Assert.Equal(ClassSessionStatus.Scheduled, session.Status);
    }

    [Fact]
    public void ReassignTeacher_FutureScheduledSession_ChangesTheTeacher()
    {
        var session = NewSession();

        var result = session.ReassignTeacher(OtherTeacherId, BeforeStart);

        Assert.True(result.IsSuccess);
        Assert.Equal(OtherTeacherId, session.TeacherId);
    }

    [Fact]
    public void ReassignTeacher_StartedSession_SucceedsAndKeepsTheTeacher()
    {
        var session = NewSession();

        var result = session.ReassignTeacher(OtherTeacherId, StartsAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherId, session.TeacherId);
    }

    [Fact]
    public void ReassignTeacher_CancelledFutureSession_SucceedsAndKeepsTheTeacher()
    {
        var session = NewSession();
        Assert.True(session.Cancel(BeforeStart).IsSuccess);

        var result = session.ReassignTeacher(OtherTeacherId, BeforeStart);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherId, session.TeacherId);
    }

    private static ClassSession NewSession()
    {
        var rule = ScheduleRule.Create(CentreId, GroupId, DayOfWeek.Saturday, new TimeOnly(18, 0), 90, "Room 2", new DateOnly(2026, 12, 5)).Value;
        var date = new DateOnly(2026, 12, 12);
        var session = Assert.Single(SessionGenerator.Generate(rule, TeacherId, Cairo, date, date));
        Assert.Equal(StartsAt, session.StartsAt);
        Assert.Equal("Room 2", session.Room);
        return session;
    }

    private static void AssertRuleFailure(Result result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(expectedCode, result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }
}
