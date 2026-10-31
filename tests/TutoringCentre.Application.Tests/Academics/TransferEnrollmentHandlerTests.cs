using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Branching of the transfer handler against fakes. Every failure asserts both effects are absent together —
/// the old enrollment still active and untouched, and no new enrollment added — not each one on its own.
/// </summary>
public sealed class TransferEnrollmentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownEnrollment_ReturnsEnrollmentNotFound()
    {
        var sut = new Sut();

        var result = await sut.TransferAsync(Guid.CreateVersion7(), sut.Target.Id);

        sut.AssertNothingChanged(result, "enrollment.not_found", ErrorKind.NotFound);
    }

    [Fact]
    public async Task HandleAsync_EndedEnrollment_ReturnsAlreadyEnded()
    {
        var sut = new Sut();
        sut.Current.End(sut.Clock.UtcNow.AddHours(-1), EnrollmentEndReason.Withdrawn);

        var result = await sut.TransferAsync(sut.Current.Id, sut.Target.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.already_ended", result.Error!.Code);
        Assert.Equal(EnrollmentEndReason.Withdrawn, sut.Current.EndReason);
        Assert.Empty(sut.Enrollments.Added);
    }

    [Fact]
    public async Task HandleAsync_SameGroup_ReturnsSameGroup()
    {
        var sut = new Sut();

        var result = await sut.TransferAsync(sut.Current.Id, sut.Source.Id);

        sut.AssertNothingChanged(result, "enrollment.same_group", ErrorKind.Rule);
    }

    [Fact]
    public async Task HandleAsync_UnknownTarget_ReturnsGroupNotFound()
    {
        var sut = new Sut();

        var result = await sut.TransferAsync(sut.Current.Id, Guid.CreateVersion7());

        sut.AssertNothingChanged(result, "group.not_found", ErrorKind.NotFound);
    }

    [Fact]
    public async Task HandleAsync_TargetOfAnotherSubject_ReturnsSubjectMismatch()
    {
        var sut = new Sut();
        var otherSubject = Group.Create(NileCentreId, "Physics 10", Guid.CreateVersion7(), sut.Source.TeacherId, 20).Value;
        sut.Groups.Existing.Add(otherSubject);

        var result = await sut.TransferAsync(sut.Current.Id, otherSubject.Id);

        sut.AssertNothingChanged(result, "enrollment.subject_mismatch", ErrorKind.Rule);
    }

    [Fact]
    public async Task HandleAsync_StudentAlreadyInTarget_ReturnsAlreadyEnrolled()
    {
        var sut = new Sut();
        sut.Enrollments.Existing.Add(Enrollment.Start(NileCentreId, sut.Current.StudentId, sut.Target.Id, sut.Clock.UtcNow.AddDays(-1)).Value);

        var result = await sut.TransferAsync(sut.Current.Id, sut.Target.Id);

        sut.AssertNothingChanged(result, "enrollment.already_enrolled", ErrorKind.Conflict);
    }

    [Fact]
    public async Task HandleAsync_TargetFull_ReturnsGroupFullAndChangesNothing()
    {
        var sut = new Sut();
        sut.Groups.ActiveEnrollmentCounts[sut.Target.Id] = sut.Target.Capacity;

        var result = await sut.TransferAsync(sut.Current.Id, sut.Target.Id);

        sut.AssertNothingChanged(result, "group.full", ErrorKind.Conflict);
    }

    [Fact]
    public async Task HandleAsync_Success_EndsTheOldAsTransferredAndAddsANewActiveEnrollmentInTheTarget()
    {
        var sut = new Sut();

        var result = await sut.TransferAsync(sut.Current.Id, sut.Target.Id);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(EnrollmentStatus.Ended, sut.Current.Status);
        Assert.Equal(EnrollmentEndReason.Transferred, sut.Current.EndReason);
        Assert.Equal(sut.Clock.UtcNow, sut.Current.EndedAt);
        Assert.Equal(sut.Source.Id, sut.Current.GroupId);
        var added = Assert.Single(sut.Enrollments.Added);
        Assert.Equal(result.Value.EnrollmentId, added.Id);
        Assert.Equal(sut.Target.Id, added.GroupId);
        Assert.Equal(sut.Current.StudentId, added.StudentId);
        Assert.Equal(NileCentreId, added.CentreId);
        Assert.Equal(EnrollmentStatus.Active, added.Status);
        Assert.Equal(sut.Clock.UtcNow, added.EnrolledAt);
        Assert.Equal(added.Id, Assert.IsType<TutoringCentre.Application.Academics.Contracts.EnrollmentStarted>(Assert.Single(sut.Outbox.Messages)).EnrollmentId);
    }

    private sealed class Sut
    {
        public Sut()
        {
            var subjectId = Guid.CreateVersion7();
            var teacherId = Guid.CreateVersion7();
            Source = Group.Create(NileCentreId, "Grade 10 A", subjectId, teacherId, 20).Value;
            Target = Group.Create(NileCentreId, "Grade 10 B", subjectId, teacherId, 20).Value;
            Groups.Existing.Add(Source);
            Groups.Existing.Add(Target);
            Current = Enrollment.Start(NileCentreId, Guid.CreateVersion7(), Source.Id, Clock.UtcNow.AddDays(-7)).Value;
            Enrollments.Existing.Add(Current);
            var actor = new FakeCurrentActor { Actor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Secretary) };
            Handler = new TransferEnrollmentHandler(actor, Clock, Groups, Enrollments, Outbox);
        }

        public FakeOutbox Outbox { get; } = new();

        public FakeClock Clock { get; } = new();

        public FakeGroupRepository Groups { get; } = new();

        public FakeEnrollmentRepository Enrollments { get; } = new();

        public Group Source { get; }

        public Group Target { get; }

        public Enrollment Current { get; }

        public TransferEnrollmentHandler Handler { get; }

        public Task<Result<TransferEnrollmentResult>> TransferAsync(Guid enrollmentId, Guid targetGroupId) =>
            Handler.HandleAsync(new TransferEnrollmentCommand(enrollmentId, targetGroupId, 0), CancellationToken.None);

        /// <summary>Neither effect: the old enrollment is still active in its group, and nothing new was added.</summary>
        public void AssertNothingChanged(Result<TransferEnrollmentResult> result, string code, ErrorKind kind)
        {
            Assert.True(result.IsFailure);
            Assert.Equal(code, result.Error!.Code);
            Assert.Equal(kind, result.Error.Kind);
            Assert.True(
                Current.Status == EnrollmentStatus.Active
                    && Current.EndedAt is null
                    && Current.EndReason is null
                    && Current.GroupId == Source.Id
                    && Enrollments.Added.Count == 0,
                "A failed transfer must leave the old enrollment active and add no new one.");
        }
    }
}
