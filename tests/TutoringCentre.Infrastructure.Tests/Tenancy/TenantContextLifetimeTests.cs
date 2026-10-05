using Microsoft.EntityFrameworkCore;
using Npgsql;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Task 21.2: app.current_centre's lifetime — set per transaction, read back through a real dispatched request.</summary>
public sealed class TenantContextLifetimeTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Query_AsNileActor_SeesNilesId()
    {
        var result = await Fixture.QueryAsAsync<ReadCurrentCentreSettingQuery, string>(
            StaffActorIn(NileCentreId), new ReadCurrentCentreSettingQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(NileCentreId.ToString(), result.Value);
    }

    [Fact]
    public async Task Query_AsMaadiActor_SeesMaadisId()
    {
        var result = await Fixture.QueryAsAsync<ReadCurrentCentreSettingQuery, string>(
            StaffActorIn(MaadiCentreId), new ReadCurrentCentreSettingQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(MaadiCentreId.ToString(), result.Value);
    }

    [Fact]
    public async Task Query_AsActorWithoutACentre_SeesEmptyString()
    {
        var result = await Fixture.QueryAsAsync<ReadCurrentCentreSettingQuery, string>(
            new AnonymousActor(), new ReadCurrentCentreSettingQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Value);
    }

    [Fact]
    public async Task Dispatch_NileThenNoCentre_OnTheSameConnection_SecondSeesEmpty()
    {
        // One already-open connection, shared (never closed) across both requests below, via UseNpgsql(DbConnection):
        // the one guarantee a pool size setting can only ever approximate. UnitOfWork is instantiated directly
        // (InternalsVisibleTo) rather than resolved through a pool, so "Maximum Pool Size=1" never has to be relied
        // on to force reuse — this is what "a large pool hides the bug" is really about.
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();

        var nileValue = await ReadSettingThroughUnitOfWorkAsync(connection, StaffActorIn(NileCentreId));
        Assert.Equal(NileCentreId.ToString(), nileValue);

        // A second request's own BeginAsync would always overwrite the setting with its own value before reading
        // it back, passing even if Nile's value had wrongly survived the first request's commit — so what actually
        // proves the setting is transaction-local (not session-local) is reading it raw first, before anything
        // explicitly sets it again.
        await using (var checkTransaction = await connection.BeginTransactionAsync())
        {
            await using var checkCommand = new NpgsqlCommand(
                "select nullif(current_setting('app.current_centre', true), '') is null", connection, checkTransaction);
            var selectsNoCentre = (bool)(await checkCommand.ExecuteScalarAsync())!;
            await checkTransaction.RollbackAsync();

            Assert.True(selectsNoCentre, "Expected Nile's committed transaction not to leave a value behind.");
        }

        // And the ordinary case the task describes: a second, real request for an actor without a centre, on the
        // same connection, still correctly sees empty.
        var noCentreValue = await ReadSettingThroughUnitOfWorkAsync(connection, new AnonymousActor());
        Assert.Equal(string.Empty, noCentreValue);
    }

    [Fact]
    public async Task AfterAFailedCommandRollsBack_ANewTransactionOnTheSameConnection_SeesNoValue()
    {
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();

        await using (var db = OpenAppDbContext(connection, StaffActorIn(NileCentreId)))
        {
            var unitOfWork = new UnitOfWork(db);
            await unitOfWork.BeginAsync(readOnly: false, CancellationToken.None);
            var result = await new AlwaysFailingCommandHandler().HandleAsync(new AlwaysFailingCommand(), CancellationToken.None);
            Assert.True(result.IsFailure);
            await unitOfWork.RollbackAsync(CancellationToken.None); // what Dispatcher.SendAsync does on failure
        }

        await using var transaction = await connection.BeginTransactionAsync();
        await using var command = new NpgsqlCommand(
            "select nullif(current_setting('app.current_centre', true), '') is null", connection, transaction);
        var selectsNoCentre = (bool)(await command.ExecuteScalarAsync())!;
        await transaction.RollbackAsync();

        Assert.True(selectsNoCentre, "Expected the rolled-back transaction not to leave a value behind.");
    }

    [Fact]
    public async Task Dispatch_AsCommandOrQuery_BothSeeTheSetting()
    {
        var commandResult = await Fixture.SendAsAsync<ReadCurrentCentreSettingCommand, string>(
            StaffActorIn(NileCentreId), new ReadCurrentCentreSettingCommand());
        var queryResult = await Fixture.QueryAsAsync<ReadCurrentCentreSettingQuery, string>(
            StaffActorIn(NileCentreId), new ReadCurrentCentreSettingQuery());

        Assert.Equal(NileCentreId.ToString(), commandResult.Value);
        Assert.Equal(NileCentreId.ToString(), queryResult.Value);
    }

    private static async Task<string> ReadSettingThroughUnitOfWorkAsync(NpgsqlConnection connection, Actor actor)
    {
        await using var db = OpenAppDbContext(connection, actor);
        var unitOfWork = new UnitOfWork(db);
        await unitOfWork.BeginAsync(readOnly: true, CancellationToken.None);
        var value = await TenantSettingReader.ReadAsync(db, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);
        return value;
    }

    /// <summary>An AppDbContext over an externally-owned, already-open connection: disposing it never closes
    /// <paramref name="connection"/>, so the same physical connection carries into the next call.</summary>
    private static AppDbContext OpenAppDbContext(NpgsqlConnection connection, Actor actor)
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(actor);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connection).UseSnakeCaseNamingConvention();
        return new AppDbContext(optionsBuilder.Options, actorContext);
    }

    private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);
}
