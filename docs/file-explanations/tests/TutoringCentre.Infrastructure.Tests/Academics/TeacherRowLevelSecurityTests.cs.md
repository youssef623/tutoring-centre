# `tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 43.14: repeats the decisive row-level security checks on the real academics.teachers table, as the real runtime role, independent of EF or the application pipeline, plus the two integrity checks that are specific to this table's account link: the composite foreign key into identity.memberships (23503, a Maadi-only user is simply not a valid (centre_id, user_id) pair for Nile) and the partial unique index on (centre_id, user_id) (23505, the same user linked to a second Nile teacher). Both integrity checks run as the superuser — row-level security is not what is under test there, the raw constraint is.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 212 lines and 9,904 bytes. Source hash: `b0941b8b9b8c62b1c13e9e89ff2c4eca9dd5318a75f67a156184144e6b7d269a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Domain.Identity;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L16): `public sealed class TeacherRowLevelSecurityTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L23): `public async Task NoSetting_SelectCount_IsZero()`
- [Line 36](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L36): `public async Task SettingNile_SelectsOnlyNileRows()`
- [Line 53](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L53): `public async Task SettingMaadi_SelectsOnlyMaadiRows()`
- [Line 70](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L70): `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- [Line 93](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L93): `public async Task SettingNile_UpdateNileTeacherCentreIdToMaadi_Throws42501()`
- [Line 111](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L111): `public async Task SettingNile_UpdateMaadiTeacherById_AffectsZeroRows()`
- [Line 134](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L134): `public async Task AsSuperuser_InsertTeacherWithNileCentreAndMaadiOnlyUser_Throws23503ForeignKeyViolation()`
- [Line 157](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L157): `public async Task AsSuperuser_LinkSameNileUserToASecondNileTeacher_Throws23505UniqueViolation()`
- [Line 180](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L180): `private async Task<NpgsqlConnection> OpenAppConnectionAsync()`
- [Line 187](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L187): `private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string value)`
- [Line 194](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L194): `private static async Task<List<Guid>> SelectCentreIdsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction)`
- [Line 207](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L207): `private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 122 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
