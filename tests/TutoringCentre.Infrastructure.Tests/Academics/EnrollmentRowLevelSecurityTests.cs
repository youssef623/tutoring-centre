using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 47.5: the decisive row-level security checks on the real academics.enrollments table, as the real
/// runtime role, independent of EF or the application pipeline. Rows are seeded as the superuser so the policy
/// under test cannot have biased them.
/// </summary>
public sealed class EnrollmentRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task NoSetting_SelectCount_IsZero()
    {
        await SeedEnrollmentAsync(NileCentreId);
        await SeedEnrollmentAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        Assert.Equal(0, await ScalarAsync<long>(connection, transaction, "select count(*) from academics.enrollments"));
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await SeedEnrollmentAsync(NileCentreId);
        await SeedEnrollmentAsync(NileCentreId);
        await SeedEnrollmentAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        Assert.Equal(2, await ScalarAsync<long>(connection, transaction, "select count(*) from academics.enrollments"));
        Assert.Equal(
            0,
            await ScalarAsync<long>(connection, transaction, $"select count(*) from academics.enrollments where centre_id <> '{NileCentreId}'"));
    }

    [Fact]
    public async Task SettingNile_InsertWithMaadiCentre_Throws42501()
    {
        var (maadiStudentId, maadiGroupId) = await SeedReferencesAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var command = new NpgsqlCommand(
            """
            insert into academics.enrollments (id, centre_id, student_id, group_id, status, enrolled_at, created_at)
            values (@id, @centre_id, @student_id, @group_id, 'active', now(), now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("student_id", maadiStudentId);
        command.Parameters.AddWithValue("group_id", maadiGroupId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_DeleteOwnEnrollment_Throws42501BecauseDeleteIsNotGranted()
    {
        var nileEnrollmentId = await SeedEnrollmentAsync(NileCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var delete = new NpgsqlCommand("delete from academics.enrollments where id = @id", connection, transaction);
        delete.Parameters.AddWithValue("id", nileEnrollmentId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => delete.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    private async Task<(Guid StudentId, Guid GroupId)> SeedReferencesAsync(Guid centreId)
    {
        var studentId = await Fixture.SeedStudentAsync(centreId, "Ahmed Mahmoud");
        var subjectId = await Fixture.SeedSubjectAsync(centreId, $"Subject {Guid.CreateVersion7()}");
        var teacherId = await Fixture.SeedTeacherAsync(centreId, "Seeded Teacher");
        var groupId = await Fixture.SeedGroupAsync(centreId, $"Group {Guid.CreateVersion7()}", subjectId, teacherId);
        return (studentId, groupId);
    }

    private async Task<Guid> SeedEnrollmentAsync(Guid centreId)
    {
        var (studentId, groupId) = await SeedReferencesAsync(centreId);
        var id = Guid.CreateVersion7();
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.enrollments (id, centre_id, student_id, group_id, status, enrolled_at, created_at)
            values (@id, @centre_id, @student_id, @group_id, 'active', now(), now())
            """,
            connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("centre_id", centreId);
        command.Parameters.AddWithValue("student_id", studentId);
        command.Parameters.AddWithValue("group_id", groupId);
        await command.ExecuteNonQueryAsync();
        return id;
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
