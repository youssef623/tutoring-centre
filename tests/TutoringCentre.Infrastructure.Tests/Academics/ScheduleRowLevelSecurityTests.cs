using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: the decisive row-level security checks on academics.schedule_rules and academics.class_sessions, as
/// the real runtime role, independent of EF or the application pipeline (the style of Tasks 38.4 and 47.5). Rows
/// are seeded as the superuser so the policy under test cannot have biased them.
/// </summary>
public sealed class ScheduleRowLevelSecurityTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Theory]
    [InlineData("schedule_rules")]
    [InlineData("class_sessions")]
    public async Task NoSetting_SelectCount_IsZero(string table)
    {
        await SeedSessionAsync(NileCentreId);
        await SeedSessionAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        Assert.Equal(0, await ScalarAsync<long>(connection, transaction, $"select count(*) from academics.{table}"));
    }

    [Theory]
    [InlineData("schedule_rules")]
    [InlineData("class_sessions")]
    public async Task SettingNile_SelectsOnlyNileRows(string table)
    {
        await SeedSessionAsync(NileCentreId);
        await SeedSessionAsync(NileCentreId);
        await SeedSessionAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        Assert.Equal(2, await ScalarAsync<long>(connection, transaction, $"select count(*) from academics.{table}"));
        Assert.Equal(
            0,
            await ScalarAsync<long>(connection, transaction, $"select count(*) from academics.{table} where centre_id <> '{NileCentreId}'"));
    }

    [Fact]
    public async Task SettingNile_InsertRuleWithMaadiCentre_Throws42501()
    {
        var maadi = await SeedReferencesAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var command = new NpgsqlCommand(
            """
            insert into academics.schedule_rules (id, centre_id, group_id, day_of_week, start_time, duration_minutes, effective_from, created_at)
            values (@id, @centre_id, @group_id, 6, '18:00', 90, '2027-03-01', now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("group_id", maadi.GroupId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_InsertSessionWithMaadiCentre_Throws42501()
    {
        var maadi = await SeedReferencesAsync(MaadiCentreId);
        var maadiRuleId = await SeedRuleAsync(MaadiCentreId, maadi.GroupId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var command = new NpgsqlCommand(
            """
            insert into academics.class_sessions
                (id, centre_id, group_id, teacher_id, schedule_rule_id, local_date, starts_at, ends_at, status, created_at)
            values (@id, @centre_id, @group_id, @teacher_id, @rule_id, '2027-03-06', '2027-03-06 16:00Z', '2027-03-06 17:30Z', 'scheduled', now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("group_id", maadi.GroupId);
        command.Parameters.AddWithValue("teacher_id", maadi.TeacherId);
        command.Parameters.AddWithValue("rule_id", maadiRuleId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Theory]
    [InlineData("schedule_rules")]
    [InlineData("class_sessions")]
    public async Task SettingNile_DeleteOwnRow_Throws42501BecauseDeleteIsNotGranted(string table)
    {
        var (ruleId, sessionId) = await SeedSessionAsync(NileCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var delete = new NpgsqlCommand($"delete from academics.{table} where id = @id", connection, transaction);
        delete.Parameters.AddWithValue("id", table == "schedule_rules" ? ruleId : sessionId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => delete.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    private async Task<(Guid TeacherId, Guid GroupId)> SeedReferencesAsync(Guid centreId)
    {
        var subjectId = await Fixture.SeedSubjectAsync(centreId, $"Subject {Guid.CreateVersion7()}");
        var teacherId = await Fixture.SeedTeacherAsync(centreId, "Seeded Teacher");
        var groupId = await Fixture.SeedGroupAsync(centreId, $"Group {Guid.CreateVersion7()}", subjectId, teacherId);
        return (teacherId, groupId);
    }

    /// <summary>One rule and one of its sessions for the centre, as the superuser.</summary>
    private async Task<(Guid RuleId, Guid SessionId)> SeedSessionAsync(Guid centreId)
    {
        var (teacherId, groupId) = await SeedReferencesAsync(centreId);
        var ruleId = await SeedRuleAsync(centreId, groupId);
        var sessionId = await InsertSessionAsync(new SessionRow(centreId, groupId, teacherId, ruleId, new DateOnly(2027, 3, 6), At(16), At(17, 30)));
        return (ruleId, sessionId);
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
