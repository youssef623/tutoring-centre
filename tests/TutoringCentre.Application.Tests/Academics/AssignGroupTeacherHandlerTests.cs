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

        return new Sut(new AssignGroupTeacherHandler(groups, teachers), group, currentTeacher, otherTeacher, archivedTeacher);
    }

    private sealed record Sut(
        AssignGroupTeacherHandler Handler,
        Group Group,
        Teacher CurrentTeacher,
        Teacher OtherTeacher,
        Teacher ArchivedTeacher);
}
