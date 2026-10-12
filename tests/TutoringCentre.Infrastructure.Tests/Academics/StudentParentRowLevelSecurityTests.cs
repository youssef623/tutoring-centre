using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 38.4: repeats the decisive row-level security checks on the three new tables, as the real runtime
/// role, independent of EF or the application pipeline. Parents and students have no DELETE privilege at all
/// (Task 38.3); student_parents does, since links are removed and the removal is audited.
/// </summary>
public sealed class StudentParentRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task NoSetting_Parents_SelectCountIsZero()
    {
        await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        await Fixture.SeedParentAsync(MaadiCentreId, "Laila Said", "+201112345678");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.parents");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task NoSetting_Students_SelectCountIsZero()
    {
        await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        await Fixture.SeedStudentAsync(MaadiCentreId, "Sara Adel");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.students");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task NoSetting_StudentParents_SelectCountIsZero()
    {
        var nileParent = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, nileStudent, nileParent, isPrimary: true);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.student_parents");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task SettingNile_Parents_SelectsOnlyNileRows()
    {
        await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        await Fixture.SeedParentAsync(NileCentreId, "Amal Fathy", "+201212345678");
        await Fixture.SeedParentAsync(MaadiCentreId, "Laila Said", "+201112345678");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction, "academics.parents");

        Assert.Equal(2, centreIds.Count);
        Assert.All(centreIds, id => Assert.Equal(NileCentreId, id));
    }

    [Fact]
    public async Task SettingNile_Students_SelectsOnlyNileRows()
    {
        await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        await Fixture.SeedStudentAsync(NileCentreId, "Mona Samir");
        await Fixture.SeedStudentAsync(MaadiCentreId, "Sara Adel");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction, "academics.students");

        Assert.Equal(2, centreIds.Count);
        Assert.All(centreIds, id => Assert.Equal(NileCentreId, id));
    }

    [Fact]
    public async Task SettingNile_StudentParents_SelectsOnlyNileRows()
    {
        var nileParent = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, nileStudent, nileParent, isPrimary: true);

        var maadiParent = await Fixture.SeedParentAsync(MaadiCentreId, "Laila Said", "+201112345678");
        var maadiStudent = await Fixture.SeedStudentAsync(MaadiCentreId, "Sara Adel");
        await Fixture.SeedStudentParentLinkAsync(MaadiCentreId, maadiStudent, maadiParent, isPrimary: true);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction, "academics.student_parents");

        Assert.Equal(NileCentreId, Assert.Single(centreIds));
    }

    [Fact]
    public async Task SettingNile_InsertParentWithMaadiCentre_Throws42501()
    {
        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            """
            insert into academics.parents (id, centre_id, full_name, search_name, phone, created_at)
            values (@id, @centre_id, 'Smuggled', 'SMUGGLED', '+201012345678', now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_InsertStudentWithMaadiCentre_Throws42501()
    {
        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            """
            insert into academics.students (id, centre_id, full_name, search_name, status, links_revision, created_at)
            values (@id, @centre_id, 'Smuggled', 'SMUGGLED', 'active', 0, now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_InsertStudentParentWithMaadiCentre_Throws42501()
    {
        var nileParent = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            """
            insert into academics.student_parents (id, centre_id, student_id, parent_id, relationship, is_primary, created_at)
            values (@id, @centre_id, @student_id, @parent_id, 'father', true, now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("student_id", nileStudent);
        command.Parameters.AddWithValue("parent_id", nileParent);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task Delete_AnyParent_Throws42501NoPrivilege()
    {
        var parentId = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand("delete from academics.parents where id = @id", connection, transaction);
        command.Parameters.AddWithValue("id", parentId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task Delete_AnyStudent_Throws42501NoPrivilege()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand("delete from academics.students where id = @id", connection, transaction);
        command.Parameters.AddWithValue("id", studentId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_DeleteNileLink_AffectsOneRow()
    {
        var nileParent = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        var secondParent = await Fixture.SeedParentAsync(NileCentreId, "Amal Fathy", "+201212345678");
        var linkId = await Fixture.SeedStudentParentLinkAsync(NileCentreId, nileStudent, secondParent, isPrimary: false);
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, nileStudent, nileParent, isPrimary: true);

        await using (var connection = await OpenAppConnectionAsync())
        {
            await using var transaction = await connection.BeginTransactionAsync();
            await SetCentreAsync(connection, transaction, NileCentreId.ToString());

            await using var command = new NpgsqlCommand("delete from academics.student_parents where id = @id", connection, transaction);
            command.Parameters.AddWithValue("id", linkId);
            Assert.Equal(1, await command.ExecuteNonQueryAsync());

            await transaction.CommitAsync();
        }

        Assert.Equal(0, await Fixture.ScalarAsync<long>($"select count(*) from academics.student_parents where id = '{linkId}'"));
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

    private static async Task<List<Guid>> SelectCentreIdsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string table)
    {
        await using var command = new NpgsqlCommand($"select centre_id from {table}", connection, transaction);
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
