using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class DeactivateStaffHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_TargetIsTheActorsOwnMembership_ReturnsCannotChangeSelf()
    {
        var ownerId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(ownerId, NileCentreId, StaffRole.Owner));
        var memberships = new FakeMembershipRepository();
        var ownMembership = Membership.Create(ownerId, NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(ownMembership);
        var handler = new DeactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new DeactivateStaffCommand(ownMembership.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.cannot_change_self", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_DeactivatingTheOnlyActiveOwner_ReturnsLastOwner()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var onlyOwner = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(onlyOwner);
        var handler = new DeactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new DeactivateStaffCommand(onlyOwner.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.last_owner", result.Error!.Code);
        Assert.Equal(MembershipStatus.Active, onlyOwner.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyInactive_ReturnsAlreadyInactive()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        teacher.Deactivate();
        memberships.Memberships.Add(teacher);
        var handler = new DeactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new DeactivateStaffCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("membership.already_inactive", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_ActiveTeacher_Succeeds()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        memberships.Memberships.Add(teacher);
        var handler = new DeactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new DeactivateStaffCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipStatus.Inactive, teacher.Status);
    }

    private static CurrentActorContext OwnerIn(Guid centreId)
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), centreId, StaffRole.Owner));
        return actorContext;
    }
}
