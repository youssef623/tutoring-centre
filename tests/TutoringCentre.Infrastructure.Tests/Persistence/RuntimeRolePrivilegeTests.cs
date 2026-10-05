using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>
/// Raw-SQL evidence of what the runtime role can and cannot do (Day 19). Valid evidence only because the fixture
/// itself runs the app under test as tutoring_app — a test connecting as a superuser could never fail these.
/// </summary>
public sealed class RuntimeRolePrivilegeTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task CurrentUser_ThroughTheAppConnection_IsTutoringApp()
    {
        var currentUser = await PostgresFixture.ScalarAsync<string>(Fixture.AppConnectionString, "select current_user");

        Assert.Equal("tutoring_app", currentUser);
    }
}
