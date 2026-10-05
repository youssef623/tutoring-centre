using Npgsql;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>
/// Raw-SQL evidence of what the runtime role can and cannot do (Day 19). Valid evidence only because the fixture
/// itself runs the app under test as tutoring_app — a test connecting as a superuser could never fail these.
/// Every denial asserts the SQLSTATE (42501, permission denied), not merely that something was thrown.
/// </summary>
public sealed class RuntimeRolePrivilegeTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task CurrentUser_ThroughTheAppConnection_IsTutoringAppAndUnprivileged()
    {
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "select current_user, rolsuper, rolbypassrls from pg_roles where rolname = current_user",
            connection);
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        Assert.Equal(DatabaseRoles.App, reader.GetString(0));
        Assert.False(reader.GetBoolean(1));
        Assert.False(reader.GetBoolean(2));
    }

    [Fact]
    public async Task CreateTable_InPlatformSchema_IsDenied() =>
        await AssertDeniedAsync(Fixture.AppConnectionString, "create table platform.probe (id uuid primary key)");

    [Fact]
    public async Task AlterTable_Centres_IsDenied() =>
        await AssertDeniedAsync(Fixture.AppConnectionString, "alter table platform.centres add column probe text");

    [Fact]
    public async Task Delete_FromCentres_IsDenied() =>
        await AssertDeniedAsync(Fixture.AppConnectionString, "delete from platform.centres");

    [Fact]
    public async Task Insert_IntoMigrationsHistory_IsDenied() =>
        await AssertDeniedAsync(
            Fixture.AppConnectionString,
            "insert into platform.__ef_migrations_history (migration_id, product_version) values ('x', 'x')");

    [Fact]
    public async Task SelectAndInsert_OnMemberships_Succeed()
    {
        var centreId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await SeedCentreAndUserAsync(centreId, userId);

        await using var appConnection = new NpgsqlConnection(Fixture.AppConnectionString);
        await appConnection.OpenAsync();

        var membershipId = Guid.NewGuid();
        await using var insertCommand = new NpgsqlCommand(
            """
            insert into identity.memberships (id, user_id, centre_id, role, status, created_at)
            values (@id, @userId, @centreId, 'teacher', 'active', now())
            """,
            appConnection);
        insertCommand.Parameters.AddWithValue("id", membershipId);
        insertCommand.Parameters.AddWithValue("userId", userId);
        insertCommand.Parameters.AddWithValue("centreId", centreId);
        await insertCommand.ExecuteNonQueryAsync();

        await using var selectCommand = new NpgsqlCommand(
            "select count(*) from identity.memberships where id = @id",
            appConnection);
        selectCommand.Parameters.AddWithValue("id", membershipId);
        var count = (long)(await selectCommand.ExecuteScalarAsync())!;

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task AllPlatformAndIdentityTables_AreOwnedByTutoringOwner()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "select distinct tableowner from pg_tables where schemaname in ('platform', 'identity')",
            connection);
        await using var reader = await command.ExecuteReaderAsync();

        var owners = new List<string>();
        while (await reader.ReadAsync())
        {
            owners.Add(reader.GetString(0));
        }

        Assert.Equal([DatabaseRoles.Owner], owners);
    }

    /// <summary>Inserts a centre and a user as the superuser, bypassing tutoring_app's restrictions, purely to satisfy memberships' foreign keys.</summary>
    private async Task SeedCentreAndUserAsync(Guid centreId, Guid userId)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();

        await using var centreCommand = new NpgsqlCommand(
            """
            insert into platform.centres (id, name, slug, time_zone_id, default_locale, created_at)
            values (@id, 'Probe Centre', @slug, 'Africa/Cairo', 'en', now())
            """,
            connection);
        centreCommand.Parameters.AddWithValue("id", centreId);
        centreCommand.Parameters.AddWithValue("slug", $"probe-{centreId}");
        await centreCommand.ExecuteNonQueryAsync();

        await using var userCommand = new NpgsqlCommand(
            """
            insert into identity.users
                (id, display_name, preferred_locale, must_change_password, email_confirmed, phone_number_confirmed, two_factor_enabled, lockout_enabled, access_failed_count)
            values (@id, 'Probe User', 'en', false, false, false, false, true, 0)
            """,
            connection);
        userCommand.Parameters.AddWithValue("id", userId);
        await userCommand.ExecuteNonQueryAsync();
    }

    private static async Task AssertDeniedAsync(string connectionString, string sql)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }
}
