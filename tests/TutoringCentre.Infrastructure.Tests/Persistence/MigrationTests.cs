using TutoringCentre.Infrastructure.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>A brand-new environment (CI, production) must be buildable from the migrations alone.</summary>
public sealed class MigrationTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task PortalMigrationPreservesExistingMemberships()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();
        AppDbContextOptionsConfigurator.Configure(options, Fixture.OwnerConnectionString);
        await using var context = new AppDbContext(options.Options, new CurrentActorContext());
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync("20261029152645_AddConversations");
        try
        {
            var user = await Fixture.SeedUserAsync();
            var centre = Guid.NewGuid();
            await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
            await connection.OpenAsync();
            await using var seed = new NpgsqlCommand($"""
                insert into platform.centres (id,name,slug,time_zone_id,default_locale,created_at)
                values ('{centre}','Existing Centre','existing-centre','Africa/Cairo','en',now());
                insert into identity.memberships (id,centre_id,user_id,role,status,created_at)
                values ('{Guid.NewGuid()}','{centre}','{user}','owner','active',now());
                """, connection);
            await seed.ExecuteNonQueryAsync();
            await migrator.MigrateAsync();
            Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from identity.memberships"));
            Assert.True(await Fixture.ScalarAsync<bool>("select to_regclass('identity.portal_links') is not null"));
        }
        finally
        {
            await migrator.MigrateAsync();
        }
    }

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
