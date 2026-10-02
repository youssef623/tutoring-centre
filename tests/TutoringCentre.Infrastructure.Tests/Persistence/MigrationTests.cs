using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>A brand-new environment (CI, production) must be buildable from the migrations alone.</summary>
public sealed class MigrationTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_CreateCentresTable()
    {
        var exists = await Fixture.ScalarAsync<bool>("select to_regclass('platform.centres') is not null");

        Assert.True(exists);
    }

    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_CreateUniqueSlugIndex()
    {
        var indexDefinition = await Fixture.ScalarAsync<string>(
            "select indexdef from pg_indexes where schemaname = 'platform' and tablename = 'centres' and indexname = 'ux_centres_slug'");

        Assert.StartsWith("CREATE UNIQUE INDEX", indexDefinition, StringComparison.Ordinal);
        Assert.Contains("slug", indexDefinition, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_CreateDefaultLocaleCheckConstraint()
    {
        var count = await Fixture.ScalarAsync<long>(
            "select count(*) from information_schema.check_constraints where constraint_schema = 'platform' and constraint_name = 'ck_centres_default_locale'");

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_KeepHistoryTableInPlatformSchema()
    {
        var exists = await Fixture.ScalarAsync<bool>("select to_regclass('platform.__ef_migrations_history') is not null");

        Assert.True(exists);
    }
}
