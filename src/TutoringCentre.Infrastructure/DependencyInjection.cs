using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Infrastructure.Time;

namespace TutoringCentre.Infrastructure;

/// <summary>Registers Infrastructure implementations of Application ports. Called once from the Api's composition root.</summary>
public static class DependencyInjection
{
    private const string PostgresConnectionStringName = "Postgres";
    private const string PostgresHealthCheckName = "postgres";

    // A static array avoids allocating a new one per call (analyzer CA1861 under latest-recommended).
    private static readonly string[] ReadinessTags = ["ready"];

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Time: stateless and thread-safe, so one instance for the app.
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IClock, SystemClock>();

        var connectionString = configuration.GetConnectionString(PostgresConnectionStringName);
        var healthChecks = services.AddHealthChecks();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Missing configuration must not crash startup; readiness reports it instead.
            healthChecks.AddCheck(
                PostgresHealthCheckName,
                () => HealthCheckResult.Unhealthy("ConnectionStrings:Postgres is not configured."),
                ReadinessTags);
        }
        else
        {
            healthChecks.AddNpgSql(connectionString, name: PostgresHealthCheckName, tags: ReadinessTags);
        }

        return services;
    }
}
