# `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 44.14: the decisive row-level security checks on the real academics.groups table, as the real runtime role, independent of EF or the application pipeline. Rows are seeded as the superuser so the policy under test cannot have biased them.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 133 lines and 6,040 bytes. Source hash: `9da0870c4263df9c16da928cea270a6b68025de1c72e43873351fd1ff4ac2407`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L12): `public sealed class GroupRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L17): `public async Task NoSetting_SelectCount_IsZero()`
- [Line 29](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L29): `public async Task SettingNile_SelectsOnlyNileRows()`
- [Line 46](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L46): `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- [Line 72](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L72): `public async Task SettingNile_UpdateMaadiGroupById_AffectsZeroRows()`
- [Line 92](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L92): `public async Task SettingNile_DeleteOwnGroup_Throws42501BecauseDeleteIsNotGranted()`
- [Line 107](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L107): `private async Task<Guid> SeedGroupAsync(Guid centreId, string name)`
- [Line 114](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L114): `private async Task<NpgsqlConnection> OpenAppConnectionAsync()`
- [Line 121](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L121): `private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, Guid centreId)`
- [Line 128](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L128): `private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)`

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
/// Task 44.14: the decisive row-level security checks on the real academics.groups table, as the real runtime
/// role, independent of EF or the application pipeline. Rows are seeded as the superuser so the policy under
/// test cannot have biased them.
/// </summary>
public sealed class GroupRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task NoSetting_SelectCount_IsZero()
    {
        await SeedGroupAsync(NileCentreId, "Grade 10 A");
        await SeedGroupAsync(MaadiCentreId, "Grade 10 A");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        Assert.Equal(0, await ScalarAsync<long>(connection, transaction, "select count(*) from academics.groups"));
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await SeedGroupAsync(NileCentreId, "Grade 10 A");
        await SeedGroupAsync(NileCentreId, "Grade 10 B");
        await SeedGroupAsync(MaadiCentreId, "Grade 10 A");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        Assert.Equal(2, await ScalarAsync<long>(connection, transaction, "select count(*) from academics.groups"));
        Assert.Equal(
            0,
            await ScalarAsync<long>(connection, transaction, $"select count(*) from academics.groups where centre_id <> '{NileCentreId}'"));
    }

    [Fact]
    public async Task SettingNile_InsertWithMaadiCentre_Throws42501()
    {
        var maadiSubjectId = await Fixture.SeedSubjectAsync(MaadiCentreId, "Mathematics");
        var maadiTeacherId = await Fixture.SeedTeacherAsync(MaadiCentreId, "Mona Said");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var command = new NpgsqlCommand(
            """
            insert into academics.groups (id, centre_id, name, normalized_name, subject_id, teacher_id, capacity, status, created_at)
            values (@id, @centre_id, 'Smuggled', 'SMUGGLED', @subject_id, @teacher_id, 20, 'active', now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("subject_id", maadiSubjectId);
        command.Parameters.AddWithValue("teacher_id", maadiTeacherId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_UpdateMaadiGroupById_AffectsZeroRows()
    {
        var maadiGroupId = await SeedGroupAsync(MaadiCentreId, "Grade 10 A");

        await using (var connection = await OpenAppConnectionAsync())
        {
            await using var transaction = await connection.BeginTransactionAsync();
            await SetCentreAsync(connection, transaction, NileCentreId);

            await using var update = new NpgsqlCommand("update academics.groups set capacity = 99 where id = @id", connection, transaction);
            update.Parameters.AddWithValue("id", maadiGroupId);
            Assert.Equal(0, await update.ExecuteNonQueryAsync());

            await transaction.CommitAsync();
        }

        Assert.Equal(20, await Fixture.ScalarAsync<int>($"select capacity from academics.groups where id = '{maadiGroupId}'"));
    }

```

The excerpt stops at line 90; use the original file for the remaining 43 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
