using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RestoreGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_SubjectArchived_ReturnsSubjectArchivedAndStaysArchived()
    {
        var sut = CreateSut();
        sut.Subject.Archive();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(sut.Group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.subject_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(GroupStatus.Archived, sut.Group.Status);
    }

    [Fact]
    public async Task HandleAsync_TeacherArchived_ReturnsTeacherArchivedAndStaysArchived()
    {
        var sut = CreateSut();
        sut.Teacher.Archive();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(sut.Group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.teacher_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(GroupStatus.Archived, sut.Group.Status);
    }

    [Fact]
    public async Task HandleAsync_BothReferencesActive_Restores()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(sut.Group.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(GroupStatus.Active, sut.Group.Status);
    }

    [Fact]
    public async Task HandleAsync_NotArchived_ReturnsNotArchived()
    {
        var sut = CreateSut();
        sut.Group.Restore();

        var result = await sut.Handler.HandleAsync(new RestoreGroupCommand(sut.Group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    private static Sut CreateSut()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var subjects = new FakeSubjectRepository();
        subjects.Existing.Add(subject);

        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var teachers = new FakeTeacherRepository();
        teachers.Existing.Add(teacher);

        var group = Group.Create(NileCentreId, "Grade 10 A", subject.Id, teacher.Id, 20).Value;
        group.Archive();
        var groups = new FakeGroupRepository();
        groups.Existing.Add(group);

        return new Sut(new RestoreGroupHandler(groups, subjects, teachers), group, subject, teacher);
    }

    private sealed record Sut(RestoreGroupHandler Handler, Group Group, Subject Subject, Teacher Teacher);
}
