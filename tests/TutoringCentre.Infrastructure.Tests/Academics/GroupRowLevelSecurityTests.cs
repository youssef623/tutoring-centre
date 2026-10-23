using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 44.14: the decisive row-level security checks on the real academics.groups table, as the real runtime
/// role, independent of EF or the application pipeline. Rows are seeded as the superuser so the policy under
/// test cannot have biased them.
/// </summary>
public sealed class GroupRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task NoSetting_SelectCount_IsZero()
    {
        await SeedGroupAsync(NileCentreId, "Grade 10 A");
        await SeedGroupAsync(MaadiCentreId, "Grade 10 A");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        Assert.Equal(0, await ScalarAsync<long>(connection, transaction, "select count(*) from academics.groups"));
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await SeedGroupAsync(NileCentreId, "Grade 10 A");
        await SeedGroupAsync(NileCentreId, "Grade 10 B");
        await SeedGroupAsync(MaadiCentreId, "Grade 10 A");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        Assert.Equal(2, await ScalarAsync<long>(connection, transaction, "select count(*) from academics.groups"));
        Assert.Equal(
            0,
            await ScalarAsync<long>(connection, transaction, $"select count(*) from academics.groups where centre_id <> '{NileCentreId}'"));
    }

    [Fact]
    public async Task SettingNile_InsertWithMaadiCentre_Throws42501()
    {
        var maadiSubjectId = await Fixture.SeedSubjectAsync(MaadiCentreId, "Mathematics");
        var maadiTeacherId = await Fixture.SeedTeacherAsync(MaadiCentreId, "Mona Said");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var command = new NpgsqlCommand(
            """
            insert into academics.groups (id, centre_id, name, normalized_name, subject_id, teacher_id, capacity, status, created_at)
            values (@id, @centre_id, 'Smuggled', 'SMUGGLED', @subject_id, @teacher_id, 20, 'active', now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("subject_id", maadiSubjectId);
        command.Parameters.AddWithValue("teacher_id", maadiTeacherId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateMaadiGroupById_AffectsZeroRows()
    {
        var maadiGroupId = await SeedGroupAsync(MaadiCentreId, "Grade 10 A");

        await using (var connection = await OpenAppConnectionAsync())
        {
            await using var transaction = await connection.BeginTransactionAsync();
            await SetCentreAsync(connection, transaction, NileCentreId);

            await using var update = new NpgsqlCommand("update academics.groups set capacity = 99 where id = @id", connection, transaction);
            update.Parameters.AddWithValue("id", maadiGroupId);
            Assert.Equal(0, await update.ExecuteNonQueryAsync());

            await transaction.CommitAsync();
        }

        Assert.Equal(20, await Fixture.ScalarAsync<int>($"select capacity from academics.groups where id = '{maadiGroupId}'"));
    }

    [Fact]
    public async Task SettingNile_DeleteOwnGroup_Throws42501BecauseDeleteIsNotGranted()
    {
        var nileGroupId = await SeedGroupAsync(NileCentreId, "Grade 10 A");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var delete = new NpgsqlCommand("delete from academics.groups where id = @id", connection, transaction);
        delete.Parameters.AddWithValue("id", nileGroupId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => delete.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    private async Task<Guid> SeedGroupAsync(Guid centreId, string name)
    {
        var subjectId = await Fixture.SeedSubjectAsync(centreId, $"Subject for {name} {Guid.CreateVersion7()}");
        var teacherId = await Fixture.SeedTeacherAsync(centreId, "Seeded Teacher");
        return await Fixture.SeedGroupAsync(centreId, name, subjectId, teacherId);
    }

    private async Task<NpgsqlConnection> OpenAppConnectionAsync()
    {
        var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, Guid centreId)
    {
        await using var command = new NpgsqlCommand("select set_config('app.current_centre', @value, true)", connection, transaction);
        command.Parameters.AddWithValue("value", centreId.ToString());
        await command.ExecuteScalarAsync();
    }

    private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        return (T)(await command.ExecuteScalarAsync())!;
    }
}
