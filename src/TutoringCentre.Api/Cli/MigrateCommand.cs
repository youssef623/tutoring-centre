using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Cli;

/// <summary>
/// Applies pending migrations using ConnectionStrings:PostgresMigrations (tutoring_owner) and exits. A deploy
/// pipeline step, not something the long-running web process ever does itself (Task 34.7): owner credentials
/// belong to a short-lived job, not a process that serves the internet. Idempotent — a database already up to
/// date is a no-op, since <see cref="MigrationRunner.ApplyMigrationsAsync"/> only ever applies what is pending.
/// </summary>
public static class MigrateCommand
{
    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "A handful of one-off log lines in a short-lived CLI command; a source-generated delegate is unwarranted here.")]
    public static async Task<int> RunAsync(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("TutoringCentre.Api.Cli.MigrateCommand");

        try
        {
            await services.ApplyMigrationsAsync();
            logger.LogInformation("Migrate: database is up to date.");
            return 0;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Migrate: applying migrations failed.");
            return 1;
        }
    }
}
