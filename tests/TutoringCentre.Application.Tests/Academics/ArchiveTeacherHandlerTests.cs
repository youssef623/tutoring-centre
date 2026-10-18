using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeTeacherRepository();
        var handler = new ArchiveTeacherHandler(repository);

        var result = await handler.HandleAsync(new ArchiveTeacherCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveTeacher_ArchivesAndKeepsLink()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var userId = Guid.CreateVersion7();
        teacher.LinkUser(userId);
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new ArchiveTeacherHandler(repository);

        var result = await handler.HandleAsync(new ArchiveTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherStatus.Archived, teacher.Status);
        Assert.Equal(userId, teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        teacher.Archive();
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new ArchiveTeacherHandler(repository);

        var result = await handler.HandleAsync(new ArchiveTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }
}
