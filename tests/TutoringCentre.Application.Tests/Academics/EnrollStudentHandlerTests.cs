using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Branching of the enroll handler against fakes: the order of checks and the code each one answers. The fake
/// group repository reports a configurable active count and takes no lock; the lock itself is proven against a
/// real database (LastSeatConcurrencyTests).
/// </summary>
public sealed class EnrollStudentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFound()
    {
        var sut = new Sut();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(Guid.CreateVersion7(), sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "group.not_found", ErrorKind.NotFound, sut);
    }

    [Fact]
    public async Task HandleAsync_UnknownGroupAndUnknownStudent_ReturnsGroupNotFoundBeforeAnythingAboutTheStudent()
    {
        var sut = new Sut();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(Guid.CreateVersion7(), Guid.CreateVersion7()), CancellationToken.None);

        AssertFailure(result, "group.not_found", ErrorKind.NotFound, sut);
    }

    [Fact]
    public async Task HandleAsync_UnknownStudent_ReturnsStudentNotFound()
    {
        var sut = new Sut();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, Guid.CreateVersion7()), CancellationToken.None);

        AssertFailure(result, "student.not_found", ErrorKind.NotFound, sut);
    }

    [Fact]
    public async Task HandleAsync_ArchivedStudent_ReturnsStudentArchived()
    {
        var sut = new Sut();
        sut.Student.Archive();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "enrollment.student_archived", ErrorKind.Rule, sut);
    }

    [Fact]
    public async Task HandleAsync_AlreadyEnrolled_ReturnsAlreadyEnrolled()
    {
        var sut = new Sut();
        sut.Enrollments.Existing.Add(Enrollment.Start(NileCentreId, sut.Student.Id, sut.Group.Id, sut.Clock.UtcNow.AddDays(-1)).Value);

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "enrollment.already_enrolled", ErrorKind.Conflict, sut);
    }

    [Fact]
    public async Task HandleAsync_CountEqualToCapacity_ReturnsGroupFull()
    {
        var sut = new Sut();
        sut.Groups.ActiveEnrollmentCounts[sut.Group.Id] = sut.Group.Capacity;

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "group.full", ErrorKind.Conflict, sut);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsGroupArchived()
    {
        var sut = new Sut();
        sut.Group.Archive();

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        AssertFailure(result, "group.archived", ErrorKind.Rule, sut);
    }

    [Fact]
    public async Task HandleAsync_LastSeat_AddsOneActiveEnrollmentInTheActorsCentreAtTheClocksTime()
    {
        var sut = new Sut();
        sut.Groups.ActiveEnrollmentCounts[sut.Group.Id] = sut.Group.Capacity - 1;

        var result = await sut.Handler.HandleAsync(new EnrollStudentCommand(sut.Group.Id, sut.Student.Id), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        var added = Assert.Single(sut.Enrollments.Added);
        Assert.Equal(result.Value.EnrollmentId, added.Id);
        Assert.Equal(added.Id, Assert.IsType<TutoringCentre.Application.Academics.Contracts.EnrollmentStarted>(Assert.Single(sut.Outbox.Messages)).EnrollmentId);
        Assert.Equal(NileCentreId, added.CentreId);
        Assert.Equal(sut.Student.Id, added.StudentId);
        Assert.Equal(sut.Group.Id, added.GroupId);
        Assert.Equal(EnrollmentStatus.Active, added.Status);
        Assert.Equal(sut.Clock.UtcNow, added.EnrolledAt);
    }

    private static void AssertFailure(Result<EnrollStudentResult> result, string code, ErrorKind kind, Sut sut)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(kind, result.Error.Kind);
        Assert.Empty(sut.Enrollments.Added);
        Assert.Empty(sut.Outbox.Messages);
    }

    private sealed class Sut
    {
        public Sut()
        {
            Group = Group.Create(NileCentreId, "Grade 10 A", Guid.CreateVersion7(), Guid.CreateVersion7(), 20).Value;
            Student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
            Groups.Existing.Add(Group);
            Students.Existing.Add(Student);
            var actor = new FakeCurrentActor { Actor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Secretary) };
            Handler = new EnrollStudentHandler(actor, Clock, Groups, Students, Enrollments, Outbox);
        }

        public FakeOutbox Outbox { get; } = new();

        public FakeClock Clock { get; } = new();

        public FakeGroupRepository Groups { get; } = new();

        public FakeStudentRepository Students { get; } = new();

        public FakeEnrollmentRepository Enrollments { get; } = new();

        public Group Group { get; }

        public Student Student { get; }

        public EnrollStudentHandler Handler { get; }
    }
}
