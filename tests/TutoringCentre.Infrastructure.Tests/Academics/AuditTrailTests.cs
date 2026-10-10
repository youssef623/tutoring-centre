using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Identity;

// Task 23.6's test-only RenameSubjectCommand, declared directly in this namespace (SubjectTestRequests.cs),
// would otherwise shadow the real (production) one for the identical simple name.
using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 31.7: the audit trail proven through real commands against real PostgreSQL. Every assertion reads
/// audit.audit_entries through the superuser connection (<see cref="PostgresFixture.ScalarAsync{T}(string)"/>),
/// never through the application's own filtered view of it.
/// </summary>
public sealed class AuditTrailTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task CreateSubject_AsStaffOwner_WritesOneCreatedAuditRow()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-1@nile.test", StaffRole.Owner);

        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        Assert.True(created.IsSuccess);

        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type = 'subject'"));
        Assert.Equal(
            "created",
            await Fixture.ScalarAsync<string>($"select action from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            "Mathematics",
            await Fixture.ScalarAsync<string>($"select changes->'name'->>'after' from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            "staff",
            await Fixture.ScalarAsync<string>($"select actor_type from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            owner.UserId,
            await Fixture.ScalarAsync<Guid>($"select actor_user_id from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            NileCentreId,
            await Fixture.ScalarAsync<Guid>($"select centre_id from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
    }

    [Fact]
    public async Task RenameSubject_ChangesName_WritesOneUpdatedAuditRowWithNoStatusKey()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-2@nile.test", StaffRole.Owner);
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(owner.Actor, new GetSubjectQuery(created.Value.SubjectId));

        var renamed = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            owner.Actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", dto.Value.Version));
        Assert.True(renamed.IsSuccess);

        var changes = await Fixture.ScalarAsync<string>(
            $"select changes::text from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and action = 'updated'");
        Assert.Equal(
            "Mathematics",
            await Fixture.ScalarAsync<string>(
                $"select changes->'name'->>'before' from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and action = 'updated'"));
        Assert.Equal(
            "Applied Mathematics",
            await Fixture.ScalarAsync<string>(
                $"select changes->'name'->>'after' from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and action = 'updated'"));
        Assert.DoesNotContain("status", changes, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RenameSubject_ToItsOwnCurrentName_WritesNoAdditionalAuditRow()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-3@nile.test", StaffRole.Owner);
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(owner.Actor, new GetSubjectQuery(created.Value.SubjectId));
        var countAfterCreate = await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries");

        var renamed = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            owner.Actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Mathematics", dto.Value.Version));

        Assert.True(renamed.IsSuccess);
        Assert.Equal(countAfterCreate, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries"));
    }

    [Fact]
    public async Task ArchiveSubject_WritesUpdatedAuditRowWithStatusActiveBeforeAndArchivedAfter()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-4@nile.test", StaffRole.Owner);
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(owner.Actor, new GetSubjectQuery(created.Value.SubjectId));

        var archived = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(
            owner.Actor, new ArchiveSubjectCommand(created.Value.SubjectId, dto.Value.Version));
        Assert.True(archived.IsSuccess);

        Assert.Equal(
            "active",
            await Fixture.ScalarAsync<string>(
                $"select changes->'status'->>'before' from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and action = 'updated'"));
        Assert.Equal(
            "archived",
            await Fixture.ScalarAsync<string>(
                $"select changes->'status'->>'after' from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and action = 'updated'"));
    }

    [Fact]
    public async Task ChangeStaffRole_WritesUpdatedAuditRowWithRoleBeforeAndAfter()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-5@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "teacher-5@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(owner, teacher.MembershipId);

        var result = await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(
            owner.Actor, new ChangeStaffRoleCommand(teacher.MembershipId, StaffRole.Secretary, version));
        Assert.True(result.IsSuccess);

        Assert.Equal(
            "teacher",
            await Fixture.ScalarAsync<string>(
                $"select changes->'role'->>'before' from audit.audit_entries where entity_id = '{teacher.MembershipId}' and action = 'updated'"));
        Assert.Equal(
            "secretary",
            await Fixture.ScalarAsync<string>(
                $"select changes->'role'->>'after' from audit.audit_entries where entity_id = '{teacher.MembershipId}' and action = 'updated'"));
    }

    [Fact]
    public async Task CreateStaff_WritesCreatedMembershipAuditRowWithNoPasswordOrEmail()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-6@nile.test", StaffRole.Owner);

        var createdTeacher = await Fixture.SendAsAsync<CreateStaffCommand, CreateStaffResult>(
            owner.Actor, new CreateStaffCommand("teacher-6@nile.test", "Teacher Six", StaffRole.Teacher, "en"));
        Assert.True(createdTeacher.IsSuccess);

        Assert.Equal(
            "created",
            await Fixture.ScalarAsync<string>($"select action from audit.audit_entries where entity_id = '{createdTeacher.Value.MembershipId}'"));
        var changes = await Fixture.ScalarAsync<string>(
            $"select changes::text from audit.audit_entries where entity_id = '{createdTeacher.Value.MembershipId}'");
        Assert.Contains("userId", changes, StringComparison.Ordinal);
        Assert.DoesNotContain("password", changes, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("email", changes, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CommandThatRenamesThenFails_WritesNoAuditRowAndNoDataChange()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-7@nile.test", StaffRole.Owner);
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        var countAfterCreate = await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries");

        var result = await Fixture.SendAsAsync<RenameSubjectThenFailCommand, Unit>(
            owner.Actor, new RenameSubjectThenFailCommand(created.Value.SubjectId, "Applied Mathematics"));

        Assert.True(result.IsFailure);
        Assert.Equal("test.deliberate_failure", result.Error!.Code);
        Assert.Equal(countAfterCreate, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries"));
        Assert.Equal(
            "Mathematics",
            await Fixture.ScalarAsync<string>($"select name from academics.subjects where id = '{created.Value.SubjectId}'"));
    }

    [Fact]
    public async Task CreateSubject_LosesUniqueRace_WritesNoAuditRowForTheLoser()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-8@nile.test", StaffRole.Owner);
        var gate = new TaskCompletionSource();

        async Task<Result<CreateSubjectResult>> AttemptAsync()
        {
            await gate.Task;
            return await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        }

        var first = Task.Run(AttemptAsync);
        var second = Task.Run(AttemptAsync);
        gate.SetResult();
        var results = await Task.WhenAll(first, second);

        var winner = Assert.Single(results, result => result.IsSuccess);
        Assert.Single(results, result => result.IsFailure);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type = 'subject'"));
        Assert.Equal(
            winner.Value.SubjectId,
            await Fixture.ScalarAsync<Guid>("select entity_id from audit.audit_entries where entity_type = 'subject'"));
    }

    [Fact]
    public async Task CreateSubject_AsSystemActorWithCentre_WritesAuditRowWithSystemActorTypeAndNullUser()
    {
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(
            new SystemActor(NileCentreId), new CreateSubjectCommand("رياضيات"));
        Assert.True(created.IsSuccess);

        Assert.Equal(
            "system",
            await Fixture.ScalarAsync<string>($"select actor_type from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            0,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and actor_user_id is not null"));
    }

    [Fact]
    public async Task DevelopmentSeederAndCentrelessCentreCreation_WriteNoAuditRows()
    {
        var centreResult = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(
            new SystemActor(null), new CreateCentreCommand("Bootstrap Centre", "bootstrap-centre-31-7", "Africa/Cairo", SupportedLocale.En));
        Assert.True(centreResult.IsSuccess);

        var seedProvider = Fixture.CreateServiceProvider(services =>
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Postgres"] = Fixture.AppConnectionString,
                    ["ConnectionStrings:PostgresMigrations"] = Fixture.OwnerConnectionString,
                    ["Seed:Password"] = "Audit-Test-Seed-Passw0rd!",
                })
                .Build()));

        await using var scope = seedProvider.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentIdentitySeeder>();
        var seedResult = await seeder.SeedAsync(CancellationToken.None);

        Assert.True(seedResult.IsSuccess, seedResult.Error?.Message);
        Assert.True(await Fixture.ScalarAsync<long>("select count(*) from identity.memberships") > 0);
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries"));
    }

    [Fact]
    public async Task AuditEntry_CorrelationId_MatchesTheOneSetOnTheScope()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-11@nile.test", StaffRole.Owner);
        const string correlationId = "audit-trail-test-11";

        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(
            owner.Actor, new CreateSubjectCommand("Mathematics"), correlationId);
        Assert.True(created.IsSuccess);

        Assert.Equal(
            correlationId,
            await Fixture.ScalarAsync<string>($"select correlation_id from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
    }
}
