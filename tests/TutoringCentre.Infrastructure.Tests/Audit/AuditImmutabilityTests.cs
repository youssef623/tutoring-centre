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
        await SetCentreAsync(connection, transaction, MaadiCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction);

        var onlyRow = Assert.Single(centreIds);
        Assert.Equal(MaadiCentreId, onlyRow);
    }

    [Fact]
    public async Task SettingNile_InsertWithMaadiCentre_Throws42501()
    {
        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        await using var command = new NpgsqlCommand(
            """
            insert into audit.audit_entries (id, centre_id, occurred_at, actor_type, action, entity_type, entity_id, changes)
            values (@id, @centre_id, now(), 'system', 'created', 'subject', @entity_id, '{}'::jsonb)
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("entity_id", Guid.CreateVersion7());

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task ThroughEf_LoadingThenModifyingAnAuditEntry_TenantGuardThrows()
    {
        var entryId = await SeedAuditEntryAsync(NileCentreId);

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        var loaded = await probe.Context.Set<AuditEntry>().SingleAsync(entry => entry.Id == entryId);

        // AuditEntry's properties are init-only: no handler, deliberate or buggy, can call a setter on it. The
        // only way to even attempt a change is EF's own metadata API, exactly as a bug deep in a repository
        // might. That attempt is what the guard refuses.
        probe.Context.Entry(loaded).Property(nameof(AuditEntry.Changes)).CurrentValue = "{\"tampered\":true}";

        await Assert.ThrowsAsync<TenantViolationException>(() => probe.Context.SaveChangesAsync());
    }

    private async Task<Guid> SeedAuditEntryAsync(Guid centreId)
    {
        var id = Guid.CreateVersion7();
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into audit.audit_entries (id, centre_id, occurred_at, actor_type, action, entity_type, entity_id, changes)
            values (@id, @centre_id, now(), 'system', 'created', 'subject', @entity_id, '{}'::jsonb)
            """,
            connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("centre_id", centreId);
        command.Parameters.AddWithValue("entity_id", Guid.CreateVersion7());
        await command.ExecuteNonQueryAsync();
        return id;
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
        await using var command = new NpgsqlCommand("select centre_id from audit.audit_entries", connection, transaction);
        await using var reader = await command.ExecuteReaderAsync();
        var centreIds = new List<Guid>();
        while (await reader.ReadAsync())
        {
            centreIds.Add(reader.GetGuid(0));
        }

        return centreIds;
    }

    private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);
}
