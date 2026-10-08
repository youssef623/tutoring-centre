using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateSubjectHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_Valid_AddsSubjectWithActorsCentreAndReturnsId()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeSubjectRepository();
        var handler = new CreateSubjectHandler(actorContext, repository);

        // Act
        var result = await handler.HandleAsync(new CreateSubjectCommand("Mathematics"), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Equal(NileCentreId, repository.Added[0].CentreId);
        Assert.Equal("Mathematics", repository.Added[0].Name);
        Assert.Equal(repository.Added[0].Id, result.Value.SubjectId);
    }

    [Fact]
    public async Task HandleAsync_DuplicateDifferingByCaseAndSpaces_ReturnsNameTakenAndAddsNothing()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(Subject.Create(NileCentreId, "Mathematics").Value);
        var handler = new CreateSubjectHandler(actorContext, repository);

        // Act
        var result = await handler.HandleAsync(new CreateSubjectCommand("  mathematics "), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("name", result.Error.Fields!.Keys);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndAddsNothing()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeSubjectRepository();
        var handler = new CreateSubjectHandler(actorContext, repository);

        // Act
        var result = await handler.HandleAsync(new CreateSubjectCommand("   "), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_invalid", result.Error!.Code);
        Assert.Empty(repository.Added);
    }
}
