using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class WithdrawEnrollmentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownEnrollment_ReturnsEnrollmentNotFound()
    {
        var handler = new WithdrawEnrollmentHandler(new FakeClock(), new FakeEnrollmentRepository());

        var result = await handler.HandleAsync(new WithdrawEnrollmentCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_AlreadyEnded_ReturnsAlreadyEndedAndKeepsTheFirstEnd()
    {
        var clock = new FakeClock();
        var enrollment = Enrollment.Start(NileCentreId, Guid.CreateVersion7(), Guid.CreateVersion7(), clock.UtcNow.AddDays(-2)).Value;
        var firstEnd = clock.UtcNow.AddDays(-1);
        enrollment.End(firstEnd, EnrollmentEndReason.Transferred);
        var repository = new FakeEnrollmentRepository();
        repository.Existing.Add(enrollment);
        var handler = new WithdrawEnrollmentHandler(clock, repository);

        var result = await handler.HandleAsync(new WithdrawEnrollmentCommand(enrollment.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.already_ended", result.Error!.Code);
        Assert.Equal(firstEnd, enrollment.EndedAt);
        Assert.Equal(EnrollmentEndReason.Transferred, enrollment.EndReason);
    }

    [Fact]
    public async Task HandleAsync_ActiveEnrollment_EndsItAsWithdrawnAtTheClocksTime()
    {
        var clock = new FakeClock();
        var enrollment = Enrollment.Start(NileCentreId, Guid.CreateVersion7(), Guid.CreateVersion7(), clock.UtcNow.AddDays(-1)).Value;
        var repository = new FakeEnrollmentRepository();
        repository.Existing.Add(enrollment);
        var handler = new WithdrawEnrollmentHandler(clock, repository);

        var result = await handler.HandleAsync(new WithdrawEnrollmentCommand(enrollment.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(EnrollmentStatus.Ended, enrollment.Status);
        Assert.Equal(clock.UtcNow, enrollment.EndedAt);
        Assert.Equal(EnrollmentEndReason.Withdrawn, enrollment.EndReason);
        Assert.Empty(repository.Added);
    }
}
