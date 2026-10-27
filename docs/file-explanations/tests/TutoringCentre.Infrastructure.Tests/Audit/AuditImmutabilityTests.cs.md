# `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 31.8: proves the audit log cannot be altered through tutoring_app, nor read across centres — at the database-role level, independent of EF or the application pipeline, the same way Task 23.4 proved it for academics.subjects (`Academics.SubjectRawSqlRowLevelSecurityTests`).

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 183 lines and 8,043 bytes. Source hash: `28ad8740178a50558a642dccca2c4d2e623ce2f976063e1556f032aa3b6d58c5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Npgsql;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Identity;`
- Line 5: `using TutoringCentre.Infrastructure.Audit;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 8: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L17): `public sealed class AuditImmutabilityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L22): `public async Task SettingNile_UpdateAnyRow_Throws42501()`
- [Line 37](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L37): `public async Task SettingNile_DeleteAnyRow_Throws42501()`
- [Line 52](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L52): `public async Task SettingNile_Truncate_Throws42501()`
- [Line 67](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L67): `public async Task SettingNile_SelectsOnlyNileRows()`
- [Line 84](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L84): `public async Task SettingMaadi_SelectsNoneOfNilesRows()`
- [Line 100](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L100): `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- [Line 122](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L122): `public async Task ThroughEf_LoadingThenModifyingAnAuditEntry_TenantGuardThrows()`
- [Line 137](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L137): `private async Task<Guid> SeedAuditEntryAsync(Guid centreId)`
- [Line 155](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L155): `private async Task<NpgsqlConnection> OpenAppConnectionAsync()`
- [Line 162](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L162): `private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string value)`
- [Line 169](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L169): `private static async Task<List<Guid>> SelectCentreIdsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction)`
- [Line 182](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L182): `private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Audit;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Audit;

/// <summary>
/// Task 31.8: proves the audit log cannot be altered through tutoring_app, nor read across centres — at the
/// database-role level, independent of EF or the application pipeline, the same way Task 23.4 proved it for
/// academics.subjects (<see cref="Academics.SubjectRawSqlRowLevelSecurityTests"/>).
/// </summary>
public sealed class AuditImmutabilityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task SettingNile_UpdateAnyRow_Throws42501()
    {
        await SeedAuditEntryAsync(NileCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand("update audit.audit_entries set changes = '{}'::jsonb", connection, transaction);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_DeleteAnyRow_Throws42501()
    {
        await SeedAuditEntryAsync(NileCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand("delete from audit.audit_entries", connection, transaction);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_Truncate_Throws42501()
    {
        await SeedAuditEntryAsync(NileCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand("truncate audit.audit_entries", connection, transaction);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await SeedAuditEntryAsync(NileCentreId);
        await SeedAuditEntryAsync(NileCentreId);
        await SeedAuditEntryAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction);

        Assert.Equal(2, centreIds.Count);
        Assert.All(centreIds, id => Assert.Equal(NileCentreId, id));
    }

    [Fact]
    public async Task SettingMaadi_SelectsNoneOfNilesRows()
    {
        await SeedAuditEntryAsync(NileCentreId);
        await SeedAuditEntryAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
```

The excerpt stops at line 90; use the original file for the remaining 93 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
