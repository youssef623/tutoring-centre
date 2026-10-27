# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.4: proves the row-level security policy itself (Task 21.3) against raw connections as tutoring_app, independent of EF, UnitOfWork or the application pipeline — the database-level guarantee the other layers depend on actually existing.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 237 lines and 10,553 bytes. Source hash: `e6343239d4ee804620ae1054ee371e0f90e9cb33c101c5ce9b40eccedbda936f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L11): `public sealed class ProbeRawSqlRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L14): `public async Task NoSetting_SelectCount_IsZero()`
- [Line 27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L27): `public async Task SettingNile_SelectsOnlyNileRows()`
- [Line 44](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L44): `public async Task SettingMaadi_SelectsOnlyMaadiRows()`
- [Line 61](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L61): `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- [Line 78](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L78): `public async Task SettingNile_UpdateCentreIdToMaadiOnNileRow_Throws42501()`
- [Line 96](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L96): `public async Task SettingNile_UpdateAndDeleteOnMaadiRow_AffectZeroRowsAndLeaveItIntact()`
- [Line 126](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L126): `public async Task NoSetting_Insert_Throws42501()`
- [Line 142](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L142): `public async Task SettingIsNotAUuid_StatementErrorsAndReturnsNoRows()`
- [Line 153](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L153): `public async Task AsOwnerWithNoSetting_SelectCount_IsZero()`
- [Line 166](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L166): `public async Task AsAppWithRowSecurityOff_SelectErrorsRatherThanReturningUnfilteredRows()`
- [Line 184](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L184): `public async Task SettingFromACommittedTransaction_IsAbsentInTheNextTransactionOnTheSameConnection()`
- [Line 205](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L205): `private async Task<NpgsqlConnection> OpenAppConnectionAsync()`
- [Line 212](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L212): `private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string value)`
- [Line 219](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L219): `private static async Task<List<Guid>> SelectCentreIdsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction)`
- [Line 232](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs#L232): `private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.4: proves the row-level security policy itself (Task 21.3) against raw connections as tutoring_app,
/// independent of EF, UnitOfWork or the application pipeline — the database-level guarantee the other layers
/// depend on actually existing.
/// </summary>
public sealed class ProbeRawSqlRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task NoSetting_SelectCount_IsZero()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from probe.tenant_probes");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(NileCentreId, "nile-2");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

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
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-2");

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
            "insert into probe.tenant_probes (id, centre_id, label) values (@id, @centre_id, @label)", connection, transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("label", "smuggled");

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateCentreIdToMaadiOnNileRow_Throws42501()
    {
        var probeId = await Fixture.SeedProbeAsync(NileCentreId, "nile-1");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            "update probe.tenant_probes set centre_id = @maadi where id = @id", connection, transaction);
        command.Parameters.AddWithValue("maadi", MaadiCentreId);
        command.Parameters.AddWithValue("id", probeId);

```

The excerpt stops at line 90; use the original file for the remaining 147 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
