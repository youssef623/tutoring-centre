using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RestoreTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeTeacherRepository();
        var handler = new RestoreTeacherHandler(repository);

        var result = await handler.HandleAsync(new RestoreTeacherCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ArchivedTeacher_Restores()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        teacher.Archive();
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new RestoreTeacherHandler(repository);

        var result = await handler.HandleAsync(new RestoreTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherStatus.Active, teacher.Status);
    }

    [Fact]
    public async Task HandleAsync_NotArchived_ReturnsNotArchived()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new RestoreTeacherHandler(repository);

        var result = await handler.HandleAsync(new RestoreTeacherCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }
}
