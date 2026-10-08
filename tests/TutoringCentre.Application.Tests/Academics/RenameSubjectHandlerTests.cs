using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RenameSubjectHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeSubjectRepository();
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(
            new RenameSubjectCommand(Guid.CreateVersion7(), "Applied Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_NewNameAlreadyTakenByAnotherSubject_ReturnsNameTaken()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var other = Subject.Create(NileCentreId, "Chemistry").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        repository.Existing.Add(other);
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(new RenameSubjectCommand(subject.Id, "Chemistry", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_SameName_SucceedsWithNoChange()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(new RenameSubjectCommand(subject.Id, "Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Mathematics", subject.Name);
    }

    [Fact]
    public async Task HandleAsync_ArchivedSubject_ReturnsArchivedReadOnly()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        subject.Archive();
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(new RenameSubjectCommand(subject.Id, "Applied Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.archived_read_only", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_ValidNewName_Succeeds()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new RenameSubjectHandler(repository);

        var result = await handler.HandleAsync(new RenameSubjectCommand(subject.Id, "Applied Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Applied Mathematics", subject.Name);
    }
}
