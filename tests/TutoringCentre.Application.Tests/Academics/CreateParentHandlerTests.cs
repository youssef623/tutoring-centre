using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateParentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_Valid_AddsParentWithActorsCentreAndCanonicalPhone()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeParentRepository();
        var handler = new CreateParentHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateParentCommand("Omar Hassan", "01012345678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Equal(NileCentreId, repository.Added[0].CentreId);
        Assert.Equal("+201012345678", repository.Added[0].Phone.Value);
        Assert.Equal(repository.Added[0].Id, result.Value.ParentId);
    }

    [Fact]
    public async Task HandleAsync_DifferentlySpelledPhoneAlreadyExists_ReturnsPhoneTakenAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeParentRepository();
        repository.Existing.Add(Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("+201012345678").Value).Value);
        var handler = new CreateParentHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateParentCommand("Another Parent", "0101 234 5678"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.phone_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("phone", result.Error.Fields!.Keys);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithFieldAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeParentRepository();
        var handler = new CreateParentHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateParentCommand("Omar Hassan", "12345"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("phone.invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        Assert.Contains("phone", result.Error.Fields!.Keys);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_OneCharacterName_ReturnsNameInvalidAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeParentRepository();
        var handler = new CreateParentHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateParentCommand("O", "01012345678"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.name_invalid", result.Error!.Code);
        Assert.Empty(repository.Added);
    }
}
