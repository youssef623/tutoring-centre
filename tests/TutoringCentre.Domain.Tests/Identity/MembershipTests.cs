using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class MembershipTests
{
    private static readonly Guid UserId = Guid.CreateVersion7();
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Fact]
    public void Create_WithValidInput_SucceedsAsActiveWithTheGivenRole()
    {
        // Act
        var result = Membership.Create(UserId, CentreId, StaffRole.Teacher);

        // Assert
        Assert.True(result.IsSuccess);
        var membership = result.Value;
        Assert.Equal(UserId, membership.UserId);
        Assert.Equal(CentreId, membership.CentreId);
        Assert.Equal(StaffRole.Teacher, membership.Role);
        Assert.Equal(MembershipStatus.Active, membership.Status);
    }

    [Fact]
    public void Create_WithEmptyUserId_ReturnsUserRequired()
    {
        // Act
        var result = Membership.Create(Guid.Empty, CentreId, StaffRole.Owner);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.user_required", result.Error!.Code);
    }

    [Fact]
    public void Create_WithEmptyCentreId_ReturnsCentreRequired()
    {
        // Act
        var result = Membership.Create(UserId, Guid.Empty, StaffRole.Owner);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.centre_required", result.Error!.Code);
    }

    [Fact]
    public void ChangeRole_WhenActive_SucceedsAndChangesTheRole()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Teacher).Value;

        // Act
        var result = membership.ChangeRole(StaffRole.Secretary);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StaffRole.Secretary, membership.Role);
    }

    [Fact]
    public void ChangeRole_WhenInactive_FailsAndLeavesTheRoleUnchanged()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Teacher).Value;
        membership.Deactivate();

        // Act
        var result = membership.ChangeRole(StaffRole.Secretary);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.inactive", result.Error!.Code);
        Assert.Equal(StaffRole.Teacher, membership.Role);
    }

    [Fact]
    public void ChangeRole_ToTheSameRole_FailsAndLeavesTheRoleUnchanged()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Teacher).Value;

        // Act
        var result = membership.ChangeRole(StaffRole.Teacher);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.role_unchanged", result.Error!.Code);
        Assert.Equal(StaffRole.Teacher, membership.Role);
    }

    [Fact]
    public void Deactivate_WhenActive_SucceedsAndBecomesInactive()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Secretary).Value;

        // Act
        var result = membership.Deactivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipStatus.Inactive, membership.Status);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ReturnsAlreadyInactive()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Secretary).Value;
        membership.Deactivate();

        // Act
        var result = membership.Deactivate();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.already_inactive", result.Error!.Code);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ReturnsAlreadyActive()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Owner).Value;

        // Act
        var result = membership.Activate();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.already_active", result.Error!.Code);
    }
}
