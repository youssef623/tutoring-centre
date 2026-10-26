using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class CreateStaffHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_NewEmail_AddsMembershipInTheActorsCentreAndReturnsATemporaryPassword()
    {
        var actorContext = ActorIn(NileCentreId);
        var accountService = new FakeStaffAccountService();
        var memberships = new FakeMembershipRepository();
        var handler = new CreateStaffHandler(actorContext, accountService, memberships);

        var result = await handler.HandleAsync(new CreateStaffCommand("new@nile.test", "New Teacher", StaffRole.Teacher, "en"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.TemporaryPassword);
        Assert.Single(memberships.Added);
        Assert.Equal(NileCentreId, memberships.Added[0].CentreId);
        Assert.Equal(StaffRole.Teacher, memberships.Added[0].Role);
        Assert.Equal(result.Value.UserId, memberships.Added[0].UserId);
    }

    [Fact]
    public async Task HandleAsync_ExistingAccount_AddsMembershipWithNoTemporaryPassword()
    {
        var actorContext = ActorIn(NileCentreId);
        var accountService = new FakeStaffAccountService();
        var existingUserId = Guid.CreateVersion7();
        accountService.ExistingAccounts["existing@nile.test"] = existingUserId;
        var memberships = new FakeMembershipRepository();
        var handler = new CreateStaffHandler(actorContext, accountService, memberships);

        var result = await handler.HandleAsync(new CreateStaffCommand("existing@nile.test", "Existing Person", StaffRole.Secretary, "ar"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.TemporaryPassword);
        Assert.Equal(existingUserId, result.Value.UserId);
        Assert.Single(memberships.Added);
    }

    [Fact]
    public async Task HandleAsync_AlreadyAnActiveMemberOfThisCentre_ReturnsAlreadyMemberAndAddsNothing()
    {
        var actorContext = ActorIn(NileCentreId);
        var accountService = new FakeStaffAccountService();
        var existingUserId = Guid.CreateVersion7();
        accountService.ExistingAccounts["already@nile.test"] = existingUserId;
        var memberships = new FakeMembershipRepository();
        memberships.Memberships.Add(Membership.Create(existingUserId, NileCentreId, StaffRole.Teacher).Value);
        var handler = new CreateStaffHandler(actorContext, accountService, memberships);

        var result = await handler.HandleAsync(new CreateStaffCommand("already@nile.test", "Already Member", StaffRole.Secretary, "en"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.already_member", result.Error!.Code);
        Assert.Contains("email", result.Error.Fields!.Keys);
        Assert.Empty(memberships.Added);
    }

    [Fact]
    public async Task HandleAsync_AlreadyAnInactiveMemberOfThisCentre_ReturnsAlreadyMemberAndAddsNothing()
    {
        var actorContext = ActorIn(NileCentreId);
        var accountService = new FakeStaffAccountService();
        var existingUserId = Guid.CreateVersion7();
        accountService.ExistingAccounts["inactive-already@nile.test"] = existingUserId;
        var memberships = new FakeMembershipRepository();
        var inactiveMembership = Membership.Create(existingUserId, NileCentreId, StaffRole.Teacher).Value;
        inactiveMembership.Deactivate();
        memberships.Memberships.Add(inactiveMembership);
        var handler = new CreateStaffHandler(actorContext, accountService, memberships);

        var result = await handler.HandleAsync(new CreateStaffCommand("inactive-already@nile.test", "Inactive Already", StaffRole.Secretary, "en"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.already_member", result.Error!.Code);
        Assert.Empty(memberships.Added);
    }

    private static CurrentActorContext ActorIn(Guid centreId)
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), centreId, StaffRole.Owner));
        return actorContext;
    }
}
