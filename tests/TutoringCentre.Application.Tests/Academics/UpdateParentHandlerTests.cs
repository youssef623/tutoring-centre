using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class UpdateParentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeParentRepository();
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateParentCommand(Guid.CreateVersion7(), "Omar Hassan", "01012345678", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_SamePhone_SucceedsWithoutRunningAUniquenessQuery()
    {
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var repository = new FakeParentRepository();
        repository.Existing.Add(parent);
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateParentCommand(parent.Id, "Omar Hassan Ali", "01012345678", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Omar Hassan Ali", parent.FullName);
        Assert.Equal(0, repository.PhoneExistsCallCount);
    }

    [Fact]
    public async Task HandleAsync_PhoneChangedAndTakenByAnother_ReturnsPhoneTaken()
    {
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var other = Parent.Create(NileCentreId, "Amal Fathy", PhoneNumber.Create("01212345678").Value).Value;
        var repository = new FakeParentRepository();
        repository.Existing.Add(parent);
        repository.Existing.Add(other);
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateParentCommand(parent.Id, "Omar Hassan", "01212345678", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.phone_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Equal(1, repository.PhoneExistsCallCount);
    }

    [Fact]
    public async Task HandleAsync_PhoneChangedToAFreeOne_Succeeds()
    {
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var repository = new FakeParentRepository();
        repository.Existing.Add(parent);
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateParentCommand(parent.Id, "Omar Hassan", "01112345678", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("+201112345678", parent.Phone.Value);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()
    {
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var repository = new FakeParentRepository();
        repository.Existing.Add(parent);
        var handler = new UpdateParentHandler(repository);

        var result = await handler.HandleAsync(new UpdateParentCommand(parent.Id, "Omar Hassan", "12345", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("phone.invalid", result.Error!.Code);
        Assert.Contains("phone", result.Error.Fields!.Keys);
    }
}
