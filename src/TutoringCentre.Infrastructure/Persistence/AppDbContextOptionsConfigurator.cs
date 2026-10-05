using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The one place Npgsql, the snake_case naming convention, the migrations history table and the save interceptors
/// are configured, so the runtime registration (tutoring_app, DependencyInjection.cs) and the short-lived
/// migration context (tutoring_owner, <see cref="MigrationRunner"/> and the design-time factory) can never drift
/// apart — including which interceptors run on each.
/// </summary>
public static class AppDbContextOptionsConfigurator
{
    // Lets an operator find this app's connections in pg_stat_activity regardless of which role they connect as.
    // Not "TutoringCentre.Api" verbatim: that string collides with the architecture tests' namespace-dependency scan.
    private const string ApplicationName = "TutoringCentreApi";

    public static void Configure(DbContextOptionsBuilder builder, string connectionString, params IInterceptor[] interceptors)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var namedConnectionString = new NpgsqlConnectionStringBuilder(connectionString) { ApplicationName = ApplicationName }.ConnectionString;

        builder
            .UseNpgsql(namedConnectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", Schemas.Platform))
            .UseSnakeCaseNamingConvention();

        if (interceptors.Length > 0)
        {
            builder.AddInterceptors(interceptors);
        }
    }
}
