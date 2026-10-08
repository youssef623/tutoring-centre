using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RestoreSubjectHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeSubjectRepository();
        var handler = new RestoreSubjectHandler(repository);

        var result = await handler.HandleAsync(new RestoreSubjectCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveSubject_ReturnsNotArchived()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new RestoreSubjectHandler(repository);

        var result = await handler.HandleAsync(new RestoreSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_archived", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_ArchivedSubject_BecomesActive()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        subject.Archive();
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new RestoreSubjectHandler(repository);

        var result = await handler.HandleAsync(new RestoreSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SubjectStatus.Active, subject.Status);
    }
}
