using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Queries.ValidateStaffSession;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

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
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", null, null);

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
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-current", true, null) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-stale", null, null);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_CentreSetAndMembershipInactive_ReturnsFalse()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", false, StaffRole.Teacher) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", CentreId, StaffRole.Teacher);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_RoleNoLongerMatchesTheMembership_ReturnsFalse()
    {
        // Arrange: the cookie still claims Teacher, but the membership was promoted to Secretary since issue.
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", true, StaffRole.Secretary) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", CentreId, StaffRole.Teacher);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_MatchingStampAndNoCentre_ReturnsTrue()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", true, null) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", null, null);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.Value);
    }

    [Fact]
    public async Task HandleAsync_MatchingStampActiveCentreMembershipAndRole_ReturnsTrue()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", true, StaffRole.Owner) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", CentreId, StaffRole.Owner);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.Value);
    }
}
