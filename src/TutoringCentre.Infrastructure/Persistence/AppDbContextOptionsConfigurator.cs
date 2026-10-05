using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The one place Npgsql, the snake_case naming convention and the migrations history table are configured, so the
/// runtime registration (tutoring_app, DependencyInjection.cs) and the short-lived migration context
/// (tutoring_owner, <see cref="MigrationRunner"/> and the design-time factory) can never drift apart.
/// </summary>
public static class AppDbContextOptionsConfigurator
{
    // Lets an operator find this app's connections in pg_stat_activity regardless of which role they connect as.
    // Not "TutoringCentre.Api" verbatim: that string collides with the architecture tests' namespace-dependency scan.
    private const string ApplicationName = "TutoringCentreApi";

    public static void Configure(DbContextOptionsBuilder builder, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var namedConnectionString = new NpgsqlConnectionStringBuilder(connectionString) { ApplicationName = ApplicationName }.ConnectionString;

        builder
            .UseNpgsql(namedConnectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", Schemas.Platform))
            .UseSnakeCaseNamingConvention();
    }
}
