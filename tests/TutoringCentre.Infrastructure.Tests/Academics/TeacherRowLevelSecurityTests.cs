using Npgsql;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Identity;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 43.14: repeats the decisive row-level security checks on the real academics.teachers table, as the
/// real runtime role, independent of EF or the application pipeline, plus the two integrity checks that are
/// specific to this table's account link: the composite foreign key into identity.memberships (23503, a
/// Maadi-only user is simply not a valid (centre_id, user_id) pair for Nile) and the partial unique index on
/// (centre_id, user_id) (23505, the same user linked to a second Nile teacher). Both integrity checks run as
/// the superuser — row-level security is not what is under test there, the raw constraint is.
/// </summary>
public sealed class TeacherRowLevelSecurityTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";
    private const string ForeignKeyViolation = "23503";
    private const string UniqueViolation = "23505";

    [Fact]
    public async Task NoSetting_SelectCount_IsZero()
    {
        await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");
        await Fixture.SeedTeacherAsync(MaadiCentreId, "Mona Said");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.teachers");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");
        await Fixture.SeedTeacherAsync(NileCentreId, "Youssef Kamal");
        await Fixture.SeedTeacherAsync(MaadiCentreId, "Mona Said");

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
        await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");
        await Fixture.SeedTeacherAsync(MaadiCentreId, "Mona Said");
        await Fixture.SeedTeacherAsync(MaadiCentreId, "Rania Adel");

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
            insert into academics.teachers (id, centre_id, full_name, search_name, status, created_at)
            values (@id, @centre_id, @full_name, @search_name, 'active', now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("full_name", "Smuggled");
        command.Parameters.AddWithValue("search_name", "SMUGGLED");

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateNileTeacherCentreIdToMaadi_Throws42501()
    {
        var teacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            "update academics.teachers set centre_id = @maadi where id = @id", connection, transaction);
        command.Parameters.AddWithValue("maadi", MaadiCentreId);
        command.Parameters.AddWithValue("id", teacherId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateMaadiTeacherById_AffectsZeroRows()
    {
        var teacherId = await Fixture.SeedTeacherAsync(MaadiCentreId, "Mona Said");

        await using (var connection = await OpenAppConnectionAsync())
        {
            await using var transaction = await connection.BeginTransactionAsync();
            await SetCentreAsync(connection, transaction, NileCentreId.ToString());

            await using var update = new NpgsqlCommand(
                "update academics.teachers set full_name = 'Changed' where id = @id", connection, transaction);
            update.Parameters.AddWithValue("id", teacherId);
            Assert.Equal(0, await update.ExecuteNonQueryAsync());

            await transaction.CommitAsync();
        }

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>($"select count(*) from academics.teachers where id = '{teacherId}' and full_name = 'Mona Said'"));
    }

    [Fact]
    public async Task AsSuperuser_InsertTeacherWithNileCentreAndMaadiOnlyUser_Throws23503ForeignKeyViolation()
    {
        var maadiOnlyTeacher = await CreateStaffAsync(MaadiCentreId, "maadi-only-rls1@maadi.test", StaffRole.Teacher);

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.teachers (id, centre_id, full_name, search_name, user_id, status, created_at)
            values (@id, @centre_id, @full_name, @search_name, @user_id, 'active', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("full_name", "Smuggled Link");
        command.Parameters.AddWithValue("search_name", "SMUGGLED LINK");
        command.Parameters.AddWithValue("user_id", maadiOnlyTeacher.UserId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(ForeignKeyViolation, exception.SqlState);
    }

    [Fact]
    public async Task AsSuperuser_LinkSameNileUserToASecondNileTeacher_Throws23505UniqueViolation()
    {
        var nileTeacherMember = await CreateStaffAsync(NileCentreId, "nile-teacher-rls2@nile.test", StaffRole.Teacher);
        await Fixture.SeedTeacherAsync(NileCentreId, "First Link", nileTeacherMember.UserId);

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.teachers (id, centre_id, full_name, search_name, user_id, status, created_at)
            values (@id, @centre_id, @full_name, @search_name, @user_id, 'active', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("full_name", "Second Link");
        command.Parameters.AddWithValue("search_name", "SECOND LINK");
        command.Parameters.AddWithValue("user_id", nileTeacherMember.UserId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(UniqueViolation, exception.SqlState);
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
        await using var command = new NpgsqlCommand("select centre_id from academics.teachers", connection, transaction);
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
