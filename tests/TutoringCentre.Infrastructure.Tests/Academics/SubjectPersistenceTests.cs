using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 23.6: Subject exercised through the real AppDbContext, interceptors and unit of work, as different
/// actors, using the test-only requests in <see cref="SubjectTestRequests"/> — the repository and read service
/// that would normally carry this arrive on Day 25.
/// </summary>
public sealed class SubjectPersistenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task NileActorAddsMathematics_PersistsWithNileCentreCreatedAtNoUpdatedAtActiveStatus()
    {
        var result = await Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"""
                select count(*) from academics.subjects
                where id = '{result.Value}' and centre_id = '{NileCentreId}' and status = 'active'
                  and created_at is not null and updated_at is null
                """));
    }

    [Fact]
    public async Task MaadiActorListsSubjects_DoesNotContainNilesSubject()
    {
        await Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"));

        var result = await Fixture.QueryAsAsync<ListSubjectNamesQuery, List<string>>(StaffActorIn(MaadiCentreId), new ListSubjectNamesQuery());

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain("Mathematics", result.Value);
    }

    [Fact]
    public async Task NileActorAddsWhitespaceVariantOfMathematics_TranslatedToNameTakenConflict()
    {
        await Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"));

        // Day 25: the save-time unique violation is now translated to a Result, not an exception.
        var result = await Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(NileCentreId), new AddSubjectCommand(NileCentreId, " mathematics "));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from academics.subjects"));
    }

    [Fact]
    public async Task MaadiActorAddsMathematics_Succeeds()
    {
        await Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"));

        var result = await Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(MaadiCentreId), new AddSubjectCommand(MaadiCentreId, "Mathematics"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task StatusOutsideAllowedValuesWrittenByRawSql_ChecksViolation()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.subjects (id, centre_id, name, normalized_name, status, created_at)
            values (@id, @centre_id, 'Mathematics', 'MATHEMATICS', 'retired', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23514", exception.SqlState); // check_violation
        Assert.Equal("ck_subjects_status", exception.ConstraintName);
    }

    [Fact]
    public async Task Rename_SetsUpdatedAtAndChangesVersion()
    {
        var subjectId = (await Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"))).Value;
        var versionBeforeRename = await Fixture.ScalarAsync<string>($"select xmin::text from academics.subjects where id = '{subjectId}'");

        var result = await Fixture.SendAsAsync<RenameSubjectCommand, Unit>(
            StaffActorIn(NileCentreId), new RenameSubjectCommand(subjectId, "Applied Mathematics"));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{subjectId}' and name = 'Applied Mathematics' and updated_at is not null"));
        var versionAfterRename = await Fixture.ScalarAsync<string>($"select xmin::text from academics.subjects where id = '{subjectId}'");
        Assert.NotEqual(versionBeforeRename, versionAfterRename);
    }

    [Fact]
    public async Task ConcurrentRenameFromTwoContexts_SecondSaveThrowsConcurrencyException()
    {
        var subjectId = (await Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(NileCentreId), new AddSubjectCommand(NileCentreId, "Mathematics"))).Value;

        await using var scope1 = Fixture.Services.CreateAsyncScope();
        await using var scope2 = Fixture.Services.CreateAsyncScope();

        var (unitOfWork1, db1) = await BeginScopeAsync(scope1, StaffActorIn(NileCentreId));
        var (unitOfWork2, db2) = await BeginScopeAsync(scope2, StaffActorIn(NileCentreId));

        var subject1 = await db1.Set<Subject>().SingleAsync(s => s.Id == subjectId);
        var subject2 = await db2.Set<Subject>().SingleAsync(s => s.Id == subjectId);

        Assert.True(subject1.Rename("Applied Mathematics").IsSuccess);
        await db1.SaveChangesAsync();
        await unitOfWork1.CommitAsync(CancellationToken.None);

        Assert.True(subject2.Rename("Pure Mathematics").IsSuccess);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => db2.SaveChangesAsync());

        await unitOfWork2.RollbackAsync(CancellationToken.None);
    }

    [Fact]
    public async Task NileActorAddsSubjectConstructedWithMaadiCentre_TenantViolationAndNothingStored()
    {
        await Assert.ThrowsAsync<TenantViolationException>(() =>
            Fixture.SendAsAsync<AddSubjectCommand, Guid>(StaffActorIn(NileCentreId), new AddSubjectCommand(MaadiCentreId, "Chemistry")));

        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from academics.subjects"));
    }

    private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);

    private static async Task<(IUnitOfWork UnitOfWork, AppDbContext Db)> BeginScopeAsync(AsyncServiceScope scope, Actor actor)
    {
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await unitOfWork.BeginAsync(readOnly: false, CancellationToken.None);
        return (unitOfWork, db);
    }
}
