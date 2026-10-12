using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
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

    // Phones are synthetic, in a block no real Egyptian number could collide with (010 + six zeros + a running
    // index), never a value that looks like it could belong to an actual person. Three families per centre list
    // two student names (siblings sharing a parent); the rest list one.
    private static readonly SeedFamily[] Families =
    [
        new("nile-centre", "محمود حسن", "01000000001", ["أحمد محمود", "سارة محمود"]),
        new("nile-centre", "علي سعيد", "01000000002", ["إبراهيم علي", "ياسمين علي"]),
        new("nile-centre", "حسن عبدالله", "01000000003", ["فاطمة حسن", "يوسف حسن"]),
        new("nile-centre", "السيد محمد", "01000000004", ["مصطفى السيد"]),
        new("nile-centre", "أحمد فوزي", "01000000005", ["نور الدين أحمد"]),
        new("nile-centre", "طارق سمير", "01000000006", ["ريم طارق"]),
        new("nile-centre", "عادل منصور", "01000000007", ["كريم عادل"]),
        new("nile-centre", "وليد جمال", "01000000008", ["هبة وليد"]),
        new("nile-centre", "خالد رضا", "01000000009", ["عمر خالد"]),
        new("maadi-hub", "Robert Wilson", "01000000010", ["James Wilson"]),
        new("maadi-hub", "Michael Clark", "01000000011", ["Emma Clark"]),
        new("maadi-hub", "David Brown", "01000000012", ["Oliver Brown"]),
        new("maadi-hub", "Daniel Davis", "01000000013", ["Sophia Davis"]),
        new("maadi-hub", "Thomas Miller", "01000000014", ["Lucas Miller"]),
        new("maadi-hub", "Christopher Anderson", "01000000015", ["Mia Anderson"]),
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

        foreach (var family in Families)
        {
            if (!centreIdsBySlug.TryGetValue(family.CentreSlug, out var centreId))
            {
                logger.LogError("Seed: centre {Slug} does not exist; cannot seed its students", family.CentreSlug);
                exitCode = 1;
                continue;
            }

            // One scope per centre's worth of work would mix families from different centres into a lookup that
            // never clears between them; one scope per family keeps each lookup and every registration in it
            // scoped to exactly that family's centre, same as the subjects loop above does per centre.
            await using var familyScope = services.CreateAsyncScope();
            familyScope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(centreId));
            var dispatcher = familyScope.ServiceProvider.GetRequiredService<Dispatcher>();

            var lookup = await dispatcher.QueryAsync<LookupParentByPhoneQuery, ParentLookupDto>(
                new LookupParentByPhoneQuery(family.ParentPhone), CancellationToken.None);
            Guid? parentId = lookup.IsSuccess ? lookup.Value.Parent?.Id : null;

            foreach (var studentName in family.StudentNames)
            {
                var existingSearch = await dispatcher.QueryAsync<ListStudentsQuery, StudentPageDto>(
                    new ListStudentsQuery(studentName, true, null, 5), CancellationToken.None);
                if (existingSearch.IsSuccess && existingSearch.Value.Items.Any(item => item.FullName == studentName))
                {
                    logger.LogInformation("Seed: student {Name} in {Slug} already exists", studentName, family.CentreSlug);
                    continue;
                }

                var parentForm = parentId is { } existingParentId
                    ? new RegisterStudentParent(existingParentId, null, null)
                    : new RegisterStudentParent(null, family.ParentFullName, family.ParentPhone);

                var result = await dispatcher.SendAsync<RegisterStudentCommand, RegisterStudentResult>(
                    new RegisterStudentCommand(studentName, null, null, parentForm, ParentRelationship.Father), CancellationToken.None);

                if (result.IsSuccess)
                {
                    parentId = result.Value.ParentId;
                    logger.LogInformation("Seed: created student {Name} in {Slug}", studentName, family.CentreSlug);
                }
                else
                {
                    logger.LogError(
                        "Seed: creating student {Name} in {Slug} failed with {ErrorCode}", studentName, family.CentreSlug, result.Error?.Code);
                    exitCode = 1;
                }
            }
        }

        return exitCode;
    }

    private sealed record SeedCentre(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale);

    private sealed record SeedSubjects(string CentreSlug, string[] Names);

    private sealed record SeedFamily(string CentreSlug, string ParentFullName, string ParentPhone, string[] StudentNames);
}
