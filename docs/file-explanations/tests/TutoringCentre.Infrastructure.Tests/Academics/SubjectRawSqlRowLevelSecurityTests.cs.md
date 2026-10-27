# `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 23.4: repeats the decisive row-level security checks (Task 21.4) on the real academics.subjects table, as the real runtime role, independent of EF or the application pipeline — the direct evidence that this table, not just the probe, is actually wired to the policy. Test 7 is specific to subjects: tutoring_app has no DELETE privilege on it at all (Task 23.3), so a delete is refused before row-level security ever runs.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 189 lines and 8,269 bytes. Source hash: `5c49110abb911682b14d52ac0bcfb67abab7d793c10d4a55feeb635d7c77ae04`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L13): `public sealed class SubjectRawSqlRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L18): `public async Task NoSetting_SelectCount_IsZero()`
- [Line 31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L31): `public async Task SettingNile_SelectsOnlyNileRows()`
- [Line 48](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L48): `public async Task SettingMaadi_SelectsOnlyMaadiRows()`
- [Line 65](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L65): `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- [Line 88](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L88): `public async Task SettingNile_UpdateNileSubjectCentreIdToMaadi_Throws42501()`
- [Line 106](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L106): `public async Task SettingNile_UpdateMaadiSubjectById_AffectsZeroRows()`
- [Line 129](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L129): `public async Task Delete_AnySubject_Throws42501NoPrivilege()`
- [Line 145](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L145): `public async Task AsOwnerWithNoSetting_SelectCount_IsZero()`
- [Line 157](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L157): `private async Task<NpgsqlConnection> OpenAppConnectionAsync()`
- [Line 164](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L164): `private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string value)`
- [Line 171](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L171): `private static async Task<List<Guid>> SelectCentreIdsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction)`
- [Line 184](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs#L184): `private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 99 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
