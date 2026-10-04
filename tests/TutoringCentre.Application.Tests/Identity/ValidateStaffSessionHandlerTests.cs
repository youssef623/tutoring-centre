using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Queries.ValidateStaffSession;
using TutoringCentre.Application.Tests.Fakes;

namespace TutoringCentre.Application.Tests.Identity;

public sealed class ValidateStaffSessionHandlerTests
{
    private static readonly Guid UserId = Guid.CreateVersion7();
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownUser_ReturnsFalse()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = null };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", null);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_WrongSecurityStamp_ReturnsFalse()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-current", true) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-stale", null);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_CentreSetAndMembershipInactive_ReturnsFalse()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", false) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", CentreId);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_MatchingStampAndNoCentre_ReturnsTrue()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", true) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", null);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.Value);
    }

    [Fact]
    public async Task HandleAsync_MatchingStampAndActiveCentreMembership_ReturnsTrue()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", true) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", CentreId);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.Value);
    }
}
