using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class UpdateTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeTeacherRepository();
        var handler = new UpdateTeacherHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateTeacherCommand(Guid.CreateVersion7(), "Amina Hassan", null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_Valid_UpdatesNameAndPhone()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new UpdateTeacherHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateTeacherCommand(teacher.Id, "Amina Youssef", "+201012345678", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Amina Youssef", teacher.FullName);
        Assert.Equal("+201012345678", teacher.Phone!.Value);
    }

    [Fact]
    public async Task HandleAsync_ArchivedTeacher_ReturnsArchivedReadOnly()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        teacher.Archive();
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new UpdateTeacherHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateTeacherCommand(teacher.Id, "Amina Youssef", null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.archived_read_only", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidOnPhoneFieldAndLeavesTeacherUnchanged()
    {
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var repository = new FakeTeacherRepository();
        repository.Existing.Add(teacher);
        var handler = new UpdateTeacherHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateTeacherCommand(teacher.Id, "Amina Hassan", "not-a-phone", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.Validation, result.Error!.Kind);
        Assert.Contains("phone", result.Error.Fields!.Keys);
        Assert.Null(teacher.Phone);
    }
}
