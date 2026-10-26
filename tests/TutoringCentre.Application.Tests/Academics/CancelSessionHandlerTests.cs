using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CancelSessionHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownSession_ReturnsSessionNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new CancelSessionCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.session_not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_FutureSession_IsCancelled()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new CancelSessionCommand(sut.Future.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(ClassSessionStatus.Cancelled, sut.Future.Status);
    }

    [Fact]
    public async Task HandleAsync_PastSession_ReturnsSessionInPastAndStaysScheduled()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new CancelSessionCommand(sut.Past.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.session_in_past", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(ClassSessionStatus.Scheduled, sut.Past.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyCancelledSession_ReturnsSessionAlreadyCancelled()
    {
        var sut = CreateSut();
        sut.Future.Cancel(Now);

        var result = await sut.Handler.HandleAsync(new CancelSessionCommand(sut.Future.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("schedule.session_already_cancelled", result.Error!.Code);
    }

    private static Sut CreateSut()
    {
        var rule = SaturdayRule(Guid.CreateVersion7(), Today.AddDays(-14));
        var generated = Sessions(rule, Guid.CreateVersion7(), Today.AddDays(-14), Today.AddDays(13));
        var sessions = new FakeClassSessionRepository();
        sessions.Existing.AddRange(generated);

        var past = generated.First(session => session.StartsAt < Now);
        var future = generated.First(session => session.StartsAt > Now);
        return new Sut(new CancelSessionHandler(Clock(), sessions), past, future);
    }

    private sealed record Sut(CancelSessionHandler Handler, ClassSession Past, ClassSession Future);
}
