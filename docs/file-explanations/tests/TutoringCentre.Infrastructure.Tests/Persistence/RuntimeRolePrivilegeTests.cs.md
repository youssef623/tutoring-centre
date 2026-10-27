# `tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Raw-SQL evidence of what the runtime role can and cannot do (Day 19). Valid evidence only because the fixture itself runs the app under test as tutoring_app — a test connecting as a superuser could never fail these. Every denial asserts the SQLSTATE (42501, permission denied), not merely that something was thrown.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 144 lines and 6,335 bytes. Source hash: `462c18548dbbd56fad89fc7b8a5d983b01b5440b7381d9c4a6882c6c03c5d3d7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Persistence;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L12): `public sealed class RuntimeRolePrivilegeTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L17): `public async Task CurrentUser_ThroughTheAppConnection_IsTutoringAppAndUnprivileged()`
- [Line 33](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L33): `public async Task CreateTable_InPlatformSchema_IsDenied() =>`
- [Line 37](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L37): `public async Task AlterTable_Centres_IsDenied() =>`
- [Line 41](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L41): `public async Task Delete_FromCentres_IsDenied() =>`
- [Line 45](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L45): `public async Task Insert_IntoMigrationsHistory_IsDenied() =>`
- [Line 51](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L51): `public async Task SelectAndInsert_OnMemberships_Succeed()`
- [Line 82](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L82): `public async Task Update_DataProtectionKeys_IsDenied() =>`
- [Line 86](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L86): `public async Task Delete_FromDataProtectionKeys_IsDenied() =>`
- [Line 90](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L90): `public async Task AllPlatformAndIdentityTables_AreOwnedByTutoringOwner()`
- [Line 109](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L109): `private async Task SeedCentreAndUserAsync(Guid centreId, Guid userId)`
- [Line 135](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs#L135): `private static async Task AssertDeniedAsync(string connectionString, string sql)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
    public async Task Update_DataProtectionKeys_IsDenied() =>
        await AssertDeniedAsync(Fixture.AppConnectionString, "update platform.data_protection_keys set xml = 'x'");

    [Fact]
    public async Task Delete_FromDataProtectionKeys_IsDenied() =>
        await AssertDeniedAsync(Fixture.AppConnectionString, "delete from platform.data_protection_keys");

    [Fact]
    public async Task AllPlatformAndIdentityTables_AreOwnedByTutoringOwner()
```

The excerpt stops at line 90; use the original file for the remaining 54 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
