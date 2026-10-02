using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Api.Cli;

/// <summary>Creates the two demo centres through the dispatcher. A CLI is just another client of the Application layer.</summary>
public static class SeedCommand
{
    private static readonly SeedCentre[] Centres =
    [
        new("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar),
        new("Maadi Learning Hub", "maadi-hub", "Africa/Cairo", SupportedLocale.En),
    ];

    /// <returns>0 when every centre exists afterwards; 1 when any command failed for another reason.</returns>
    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "A handful of one-off log lines in a short-lived CLI command; a source-generated delegate is unwarranted here.")]
    [SuppressMessage(
        "Performance",
        "CA1873:Avoid potentially expensive logging",
        Justification = "The logged arguments (slug, error code) are plain strings already in hand; nothing expensive is being guarded against.")]
    public static async Task<int> RunAsync(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("TutoringCentre.Api.Cli.SeedCommand");
        var exitCode = 0;

        foreach (var centre in Centres)
        {
            // One scope per command: its own actor, unit of work and DbContext.
            await using var scope = services.CreateAsyncScope();
            scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(null));
            var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();

            var result = await dispatcher.SendAsync<CreateCentreCommand, CreateCentreResult>(
                new CreateCentreCommand(centre.Name, centre.Slug, centre.TimeZoneId, centre.DefaultLocale),
                CancellationToken.None);

            if (result.IsSuccess)
            {
                logger.LogInformation("Seed: created centre {Slug}", centre.Slug);
            }
            else if (result.Error?.Code == "centre.slug_taken")
            {
                logger.LogInformation("Seed: centre {Slug} already exists", centre.Slug);
            }
            else
            {
                logger.LogError("Seed: creating centre {Slug} failed with {ErrorCode}", centre.Slug, result.Error?.Code);
                exitCode = 1;
            }
        }

        return exitCode;
    }

    private sealed record SeedCentre(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale);
}
