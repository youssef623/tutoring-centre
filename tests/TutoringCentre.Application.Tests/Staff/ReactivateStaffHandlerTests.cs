using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class ReactivateStaffHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_AlreadyActive_ReturnsAlreadyActive()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        memberships.Memberships.Add(teacher);
        var handler = new ReactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ReactivateStaffCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("membership.already_active", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_Inactive_Succeeds()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        teacher.Deactivate();
        memberships.Memberships.Add(teacher);
        var handler = new ReactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ReactivateStaffCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipStatus.Active, teacher.Status);
    }

    private static CurrentActorContext OwnerIn(Guid centreId)
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), centreId, StaffRole.Owner));
        return actorContext;
    }
}
