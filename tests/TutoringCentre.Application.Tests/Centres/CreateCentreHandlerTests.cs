using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class CreateCentreHandlerTests
{
    [Fact]
    public async Task HandleAsync_AnonymousActor_ReturnsForbiddenAndDoesNotAdd()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        var repository = new FakeCentreRepository();
        repository.Existing.Add(Centre.Create("Existing", "nile-centre", "Africa/Cairo", SupportedLocale.Ar).Value);
        var handler = new CreateCentreHandler(actorContext, repository);
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("centre.create_forbidden", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_SlugAlreadyExists_ReturnsConflictAndDoesNotAdd()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new SystemActor(null));
        var repository = new FakeCentreRepository();
        repository.Existing.Add(Centre.Create("Existing", "nile-centre", "Africa/Cairo", SupportedLocale.Ar).Value);
        var handler = new CreateCentreHandler(actorContext, repository);
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("centre.slug_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_InvalidSlug_ReturnsDomainErrorAndDoesNotAdd()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new SystemActor(null));
        var repository = new FakeCentreRepository();
        var handler = new CreateCentreHandler(actorContext, repository);
        var command = new CreateCentreCommand("Nile Tutoring Centre", "Bad Slug", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("centre.slug_invalid", result.Error!.Code);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_SystemActorValidInput_AddsCentreAndReturnsResult()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        actorContext.Set(new SystemActor(null));
        var repository = new FakeCentreRepository();
        var handler = new CreateCentreHandler(actorContext, repository);
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Equal("Nile Tutoring Centre", repository.Added[0].Name);
        Assert.Equal("nile-centre", repository.Added[0].Slug);
        Assert.Equal("nile-centre", result.Value.Slug);
        Assert.Equal(repository.Added[0].Id, result.Value.CentreId);
    }
}
