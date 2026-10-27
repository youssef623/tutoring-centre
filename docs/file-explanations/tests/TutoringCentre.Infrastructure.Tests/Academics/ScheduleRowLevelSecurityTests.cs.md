# `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 52.11: the decisive row-level security checks on academics.schedule_rules and academics.class_sessions, as the real runtime role, independent of EF or the application pipeline (the style of Tasks 38.4 and 47.5). Rows are seeded as the superuser so the policy under test cannot have biased them.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 154 lines and 7,224 bytes. Source hash: `8f731c6c00648d506e8fd295e3cb74562e88d2438b4f408041070f1c23faac06`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L11): `public sealed class ScheduleRowLevelSecurityTests(PostgresFixture fixture) : ScheduleTestBase(fixture)`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L18): `public async Task NoSetting_SelectCount_IsZero(string table)`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L32): `public async Task SettingNile_SelectsOnlyNileRows(string table)`
- [Line 49](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L49): `public async Task SettingNile_InsertRuleWithMaadiCentre_Throws42501()`
- [Line 73](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L73): `public async Task SettingNile_InsertSessionWithMaadiCentre_Throws42501()`
- [Line 103](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L103): `public async Task SettingNile_DeleteOwnRow_Throws42501BecauseDeleteIsNotGranted(string table)`
- [Line 135](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L135): `private async Task<NpgsqlConnection> OpenAppConnectionAsync()`
- [Line 142](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L142): `private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, Guid centreId)`
- [Line 149](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L149): `private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 64 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
