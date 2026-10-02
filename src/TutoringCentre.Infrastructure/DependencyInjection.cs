using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace TutoringCentre.Infrastructure;

public static class DependencyInjection
{
    private const string PostgresConnectionStringName = "Postgres";
    private const string PostgresHealthCheckName = "postgres";
    private static readonly string[] ReadinessTags = ["ready"];

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(PostgresConnectionStringName);
        var healthChecks = services.AddHealthChecks();

        if (string.IsNullOrEmpty(connectionString))
        {
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
