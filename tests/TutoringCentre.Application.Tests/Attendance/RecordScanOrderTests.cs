using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.RecordScan;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class RecordScanOrderTests
{
    [Theory]
    [InlineData("anonymous", "auth.not_authenticated")]
    [InlineData("tenant", "tenant.not_selected")]
    [InlineData("permission", "auth.permission_denied")]
    public async Task ActorCheck_PrecedesSessionAndTransaction(string failure, string code)
    {
        var actor = new CurrentActorContext();
        actor.Set(failure switch
        {
            "anonymous" => new AnonymousActor(),
            "tenant" => new StaffActor(Guid.NewGuid(), null, null),
            _ => new StaffActor(Guid.NewGuid(), Guid.NewGuid(), null),
        });
        var unit = new FakeUnitOfWork();
        using var provider = new ServiceCollection()
            .AddSingleton<ICurrentActor>(actor)
            .AddSingleton<IValidator<RecordScanCommand>>(new RecordScanValidator())
            .BuildServiceProvider();
        var result = await new Dispatcher(provider, unit, NullLogger<Dispatcher>.Instance)
            .SendAsync<RecordScanCommand, ScanResult>(new(Guid.NewGuid(), CardToken.Generate().Reveal()), CancellationToken.None);
        Assert.Equal(code, result.Error!.Code);
        Assert.Empty(unit.Calls);
    }

    [Fact]
    public async Task TeachingScope_BeatsClosedWindow()
    {
        var f = new ScanFixture();
        f.Attendance.Clock.UtcNow = f.Attendance.OtherSession.EndsAt.AddDays(1);
        Assert.Equal("attendance.session_not_found", (await f.Scan(f.Attendance.OtherSession.SessionId)).Error!.Code);
        Assert.Equal(0, f.Cards.Reads);
    }

    [Fact]
    public async Task Cancellation_BeatsNotOpenWindowAndUnknownCard()
    {
        var f = new ScanFixture();
        var a = f.Attendance;
        a.Roster.Sessions[a.Session.SessionId] = a.Session with { IsCancelled = true };
        a.Clock.UtcNow = a.Session.StartsAt.AddDays(-1);
        f.Cards.Items.Clear();
        Assert.Equal("attendance.session_cancelled", (await f.Scan()).Error!.Code);
        Assert.Equal(0, f.Cards.Reads);
    }

    [Theory]
    [InlineData("missing", "attendance.session_not_found")]
    [InlineData("scope", "attendance.session_not_found")]
    [InlineData("cancelled", "attendance.session_cancelled")]
    [InlineData("early", "attendance.session_not_open")]
    [InlineData("closed", "attendance.window_closed")]
    [InlineData("unknown", "card.not_recognised")]
    [InlineData("revoked", "card.revoked")]
    [InlineData("unexpected", "attendance.student_not_in_session")]
    public async Task SingleFailure_ReturnsContractCode(string failure, string code)
    {
        var f = new ScanFixture();
        var a = f.Attendance;
        switch (failure)
        {
            case "missing": a.Roster.Sessions.Clear(); break;
            case "scope": a.Scope.Scope = TutoringCentre.Application.Academics.Contracts.TeachingScope.None; break;
            case "cancelled": a.Roster.Sessions[a.Session.SessionId] = a.Session with { IsCancelled = true }; break;
            case "early": a.Clock.UtcNow = a.Session.StartsAt.AddHours(-1); break;
            case "closed": a.Clock.UtcNow = a.Session.EndsAt.AddDays(1); break;
            case "unknown": f.Cards.Items.Clear(); break;
            case "revoked": f.Revoke(); break;
            case "unexpected": a.Roster.Students.Clear(); break;
        }
        Assert.Equal(code, (await f.Scan()).Error!.Code);
        Assert.Empty(a.Records.Added);
        Assert.Equal(0, a.Records.SessionReads);
        if (failure is "missing" or "scope" or "cancelled" or "early" or "closed") Assert.Equal(0, f.Cards.Reads);
        if (failure is not "unexpected") Assert.DoesNotContain("expected", a.Roster.Calls);
        if (failure == "missing") Assert.Equal(0, a.Scope.Calls);
    }

    [Fact]
    public async Task ClosedWindow_BeatsUnknownCard()
    {
        var f = new ScanFixture();
        f.Attendance.Clock.UtcNow = f.Attendance.Session.EndsAt.AddDays(1);
        f.Cards.Items.Clear();
        Assert.Equal("attendance.window_closed", (await f.Scan()).Error!.Code);
        Assert.Equal(0, f.Cards.Reads);
    }

    [Fact]
    public async Task OtherTeacher_BeatsRevokedCard()
    {
        var f = new ScanFixture();
        f.Revoke();
        Assert.Equal("attendance.session_not_found", (await f.Scan(f.Attendance.OtherSession.SessionId)).Error!.Code);
        Assert.Equal(0, f.Cards.Reads);
    }

    [Fact]
    public async Task RevokedCard_BeatsUnexpectedStudent()
    {
        var f = new ScanFixture();
        f.Revoke();
        f.Attendance.Roster.Students.Clear();
        Assert.Equal("card.revoked", (await f.Scan()).Error!.Code);
        Assert.DoesNotContain("expected", f.Attendance.Roster.Calls);
    }

    [Fact]
    public async Task UnknownCard_BeatsUnexpectedStudent()
    {
        var f = new ScanFixture();
        f.Cards.Items.Clear();
        f.Attendance.Roster.Students.Clear();
        Assert.Equal("card.not_recognised", (await f.Scan()).Error!.Code);
        Assert.DoesNotContain("expected", f.Attendance.Roster.Calls);
    }

    [Fact]
    public async Task MissingSession_BeatsDeniedScope()
    {
        var f = new ScanFixture();
        f.Attendance.Roster.Sessions.Clear();
        f.Attendance.Scope.Scope = TutoringCentre.Application.Academics.Contracts.TeachingScope.None;
        Assert.Equal("attendance.session_not_found", (await f.Scan()).Error!.Code);
        Assert.Equal(0, f.Attendance.Scope.Calls);
    }

    [Fact]
    public async Task UnexpectedStudent_PreventsRecordRead()
    {
        var f = new ScanFixture();
        f.Attendance.Roster.Students.Clear();
        f.Existing(TutoringCentre.Domain.Attendance.AttendanceStatus.Present);
        Assert.Equal("attendance.student_not_in_session", (await f.Scan()).Error!.Code);
        Assert.Equal(0, f.Attendance.Records.SessionReads);
    }
}
