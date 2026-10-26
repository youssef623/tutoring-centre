using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class ChangeStaffRoleHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();
    private static readonly Guid MaadiCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(Guid.CreateVersion7(), StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_MembershipBelongsToAnotherCentre_ReturnsNotFound()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var maadiMembership = Membership.Create(Guid.CreateVersion7(), MaadiCentreId, StaffRole.Teacher).Value;
        memberships.Memberships.Add(maadiMembership);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(maadiMembership.Id, StaffRole.Secretary, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_TargetIsTheActorsOwnMembership_ReturnsCannotChangeSelf()
    {
        var actorContext = OwnerIn(NileCentreId, out var ownerId);
        var memberships = new FakeMembershipRepository();
        var ownMembership = Membership.Create(ownerId, NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(ownMembership);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(ownMembership.Id, StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.cannot_change_self", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_DemotingTheOnlyActiveOwner_ReturnsLastOwner()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var onlyOwner = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(onlyOwner);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(onlyOwner.Id, StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.last_owner", result.Error!.Code);
        Assert.Equal(StaffRole.Owner, onlyOwner.Role);
    }

    [Fact]
    public async Task HandleAsync_DemotingOneOfTwoActiveOwners_Succeeds()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var firstOwner = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner).Value;
        var secondOwner = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(firstOwner);
        memberships.Memberships.Add(secondOwner);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(firstOwner.Id, StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(StaffRole.Teacher, firstOwner.Role);
    }

    [Fact]
    public async Task HandleAsync_SameRole_ReturnsRoleUnchanged()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        memberships.Memberships.Add(teacher);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(teacher.Id, StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("membership.role_unchanged", result.Error!.Code);
    }

    private static CurrentActorContext OwnerIn(Guid centreId, out Guid ownerId)
    {
        ownerId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(ownerId, centreId, StaffRole.Owner));
        return actorContext;
    }
}
