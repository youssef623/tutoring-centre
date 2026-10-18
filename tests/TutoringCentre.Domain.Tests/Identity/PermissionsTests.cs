using System.Text.RegularExpressions;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class PermissionsTests
{
    private static readonly Regex KeyPattern = new("^[a-z]+(\\.[a-z]+)+$", RegexOptions.Compiled);

    [Fact]
    public void All_HasTenDistinctKeysMatchingTheNamingPattern()
    {
        // Act
        var keys = Permissions.All;

        // Assert
        Assert.Equal(10, keys.Count);
        Assert.Equal(keys.Count, keys.Distinct().Count());
        Assert.All(keys, key => Assert.Matches(KeyPattern, key));
    }
}
