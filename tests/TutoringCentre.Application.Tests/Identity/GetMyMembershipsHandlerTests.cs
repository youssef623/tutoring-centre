using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Queries.GetMyMemberships;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity;

public sealed class GetMyMembershipsHandlerTests
{
    [Fact]
    public async Task HandleAsync_AnonymousActor_ReturnsNotAuthenticated()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        var handler = new GetMyMembershipsHandler(actorContext, new FakeMembershipReadService());

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("auth.not_authenticated", result.Error!.Code);
        Assert.Equal(ErrorKind.Unauthenticated, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_NoProfileForActor_ReturnsNotAuthenticated()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, null, null));
        var readService = new FakeMembershipReadService { Profile = null };
        var handler = new GetMyMembershipsHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("auth.not_authenticated", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_ActorCentreStillActive_ReportsActiveCentreAndRole()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var centreId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, centreId, StaffRole.Teacher));
        var membership = new MembershipDto(centreId, "Nile Centre", "nile-centre", StaffRole.Teacher);
        var readService = new FakeMembershipReadService
        {
            Profile = new StaffProfileDto(userId, "Teacher", "teacher@both.test", "en", [membership]),
        };
        var handler = new GetMyMembershipsHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(centreId, result.Value.ActiveCentreId);
        Assert.Equal(StaffRole.Teacher, result.Value.ActiveRole);
        Assert.Single(result.Value.Memberships);
        Assert.Equal(["subjects.view"], result.Value.Permissions);
    }

    [Fact]
    public async Task HandleAsync_SecretaryWithActiveCentre_ReportsSortedPermissions()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var centreId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, centreId, StaffRole.Secretary));
        var membership = new MembershipDto(centreId, "Nile Centre", "nile-centre", StaffRole.Secretary);
        var readService = new FakeMembershipReadService
        {
            Profile = new StaffProfileDto(userId, "Secretary", "secretary@nile.test", "ar", [membership]),
        };
        var handler = new GetMyMembershipsHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(["subjects.manage", "subjects.view"], result.Value.Permissions);
    }

    [Fact]
    public async Task HandleAsync_ActorCentreNoLongerActive_DropsActiveCentreAndRole()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var staleCentreId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, staleCentreId, StaffRole.Secretary));
        var readService = new FakeMembershipReadService
        {
            // The actor still claims staleCentreId, but the freshly read profile no longer lists it as active.
            Profile = new StaffProfileDto(userId, "Secretary", "secretary@nile.test", "ar", []),
        };
        var handler = new GetMyMembershipsHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.ActiveCentreId);
        Assert.Null(result.Value.ActiveRole);
        Assert.Empty(result.Value.Permissions);
    }
}
