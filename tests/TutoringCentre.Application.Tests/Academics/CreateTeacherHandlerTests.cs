using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateTeacherHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_Valid_AddsTeacherWithActorsCentreAndReturnsId()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var handler = new CreateTeacherHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateTeacherCommand("Amina Hassan", "+201012345678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Equal(NileCentreId, repository.Added[0].CentreId);
        Assert.Equal("Amina Hassan", repository.Added[0].FullName);
        Assert.Equal(repository.Added[0].Id, result.Value.TeacherId);
    }

    [Fact]
    public async Task HandleAsync_TwoTeachersSharingAName_BothSucceed()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var handler = new CreateTeacherHandler(actorContext, repository);

        var first = await handler.HandleAsync(new CreateTeacherCommand("Amina Hassan", null), CancellationToken.None);
        var second = await handler.HandleAsync(new CreateTeacherCommand("Amina Hassan", null), CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(2, repository.Added.Count);
    }

    [Fact]
    public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var handler = new CreateTeacherHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateTeacherCommand("   ", null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.name_invalid", result.Error!.Code);
        Assert.Empty(repository.Added);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidOnPhoneFieldAndAddsNothing()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var handler = new CreateTeacherHandler(actorContext, repository);

        var result = await handler.HandleAsync(new CreateTeacherCommand("Amina Hassan", "not-a-phone"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.Validation, result.Error!.Kind);
        Assert.Contains("phone", result.Error.Fields!.Keys);
        Assert.Empty(repository.Added);
    }
}
