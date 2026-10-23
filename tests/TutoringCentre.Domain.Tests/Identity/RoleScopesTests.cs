using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class RoleScopesTests
{
    [Theory]
    [InlineData(StaffRole.Owner, false)]
    [InlineData(StaffRole.Secretary, false)]
    [InlineData(StaffRole.Teacher, true)]
    public void IsLimitedToOwnGroups_MatchesTheMatrixCellLiterally(StaffRole role, bool expected)
    {
        Assert.Equal(expected, RoleScopes.IsLimitedToOwnGroups(role));
    }
}
