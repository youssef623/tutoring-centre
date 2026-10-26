using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 23.4: repeats the decisive row-level security checks (Task 21.4) on the real academics.subjects table,
/// as the real runtime role, independent of EF or the application pipeline — the direct evidence that this
/// table, not just the probe, is actually wired to the policy. Test 7 is specific to subjects: tutoring_app has
/// no DELETE privilege on it at all (Task 23.3), so a delete is refused before row-level security ever runs.
/// </summary>
public sealed class SubjectRawSqlRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task NoSetting_SelectCount_IsZero()
    {
        await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        await Fixture.SeedSubjectAsync(MaadiCentreId, "Physics");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.subjects");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        await Fixture.SeedSubjectAsync(NileCentreId, "Chemistry");
        await Fixture.SeedSubjectAsync(MaadiCentreId, "Physics");

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
        await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        await Fixture.SeedSubjectAsync(MaadiCentreId, "Physics");
        await Fixture.SeedSubjectAsync(MaadiCentreId, "Biology");

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
            """
            insert into academics.subjects (id, centre_id, name, normalized_name, status, created_at)
            values (@id, @centre_id, @name, @normalized_name, 'active', now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("name", "Smuggled");
        command.Parameters.AddWithValue("normalized_name", "SMUGGLED");

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateNileSubjectCentreIdToMaadi_Throws42501()
    {
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            "update academics.subjects set centre_id = @maadi where id = @id", connection, transaction);
        command.Parameters.AddWithValue("maadi", MaadiCentreId);
        command.Parameters.AddWithValue("id", subjectId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateMaadiSubjectById_AffectsZeroRows()
    {
        var subjectId = await Fixture.SeedSubjectAsync(MaadiCentreId, "Physics");

        await using (var connection = await OpenAppConnectionAsync())
        {
            await using var transaction = await connection.BeginTransactionAsync();
            await SetCentreAsync(connection, transaction, NileCentreId.ToString());

            await using var update = new NpgsqlCommand(
                "update academics.subjects set name = 'Changed' where id = @id", connection, transaction);
            update.Parameters.AddWithValue("id", subjectId);
            Assert.Equal(0, await update.ExecuteNonQueryAsync());

            await transaction.CommitAsync();
        }

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{subjectId}' and name = 'Physics'"));
    }

    [Fact]
    public async Task Delete_AnySubject_Throws42501NoPrivilege()
    {
        var subjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand("delete from academics.subjects where id = @id", connection, transaction);
        command.Parameters.AddWithValue("id", subjectId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task AsOwnerWithNoSetting_SelectCount_IsZero()
    {
        await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");

        await using var connection = new NpgsqlConnection(Fixture.OwnerConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.subjects");

        Assert.Equal(0, count);
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
        await using var command = new NpgsqlCommand("select centre_id from academics.subjects", connection, transaction);
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
