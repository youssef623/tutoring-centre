using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;
namespace TutoringCentre.Infrastructure.Tests.Billing;
public sealed class BillingMigrationTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task UpgradeBackfillsPreexistingGroupsToZero()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(Fixture.OwnerConnectionString, options => options.MigrationsHistoryTable("__ef_migrations_history", "platform")).UseSnakeCaseNamingConvention().Options;
        var actor = new CurrentActorContext(); await using var db = new AppDbContext(options, actor);
        var migrations = db.Database.GetMigrations().ToArray(); var migrator = db.GetService<IMigrator>();
        Assert.Contains("AddGroupMonthlyFeeAndBilling", migrations[^1], StringComparison.Ordinal);
        try
        {
            await migrator.MigrateAsync(migrations[^2]);
            var subject = await Fixture.SeedSubjectAsync(NileCentreId, "EVAL Upgrade"); var teacher = await Fixture.SeedTeacherAsync(NileCentreId, "EVAL Upgrade");
            var group = await Fixture.SeedGroupAsync(NileCentreId, "EVAL Upgrade", subject, teacher);
            await migrator.MigrateAsync();
            Assert.Equal(0m, await Fixture.ScalarAsync<decimal>($"select monthly_fee from academics.groups where id='{group}'"));
            Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from billing.charges"));
        }
        finally { await migrator.MigrateAsync(); }
    }
}
