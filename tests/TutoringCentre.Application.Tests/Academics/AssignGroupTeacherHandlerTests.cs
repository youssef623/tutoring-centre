using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class AssignGroupTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(Guid.CreateVersion7(), sut.OtherTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_UnknownTeacher_ReturnsTeacherNotFoundAndKeepsTeacher()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(sut.CurrentTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_ArchivedTeacher_ReturnsTeacherArchivedAndKeepsTeacher()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, sut.ArchivedTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.teacher_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(sut.CurrentTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_ActiveTeacher_Reassigns()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, sut.OtherTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(sut.OtherTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_SameTeacher_SucceedsWithNoChange()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, sut.CurrentTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(sut.CurrentTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()
    {
        var sut = CreateSut();
        sut.Group.Archive();

        var result = await sut.Handler.HandleAsync(
            new AssignGroupTeacherCommand(sut.Group.Id, sut.OtherTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal(sut.CurrentTeacher.Id, sut.Group.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_ActiveTeacher_MovesFutureScheduledSessionsAndLeavesPastAndCancelledOnes()
    {
        var teachers = new FakeTeacherRepository();
        var currentTeacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var otherTeacher = Teacher.Create(NileCentreId, "Youssef Kamal", null).Value;
        teachers.Existing.AddRange([currentTeacher, otherTeacher]);
        var group = Group.Create(NileCentreId, "Grade 10 A", Guid.CreateVersion7(), currentTeacher.Id, 20).Value;
        var groups = new FakeGroupRepository();
        groups.Existing.Add(group);

        var rule = ScheduleTestData.SaturdayRule(group.Id, ScheduleTestData.Today.AddDays(-14));
        var generated = ScheduleTestData.Sessions(rule, currentTeacher.Id, ScheduleTestData.Today.AddDays(-14), ScheduleTestData.Today.AddDays(27));
        var sessions = new FakeClassSessionRepository();
        sessions.Existing.AddRange(generated);
        var cancelled = generated[^1];
        cancelled.Cancel(ScheduleTestData.Now);
        var handler = new AssignGroupTeacherHandler(ScheduleTestData.Clock(), groups, teachers, sessions);

        var result = await handler.HandleAsync(new AssignGroupTeacherCommand(group.Id, otherTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        var past = generated.Where(session => session.StartsAt <= ScheduleTestData.Now).ToList();
        var futureScheduled = generated.Where(session => session.StartsAt > ScheduleTestData.Now && session != cancelled).ToList();
        Assert.NotEmpty(past);
        Assert.NotEmpty(futureScheduled);
        Assert.All(past, session => Assert.Equal(currentTeacher.Id, session.TeacherId));
        Assert.All(futureScheduled, session => Assert.Equal(otherTeacher.Id, session.TeacherId));
        Assert.Equal(currentTeacher.Id, cancelled.TeacherId);
    }

    private static Sut CreateSut()
    {
        var teachers = new FakeTeacherRepository();
        var currentTeacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var otherTeacher = Teacher.Create(NileCentreId, "Youssef Kamal", null).Value;
        var archivedTeacher = Teacher.Create(NileCentreId, "Mona Said", null).Value;
        archivedTeacher.Archive();
        teachers.Existing.AddRange([currentTeacher, otherTeacher, archivedTeacher]);

        var group = Group.Create(NileCentreId, "Grade 10 A", Guid.CreateVersion7(), currentTeacher.Id, 20).Value;
        var groups = new FakeGroupRepository();
        groups.Existing.Add(group);

        return new Sut(new AssignGroupTeacherHandler(new FakeClock(), groups, teachers, new FakeClassSessionRepository()), group, currentTeacher, otherTeacher, archivedTeacher);
    }

    private sealed record Sut(
        AssignGroupTeacherHandler Handler,
        Group Group,
        Teacher CurrentTeacher,
        Teacher OtherTeacher,
        Teacher ArchivedTeacher);
}
