using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Platform;
using TutoringCentre.Application.Platform.Queries.GetSystemInfo;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Platform;

public sealed class SystemInfoQueryTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task QueryAsync_MigratedDatabase_ReportsLatestMigrationAsUpToDate()
    {
        var result = await Fixture.QueryAsAsync<GetSystemInfoQuery, SystemInfoDto>(new AnonymousActor(), new GetSystemInfoQuery());

        Assert.True(result.IsSuccess);
        Assert.EndsWith("_AddCentreRowVersion", result.Value.LatestMigration, StringComparison.Ordinal);
        Assert.True(result.Value.DatabaseUpToDate);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.ApplicationVersion));
    }
}
