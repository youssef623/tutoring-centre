using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Queries.GetActiveMembership;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity;

public sealed class GetActiveMembershipHandlerTests
{
    [Fact]
    public async Task HandleAsync_AnonymousActor_ReturnsNotAuthenticated()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        var handler = new GetActiveMembershipHandler(actorContext, new FakeMembershipReadService());

        // Act
        var result = await handler.HandleAsync(new GetActiveMembershipQuery(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("auth.not_authenticated", result.Error!.Code);
        Assert.Equal(ErrorKind.Unauthenticated, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_NoActiveMembershipInCentre_ReturnsTenantNoMembership()
    {
        // Arrange — covers unknown centre, non-member and inactive member alike: the read service returns null for all three.
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), null, null));
        var readService = new FakeMembershipReadService { ActiveMembership = null };
        var handler = new GetActiveMembershipHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetActiveMembershipQuery(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("tenant.no_membership", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveMembershipInCentre_Succeeds()
    {
        // Arrange
        var centreId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), null, null));
        var readService = new FakeMembershipReadService
        {
            ActiveMembership = new ActiveMembershipDto(centreId, "Nile Centre", StaffRole.Owner),
        };
        var handler = new GetActiveMembershipHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetActiveMembershipQuery(centreId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(centreId, result.Value.CentreId);
        Assert.Equal(StaffRole.Owner, result.Value.Role);
    }
}
