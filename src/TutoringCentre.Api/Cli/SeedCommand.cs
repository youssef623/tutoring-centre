using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Cli;

/// <summary>Creates the two demo centres and their demo subjects through the dispatcher. A CLI is just another client of the Application layer.</summary>
public static class SeedCommand
{
    private static readonly SeedCentre[] Centres =
    [
        new("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar),
        new("Maadi Learning Hub", "maadi-hub", "Africa/Cairo", SupportedLocale.En),
    ];

    private static readonly SeedSubjects[] Subjects =
    [
        new("nile-centre", ["رياضيات", "فيزياء", "كيمياء"]),
        new("maadi-hub", ["Mathematics", "Physics"]),
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

        await using (var scope = services.CreateAsyncScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentIdentitySeeder>();
            var result = await seeder.SeedAsync(CancellationToken.None);

            if (result.IsSuccess)
            {
                logger.LogInformation("Seed: staff accounts and memberships are up to date");
            }
            else
            {
                logger.LogError("Seed: seeding staff failed with {ErrorCode}", result.Error?.Code);
                exitCode = 1;
            }
        }

        // Read-only lookup, not a write: resolving which centre a slug belongs to needs no tenant enforcement
        // proof the way creating a subject does. TutoringCentre.Api.Cli is one of the two namespaces (alongside
        // Program.cs) allowed to reference Infrastructure directly.
        Dictionary<string, Guid> centreIdsBySlug;
        await using (var scope = services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            centreIdsBySlug = await db.Set<Centre>().ToDictionaryAsync(centre => centre.Slug, centre => centre.Id, CancellationToken.None);
        }

        foreach (var centreSubjects in Subjects)
        {
            if (!centreIdsBySlug.TryGetValue(centreSubjects.CentreSlug, out var centreId))
            {
                logger.LogError("Seed: centre {Slug} does not exist; cannot seed its subjects", centreSubjects.CentreSlug);
                exitCode = 1;
                continue;
            }

            // One scope per centre: the actor (and therefore the tenant step, write guard and RLS) is set once,
            // then every subject for that centre dispatches through the same scope's Dispatcher.
            await using var subjectScope = services.CreateAsyncScope();
            subjectScope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(centreId));
            var dispatcher = subjectScope.ServiceProvider.GetRequiredService<Dispatcher>();

            foreach (var name in centreSubjects.Names)
            {
                var result = await dispatcher.SendAsync<CreateSubjectCommand, CreateSubjectResult>(
                    new CreateSubjectCommand(name), CancellationToken.None);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Seed: created subject {Name} in {Slug}", name, centreSubjects.CentreSlug);
                }
                else if (result.Error?.Code == "subject.name_taken")
                {
                    logger.LogInformation("Seed: subject {Name} in {Slug} already exists", name, centreSubjects.CentreSlug);
                }
                else
                {
                    logger.LogError(
                        "Seed: creating subject {Name} in {Slug} failed with {ErrorCode}", name, centreSubjects.CentreSlug, result.Error?.Code);
                    exitCode = 1;
                }
            }
        }

        return exitCode;
    }

    private sealed record SeedCentre(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale);

    private sealed record SeedSubjects(string CentreSlug, string[] Names);
}
