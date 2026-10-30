using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Portal.Commands.DeactivatePortalAccount;
using TutoringCentre.Application.Identity.Portal.Commands.ReactivatePortalAccount;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity.Portal;

public sealed class PortalAccountStatusHandlerTests
{
    [Theory]
    [InlineData(PortalRole.Parent)]
    [InlineData(PortalRole.Student)]
    public async Task DeactivationAndReactivationUseMembershipRulesAndClientVersion(PortalRole role)
    {
        var centre = Guid.NewGuid();
        var user = Guid.NewGuid();
        var actor = new CurrentActorContext();
        actor.Set(new StaffActor(Guid.NewGuid(), centre, StaffRole.Owner));
        var link = role == PortalRole.Parent ? PortalLink.CreateForParent(user, centre, Guid.NewGuid(), Guid.NewGuid()).Value
            : PortalLink.CreateForStudent(user, centre, Guid.NewGuid(), Guid.NewGuid()).Value;
        var repository = new FakePortalLinkRepository { Membership = Membership.CreatePortal(user, centre, role).Value };
        repository.Links.Add(link);
        var deactivate = new DeactivatePortalAccountHandler(actor, repository);
        var reactivate = new ReactivatePortalAccountHandler(actor, repository);
        var result = await deactivate.HandleAsync(new(link.Id, 42), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(42u, repository.ExpectedVersion);
        Assert.Equal(MembershipStatus.Inactive, repository.Membership.Status);
        Assert.Equal("membership.already_inactive", (await deactivate.HandleAsync(new(link.Id, 43), CancellationToken.None)).Error!.Code);
        Assert.True((await reactivate.HandleAsync(new(link.Id, 44), CancellationToken.None)).IsSuccess);
        Assert.Equal(44u, repository.ExpectedVersion);
        Assert.Equal("membership.already_active", (await reactivate.HandleAsync(new(link.Id, 45), CancellationToken.None)).Error!.Code);
        Assert.Single(repository.Links);
    }

    [Fact]
    public async Task ForeignLinkReturnsUniformNotFoundAndLeavesMembershipActive()
    {
        var actor = new CurrentActorContext();
        actor.Set(new StaffActor(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Owner));
        var centre = Guid.NewGuid();
        var user = Guid.NewGuid();
        var link = PortalLink.CreateForParent(user, centre, Guid.NewGuid(), Guid.NewGuid()).Value;
        var repository = new FakePortalLinkRepository { Membership = Membership.CreatePortal(user, centre, PortalRole.Parent).Value };
        repository.Links.Add(link);
        Assert.Equal("portal.not_found", (await new DeactivatePortalAccountHandler(actor, repository)
            .HandleAsync(new(link.Id, 1), CancellationToken.None)).Error!.Code);
        Assert.Equal("portal.not_found", (await new ReactivatePortalAccountHandler(actor, repository)
            .HandleAsync(new(link.Id, 1), CancellationToken.None)).Error!.Code);
        Assert.Equal(MembershipStatus.Active, repository.Membership.Status);
    }

    [Fact]
    public void StatusValidatorsRequireLinkId()
    {
        Assert.False(new DeactivatePortalAccountValidator().Validate(new DeactivatePortalAccountCommand(Guid.Empty, 1)).IsValid);
        Assert.False(new ReactivatePortalAccountValidator().Validate(new ReactivatePortalAccountCommand(Guid.Empty, 1)).IsValid);
    }
}
