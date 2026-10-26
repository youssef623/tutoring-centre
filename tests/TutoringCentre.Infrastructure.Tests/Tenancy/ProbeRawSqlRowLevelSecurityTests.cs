using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.4: proves the row-level security policy itself (Task 21.3) against raw connections as tutoring_app,
/// independent of EF, UnitOfWork or the application pipeline — the database-level guarantee the other layers
/// depend on actually existing.
/// </summary>
public sealed class ProbeRawSqlRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task NoSetting_SelectCount_IsZero()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from probe.tenant_probes");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(NileCentreId, "nile-2");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction);

        Assert.Equal(2, centreIds.Count);
        Assert.All(centreIds, id => Assert.Equal(NileCentreId, id));
    }

    [Fact]
    public async Task SettingMaadi_SelectsOnlyMaadiRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-2");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, MaadiCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction);

        Assert.Equal(2, centreIds.Count);
        Assert.All(centreIds, id => Assert.Equal(MaadiCentreId, id));
    }

    [Fact]
    public async Task SettingNile_InsertWithMaadiCentre_Throws42501()
    {
        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            "insert into probe.tenant_probes (id, centre_id, label) values (@id, @centre_id, @label)", connection, transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("label", "smuggled");

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateCentreIdToMaadiOnNileRow_Throws42501()
    {
        var probeId = await Fixture.SeedProbeAsync(NileCentreId, "nile-1");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            "update probe.tenant_probes set centre_id = @maadi where id = @id", connection, transaction);
        command.Parameters.AddWithValue("maadi", MaadiCentreId);
        command.Parameters.AddWithValue("id", probeId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateAndDeleteOnMaadiRow_AffectZeroRowsAndLeaveItIntact()
    {
        var probeId = await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using (var connection = await OpenAppConnectionAsync())
        {
            await using var transaction = await connection.BeginTransactionAsync();
            await SetCentreAsync(connection, transaction, NileCentreId.ToString());

            await using (var update = new NpgsqlCommand("update probe.tenant_probes set label = 'changed' where id = @id", connection, transaction))
            {
                update.Parameters.AddWithValue("id", probeId);
                Assert.Equal(0, await update.ExecuteNonQueryAsync());
            }

            await using (var delete = new NpgsqlCommand("delete from probe.tenant_probes where id = @id", connection, transaction))
            {
                delete.Parameters.AddWithValue("id", probeId);
                Assert.Equal(0, await delete.ExecuteNonQueryAsync());
            }

            await transaction.CommitAsync();
        }

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>($"select count(*) from probe.tenant_probes where id = '{probeId}' and label = 'maadi-1'"));
    }

    [Fact]
    public async Task NoSetting_Insert_Throws42501()
    {
        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        await using var command = new NpgsqlCommand(
            "insert into probe.tenant_probes (id, centre_id, label) values (@id, @centre_id, @label)", connection, transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("label", "no-setting");

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task SettingIsNotAUuid_StatementErrorsAndReturnsNoRows()
    {
        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, "not-a-uuid");

        await using var command = new NpgsqlCommand("select count(*) from probe.tenant_probes", connection, transaction);
        await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteScalarAsync());
    }

    [Fact]
    public async Task AsOwnerWithNoSetting_SelectCount_IsZero()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");

        await using var connection = new NpgsqlConnection(Fixture.OwnerConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from probe.tenant_probes");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task AsAppWithRowSecurityOff_SelectErrorsRatherThanReturningUnfilteredRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using (var setRowSecurity = new NpgsqlCommand("set row_security = off", connection, transaction))
        {
            await setRowSecurity.ExecuteNonQueryAsync();
        }

        await using var select = new NpgsqlCommand("select * from probe.tenant_probes", connection, transaction);
        await Assert.ThrowsAsync<PostgresException>(() => select.ExecuteReaderAsync());
    }

    [Fact]
    public async Task SettingFromACommittedTransaction_IsAbsentInTheNextTransactionOnTheSameConnection()
    {
        await using var connection = await OpenAppConnectionAsync();

        await using (var transaction1 = await connection.BeginTransactionAsync())
        {
            await SetCentreAsync(connection, transaction1, NileCentreId.ToString());
            await transaction1.CommitAsync();
        }

        // Not necessarily SQL NULL: PostgreSQL registers a never-before-set custom GUC on its first COMMIT, so it
        // can revert to '' rather than staying undefined — asserting the exact expression the policy evaluates
        // (Task 21.3) proves "absent" the way that matters here: no centre selects any row, whichever it reads as.
        await using var transaction2 = await connection.BeginTransactionAsync();
        var selectsNoCentre = await ScalarAsync<bool>(
            connection, transaction2, "select nullif(current_setting('app.current_centre', true), '') is null");
        await transaction2.RollbackAsync();

        Assert.True(selectsNoCentre, "Expected the committed transaction's centre setting not to carry over.");
    }

    private async Task<NpgsqlConnection> OpenAppConnectionAsync()
    {
        var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string value)
    {
        await using var command = new NpgsqlCommand("select set_config('app.current_centre', @value, true)", connection, transaction);
        command.Parameters.AddWithValue("value", value);
        await command.ExecuteScalarAsync();
    }

    private static async Task<List<Guid>> SelectCentreIdsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        await using var command = new NpgsqlCommand("select centre_id from probe.tenant_probes", connection, transaction);
        await using var reader = await command.ExecuteReaderAsync();
        var centreIds = new List<Guid>();
        while (await reader.ReadAsync())
        {
            centreIds.Add(reader.GetGuid(0));
        }

        return centreIds;
    }

    private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        return (T)(await command.ExecuteScalarAsync())!;
    }
}
