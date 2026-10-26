using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
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

    // Empty on purpose: no subjects, staff (beyond its owner), students or groups, so the full-setup journey builds
    // everything itself through the UI. Development and Testing only (SeedGate.IncludesSetupCentre).
    private static readonly SeedCentre SetupCentre =
        new("مركز الجيزة الجديد", DevelopmentIdentitySeeder.SetupCentreSlug, "Africa/Cairo", SupportedLocale.Ar);

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

    // The same person (the two-centre teacher account) is linked to one teacher record per centre, so the demo
    // shows the ownership guard: one account, different groups per selected centre. In Nile the linked teacher
    // owns exactly one group and the unlinked one the other three, including the two-seat last-seat demo group.
    private static readonly SeedTeacher[] Teachers =
    [
        new("nile-centre", "كريم فتحي", LinkTwoCentreTeacher: true),
        new("nile-centre", "منى السيد", LinkTwoCentreTeacher: false),
        new("maadi-hub", "Karim Fathy", LinkTwoCentreTeacher: true),
    ];

    private static readonly SeedGroup[] Groups =
    [
        new("nile-centre", "رياضيات - المجموعة أ", "رياضيات", "كريم فتحي", 20),
        new("nile-centre", "فيزياء - المجموعة أ", "فيزياء", "منى السيد", 20),
        new("nile-centre", "كيمياء - المجموعة أ", "كيمياء", "منى السيد", 15),
        new("nile-centre", "رياضيات - المجموعة ب", "رياضيات", "منى السيد", 2),
        new("maadi-hub", "Mathematics - Group A", "Mathematics", "Karim Fathy", 20),
        new("maadi-hub", "Physics - Group A", "Physics", "Karim Fathy", 20),
    ];

    // Nile: 25 enrollments across its four groups. The two-seat group gets exactly one, so the last-seat demo
    // can fill it live and then show group.full; nobody sits in both mathematics groups, so a transfer between
    // them is possible. Maadi: 6. Every student named here is one of the seeded families' children.
    private static readonly SeedEnrollment[] Enrollments =
    [
        new("nile-centre", "رياضيات - المجموعة أ",
        [
            "أحمد محمود", "سارة محمود", "إبراهيم علي", "فاطمة حسن", "يوسف حسن",
            "مصطفى السيد", "نور الدين أحمد", "ريم طارق", "كريم عادل", "هبة وليد",
        ]),
        new("nile-centre", "رياضيات - المجموعة ب", ["ياسمين علي"]),
        new("nile-centre", "فيزياء - المجموعة أ",
        [
            "أحمد محمود", "سارة محمود", "إبراهيم علي", "فاطمة حسن", "يوسف حسن", "مصطفى السيد", "ريم طارق", "عمر خالد",
        ]),
        new("nile-centre", "كيمياء - المجموعة أ", ["سارة محمود", "ياسمين علي", "فاطمة حسن", "نور الدين أحمد", "كريم عادل", "هبة وليد"]),
        new("maadi-hub", "Mathematics - Group A", ["James Wilson", "Emma Clark", "Oliver Brown", "Sophia Davis"]),
        new("maadi-hub", "Physics - Group A", ["Lucas Miller", "Mia Anderson"]),
    ];

    // Nile: six weekly rules across its four groups; Maadi: two. No two collide: the linked teacher's Nile group
    // meets Monday and Wednesday, the other Nile teacher's three groups take four different slots, and the same
    // person's Maadi groups meet on days the Nile group does not. Monday to Thursday only: the API tests and the
    // journeys schedule their own sessions on Saturdays and Sundays (some on these very groups), so the demo
    // schedule never collides with them. Times are the centre's wall-clock times; the first date is always the
    // seed day, never a calendar date written here.
    private static readonly SeedScheduleRule[] ScheduleRules =
    [
        new("nile-centre", "رياضيات - المجموعة أ", DayOfWeek.Monday, new TimeOnly(18, 0), 90, "قاعة 1"),
        new("nile-centre", "رياضيات - المجموعة أ", DayOfWeek.Wednesday, new TimeOnly(18, 0), 90, "قاعة 1"),
        new("nile-centre", "فيزياء - المجموعة أ", DayOfWeek.Monday, new TimeOnly(16, 0), 90, "قاعة 2"),
        new("nile-centre", "فيزياء - المجموعة أ", DayOfWeek.Wednesday, new TimeOnly(16, 0), 90, "قاعة 2"),
        new("nile-centre", "كيمياء - المجموعة أ", DayOfWeek.Tuesday, new TimeOnly(17, 0), 90, "قاعة 3"),
        new("nile-centre", "رياضيات - المجموعة ب", DayOfWeek.Thursday, new TimeOnly(18, 0), 60, "قاعة 1"),
        new("maadi-hub", "Mathematics - Group A", DayOfWeek.Tuesday, new TimeOnly(18, 0), 90, "Room 1"),
        new("maadi-hub", "Physics - Group A", DayOfWeek.Thursday, new TimeOnly(16, 0), 90, "Room 2"),
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
        var includeSetupCentre = SeedGate.IncludesSetupCentre(services.GetRequiredService<IHostEnvironment>());

        foreach (var centre in includeSetupCentre ? [.. Centres, SetupCentre] : Centres)
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
            var result = await seeder.SeedAsync(includeSetupCentre, CancellationToken.None);

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

        if (!await SeedTeachersAndGroupsAsync(services, centreIdsBySlug, logger))
        {
            exitCode = 1;
        }

        if (!await SeedEnrollmentsAsync(services, centreIdsBySlug, logger))
        {
            exitCode = 1;
        }

        if (!await SeedScheduleRulesAsync(services, centreIdsBySlug, logger))
        {
            exitCode = 1;
        }

        return exitCode;
    }

    /// <summary>
    /// Enrollments through <see cref="EnrollStudentCommand"/>, the same command the enroll endpoint dispatches,
    /// as <see cref="SystemActor"/> of each centre. Groups and students are resolved by name through their list
    /// queries. Idempotent: <c>enrollment.already_enrolled</c> and <c>group.full</c> count as already seeded.
    /// Every write gets its own scope, so a refused command never leaves tracked changes behind.
    /// </summary>
    /// <returns>True when every enrollment exists afterwards.</returns>
    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "A handful of one-off log lines in a short-lived CLI command; a source-generated delegate is unwarranted here.")]
    [SuppressMessage(
        "Performance",
        "CA1873:Avoid potentially expensive logging",
        Justification = "The logged arguments (names, slug, error code) are plain strings already in hand; nothing expensive is being guarded against.")]
    private static async Task<bool> SeedEnrollmentsAsync(IServiceProvider services, Dictionary<string, Guid> centreIdsBySlug, ILogger logger)
    {
        var succeeded = true;

        foreach (var seedEnrollment in Enrollments)
        {
            if (!centreIdsBySlug.TryGetValue(seedEnrollment.CentreSlug, out var centreId))
            {
                logger.LogError("Seed: centre {Slug} does not exist; cannot seed its enrollments", seedEnrollment.CentreSlug);
                succeeded = false;
                continue;
            }

            Guid? groupId;
            await using (var scope = CreateCentreScope(services, centreId, out var dispatcher))
            {
                var groups = await dispatcher.QueryAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(
                    new ListGroupsQuery(null, null, true), CancellationToken.None);
                groupId = groups.IsSuccess ? groups.Value.FirstOrDefault(group => group.Name == seedEnrollment.GroupName)?.Id : null;
            }

            if (groupId is null)
            {
                logger.LogError("Seed: group {Name} in {Slug} does not exist; cannot seed its enrollments", seedEnrollment.GroupName, seedEnrollment.CentreSlug);
                succeeded = false;
                continue;
            }

            foreach (var studentName in seedEnrollment.StudentNames)
            {
                await using var scope = CreateCentreScope(services, centreId, out var dispatcher);
                var search = await dispatcher.QueryAsync<ListStudentsQuery, StudentPageDto>(
                    new ListStudentsQuery(studentName, true, null, 5), CancellationToken.None);
                var studentId = search.IsSuccess ? search.Value.Items.FirstOrDefault(item => item.FullName == studentName)?.Id : null;
                if (studentId is null)
                {
                    logger.LogError("Seed: student {Name} in {Slug} does not exist; cannot enroll", studentName, seedEnrollment.CentreSlug);
                    succeeded = false;
                    continue;
                }

                var result = await dispatcher.SendAsync<EnrollStudentCommand, EnrollStudentResult>(
                    new EnrollStudentCommand(groupId.Value, studentId.Value), CancellationToken.None);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Seed: enrolled {Student} in {Group} in {Slug}", studentName, seedEnrollment.GroupName, seedEnrollment.CentreSlug);
                }
                else if (result.Error?.Code is "enrollment.already_enrolled" or "group.full")
                {
                    logger.LogInformation(
                        "Seed: {Student} in {Group} in {Slug} already seeded ({ErrorCode})", studentName, seedEnrollment.GroupName, seedEnrollment.CentreSlug, result.Error.Code);
                }
                else
                {
                    logger.LogError(
                        "Seed: enrolling {Student} in {Group} in {Slug} failed with {ErrorCode}", studentName, seedEnrollment.GroupName, seedEnrollment.CentreSlug, result.Error?.Code);
                    succeeded = false;
                }
            }
        }

        return succeeded;
    }

    /// <summary>
    /// Teachers, the two-centre teacher's account links and the groups, all through the same commands an HTTP
    /// client would dispatch, as <see cref="SystemActor"/> of each centre (which resolves to the All scope).
    /// Idempotent: teachers are matched by name before creating, and a <c>teacher.account_taken</c> or
    /// <c>group.name_taken</c> result counts as already seeded. Every write gets its own scope, so a refused
    /// command never leaves tracked changes behind for the next one to save.
    /// </summary>
    /// <returns>True when every teacher, link and group exists afterwards.</returns>
    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "A handful of one-off log lines in a short-lived CLI command; a source-generated delegate is unwarranted here.")]
    [SuppressMessage(
        "Performance",
        "CA1873:Avoid potentially expensive logging",
        Justification = "The logged arguments (names, slug, error code) are plain strings already in hand; nothing expensive is being guarded against.")]
    private static async Task<bool> SeedTeachersAndGroupsAsync(IServiceProvider services, Dictionary<string, Guid> centreIdsBySlug, ILogger logger)
    {
        var succeeded = true;

        // The account comes from the identity seeding that already ran, looked up through Identity itself.
        Guid? twoCentreTeacherUserId;
        await using (var scope = services.CreateAsyncScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            twoCentreTeacherUserId = (await userManager.FindByEmailAsync(DevelopmentIdentitySeeder.TwoCentreTeacherEmail))?.Id;
        }

        if (twoCentreTeacherUserId is null)
        {
            logger.LogError("Seed: the two-centre teacher account does not exist; cannot link teachers");
            succeeded = false;
        }

        foreach (var (slug, centreId) in centreIdsBySlug)
        {
            if (!Teachers.Any(teacher => teacher.CentreSlug == slug) && !Groups.Any(group => group.CentreSlug == slug))
            {
                continue;
            }

            var teacherIdsByName = new Dictionary<string, Guid>(StringComparer.Ordinal);
            IReadOnlyList<TeacherDto> existingTeachers;
            Dictionary<string, Guid> subjectIdsByName;
            await using (var scope = CreateCentreScope(services, centreId, out var dispatcher))
            {
                var teachers = await dispatcher.QueryAsync<ListTeachersQuery, IReadOnlyList<TeacherDto>>(new ListTeachersQuery(true), CancellationToken.None);
                var subjects = await dispatcher.QueryAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(new ListSubjectsQuery(true), CancellationToken.None);
                if (!teachers.IsSuccess || !subjects.IsSuccess)
                {
                    logger.LogError("Seed: listing teachers or subjects in {Slug} failed", slug);
                    succeeded = false;
                    continue;
                }

                existingTeachers = teachers.Value;
                subjectIdsByName = subjects.Value.ToDictionary(subject => subject.Name, subject => subject.Id, StringComparer.Ordinal);
            }

            foreach (var seedTeacher in Teachers.Where(teacher => teacher.CentreSlug == slug))
            {
                var existing = existingTeachers.FirstOrDefault(teacher => teacher.FullName == seedTeacher.FullName);
                Guid teacherId;
                if (existing is not null)
                {
                    teacherId = existing.Id;
                    logger.LogInformation("Seed: teacher {Name} in {Slug} already exists", seedTeacher.FullName, slug);
                }
                else
                {
                    await using var scope = CreateCentreScope(services, centreId, out var dispatcher);
                    var created = await dispatcher.SendAsync<CreateTeacherCommand, CreateTeacherResult>(
                        new CreateTeacherCommand(seedTeacher.FullName, null), CancellationToken.None);
                    if (!created.IsSuccess)
                    {
                        logger.LogError("Seed: creating teacher {Name} in {Slug} failed with {ErrorCode}", seedTeacher.FullName, slug, created.Error?.Code);
                        succeeded = false;
                        continue;
                    }

                    teacherId = created.Value.TeacherId;
                    logger.LogInformation("Seed: created teacher {Name} in {Slug}", seedTeacher.FullName, slug);
                }

                teacherIdsByName[seedTeacher.FullName] = teacherId;

                if (!seedTeacher.LinkTwoCentreTeacher || twoCentreTeacherUserId is not { } userId || existing?.UserId == userId)
                {
                    continue;
                }

                await using (var scope = CreateCentreScope(services, centreId, out var dispatcher))
                {
                    var current = await dispatcher.QueryAsync<GetTeacherQuery, TeacherDto>(new GetTeacherQuery(teacherId), CancellationToken.None);
                    var linked = current.IsSuccess
                        ? await dispatcher.SendAsync<SetTeacherAccountCommand, Unit>(
                            new SetTeacherAccountCommand(teacherId, userId, current.Value.Version), CancellationToken.None)
                        : Result<Unit>.Failure(current.Error!);

                    if (linked.IsSuccess)
                    {
                        logger.LogInformation("Seed: linked teacher {Name} in {Slug} to the two-centre teacher account", seedTeacher.FullName, slug);
                    }
                    else if (linked.Error?.Code == "teacher.account_taken")
                    {
                        logger.LogInformation("Seed: the two-centre teacher account is already linked in {Slug}", slug);
                    }
                    else
                    {
                        logger.LogError(
                            "Seed: linking teacher {Name} in {Slug} failed with {ErrorCode}", seedTeacher.FullName, slug, linked.Error?.Code);
                        succeeded = false;
                    }
                }
            }

            foreach (var seedGroup in Groups.Where(group => group.CentreSlug == slug))
            {
                if (!subjectIdsByName.TryGetValue(seedGroup.SubjectName, out var subjectId)
                    || !teacherIdsByName.TryGetValue(seedGroup.TeacherName, out var teacherId))
                {
                    logger.LogError("Seed: group {Name} in {Slug} references a missing subject or teacher", seedGroup.Name, slug);
                    succeeded = false;
                    continue;
                }

                await using var scope = CreateCentreScope(services, centreId, out var dispatcher);
                var result = await dispatcher.SendAsync<CreateGroupCommand, CreateGroupResult>(
                    new CreateGroupCommand(seedGroup.Name, subjectId, teacherId, seedGroup.Capacity), CancellationToken.None);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Seed: created group {Name} in {Slug}", seedGroup.Name, slug);
                }
                else if (result.Error?.Code == "group.name_taken")
                {
                    logger.LogInformation("Seed: group {Name} in {Slug} already exists", seedGroup.Name, slug);
                }
                else
                {
                    logger.LogError("Seed: creating group {Name} in {Slug} failed with {ErrorCode}", seedGroup.Name, slug, result.Error?.Code);
                    succeeded = false;
                }
            }
        }

        return succeeded;
    }

    /// <summary>
    /// Weekly rules through <see cref="AddScheduleRuleCommand"/>, the same command the schedule endpoint
    /// dispatches, as <see cref="SystemActor"/> of each centre; the command generates each rule's sessions through
    /// the horizon. <c>EffectiveFrom</c> is the seed day in the centre's own zone, so the timetable always has this
    /// week's sessions. Idempotent: a rule is skipped when its group already has one with the same weekday and
    /// start time (read through <see cref="ListGroupScheduleRulesQuery"/>). A double-booking is never "already
    /// seeded": the rules above are chosen not to collide, so one means the seed's own data is wrong and it fails.
    /// </summary>
    /// <returns>True when every rule exists afterwards.</returns>
    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "A handful of one-off log lines in a short-lived CLI command; a source-generated delegate is unwarranted here.")]
    [SuppressMessage(
        "Performance",
        "CA1873:Avoid potentially expensive logging",
        Justification = "The logged arguments (names, slug, weekday, time, error code) are plain values already in hand; nothing expensive is being guarded against.")]
    private static async Task<bool> SeedScheduleRulesAsync(IServiceProvider services, Dictionary<string, Guid> centreIdsBySlug, ILogger logger)
    {
        var succeeded = true;

        foreach (var centreRules in ScheduleRules.GroupBy(rule => rule.CentreSlug))
        {
            var slug = centreRules.Key;
            if (!centreIdsBySlug.TryGetValue(slug, out var centreId))
            {
                logger.LogError("Seed: centre {Slug} does not exist; cannot seed its schedules", slug);
                succeeded = false;
                continue;
            }

            DateOnly today;
            Dictionary<string, Guid> groupIdsByName;
            await using (var scope = CreateCentreScope(services, centreId, out var dispatcher))
            {
                var zone = await scope.ServiceProvider.GetRequiredService<ICentreTimeZoneProvider>().GetForCurrentCentreAsync(CancellationToken.None);
                today = CentreCalendar.Today(scope.ServiceProvider.GetRequiredService<IClock>(), zone);

                var groups = await dispatcher.QueryAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(
                    new ListGroupsQuery(null, null, true), CancellationToken.None);
                if (!groups.IsSuccess)
                {
                    logger.LogError("Seed: listing groups in {Slug} failed", slug);
                    succeeded = false;
                    continue;
                }

                groupIdsByName = groups.Value.ToDictionary(group => group.Name, group => group.Id, StringComparer.Ordinal);
            }

            foreach (var seedRule in centreRules)
            {
                if (!groupIdsByName.TryGetValue(seedRule.GroupName, out var groupId))
                {
                    logger.LogError("Seed: group {Name} in {Slug} does not exist; cannot seed its schedule", seedRule.GroupName, slug);
                    succeeded = false;
                    continue;
                }

                await using var scope = CreateCentreScope(services, centreId, out var dispatcher);
                var existing = await dispatcher.QueryAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
                    new ListGroupScheduleRulesQuery(groupId), CancellationToken.None);
                if (!existing.IsSuccess)
                {
                    logger.LogError(
                        "Seed: listing the rules of {Group} in {Slug} failed with {ErrorCode}", seedRule.GroupName, slug, existing.Error?.Code);
                    succeeded = false;
                    continue;
                }

                if (existing.Value.Any(rule => rule.DayOfWeek == seedRule.DayOfWeek && rule.StartTime == seedRule.StartTime))
                {
                    logger.LogInformation(
                        "Seed: {Group} in {Slug} already meets on {Day} at {Time}", seedRule.GroupName, slug, seedRule.DayOfWeek, seedRule.StartTime);
                    continue;
                }

                var result = await dispatcher.SendAsync<AddScheduleRuleCommand, AddScheduleRuleResult>(
                    new AddScheduleRuleCommand(groupId, seedRule.DayOfWeek, seedRule.StartTime, seedRule.DurationMinutes, seedRule.Room, today),
                    CancellationToken.None);

                if (result.IsSuccess)
                {
                    logger.LogInformation(
                        "Seed: {Group} in {Slug} meets on {Day} at {Time} ({Sessions} sessions)",
                        seedRule.GroupName, slug, seedRule.DayOfWeek, seedRule.StartTime, result.Value.SessionsCreated);
                }
                else if (result.Error?.Code is "schedule.teacher_double_booked" or "schedule.group_double_booked")
                {
                    logger.LogError(
                        "Seed: the seed's own schedule collides: {Group} in {Slug} on {Day} at {Time} failed with {ErrorCode}",
                        seedRule.GroupName, slug, seedRule.DayOfWeek, seedRule.StartTime, result.Error.Code);
                    succeeded = false;
                }
                else
                {
                    logger.LogError(
                        "Seed: scheduling {Group} in {Slug} on {Day} at {Time} failed with {ErrorCode}",
                        seedRule.GroupName, slug, seedRule.DayOfWeek, seedRule.StartTime, result.Error?.Code);
                    succeeded = false;
                }
            }
        }

        return succeeded;
    }

    private static AsyncServiceScope CreateCentreScope(IServiceProvider services, Guid centreId, out Dispatcher dispatcher)
    {
        var scope = services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(centreId));
        dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();
        return scope;
    }

    private sealed record SeedTeacher(string CentreSlug, string FullName, bool LinkTwoCentreTeacher);

    private sealed record SeedGroup(string CentreSlug, string Name, string SubjectName, string TeacherName, int Capacity);

    private sealed record SeedEnrollment(string CentreSlug, string GroupName, string[] StudentNames);

    private sealed record SeedScheduleRule(
        string CentreSlug, string GroupName, DayOfWeek DayOfWeek, TimeOnly StartTime, int DurationMinutes, string Room);

    private sealed record SeedCentre(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale);

    private sealed record SeedSubjects(string CentreSlug, string[] Names);

    private sealed record SeedFamily(string CentreSlug, string ParentFullName, string ParentPhone, string[] StudentNames);
}
