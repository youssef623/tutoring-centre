using Npgsql;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 23.5: catalogue-driven coverage so a future tenant table that forgets row-level security fails the
/// build by name, rather than relying on someone remembering to add a test for it. Discovers every table from
/// PostgreSQL's own catalogue; never enumerates tables by hand.
/// </summary>
public sealed class RowLevelSecurityCoverageTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    // Every exemption must be justified in writing (full rationale: docs/architecture/tenancy.md's Exemptions
    // table). Today there is exactly one.
    private static readonly HashSet<(string Schema, string Table)> Exemptions = new()
    {
        // Read at login, before app.current_centre is ever set, to show the user which centres they belong to
        // and let them pick one (Month 1). Forcing a centre-based policy on it would make centre selection
        // itself impossible.
        ("identity", "memberships"),
    };

    [Fact]
    public async Task EveryTableWithACentreIdColumn_HasForcedRowLevelSecurityAndACompletePolicy()
    {
        var tenantTables = await DiscoverTablesWithCentreIdAsync();

        // Guards against the discovery query itself silently finding nothing, which would make every assertion
        // below vacuously true without proving anything.
        Assert.NotEmpty(tenantTables);

        var violations = new List<string>();
        foreach (var (schema, table) in tenantTables)
        {
            if (Exemptions.Contains((schema, table)))
            {
                continue;
            }

            var (rowSecurityEnabled, rowSecurityForced) = await ReadRowSecurityFlagsAsync(schema, table);
            if (!rowSecurityEnabled || !rowSecurityForced)
            {
                violations.Add($"{schema}.{table}: row security enabled={rowSecurityEnabled}, forced={rowSecurityForced} (expected both true)");
                continue;
            }

            if (!await HasCompletePolicyAsync(schema, table))
            {
                violations.Add($"{schema}.{table}: no policy with both a USING and a WITH CHECK expression");
            }
        }

        Assert.True(violations.Count == 0, "Tenant tables missing row-level security coverage:" + Environment.NewLine + string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public async Task TutoringApp_HasNoDeletePrivilegeOnSubjects()
    {
        var canDelete = await Fixture.ScalarAsync<bool>(
            $"select has_table_privilege('{DatabaseRoles.App}', 'academics.subjects', 'DELETE')");

        Assert.False(canDelete, "tutoring_app must end with SELECT, INSERT, UPDATE only on academics.subjects (Task 23.3).");
    }

    /// <summary>
    /// Every non-system table with a centre_id column, from pg_attribute/pg_class directly — never a hand-kept
    /// list. The probe schema (Task 20.3/21.3's test-only scaffolding) is excluded on purpose, not because it
    /// was overlooked: only probe.tenant_probes carries row-level security, deliberately, to prove the
    /// mechanism once — probe.tenant_probe_children exists only for Task 21.6's foreign-key proof and was never
    /// meant to carry its own policy.
    /// </summary>
    private async Task<List<(string Schema, string Table)>> DiscoverTablesWithCentreIdAsync()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            select n.nspname, c.relname
            from pg_class c
            join pg_namespace n on n.oid = c.relnamespace
            where c.relkind = 'r'
              and n.nspname not in ('pg_catalog', 'information_schema', 'probe')
              and exists (
                  select 1 from pg_attribute a
                  where a.attrelid = c.oid and a.attname = 'centre_id' and a.attnum > 0 and not a.attisdropped
              )
            order by n.nspname, c.relname
            """,
            connection);
        await using var reader = await command.ExecuteReaderAsync();

        var tables = new List<(string, string)>();
        while (await reader.ReadAsync())
        {
            tables.Add((reader.GetString(0), reader.GetString(1)));
        }

        return tables;
    }

    private async Task<(bool Enabled, bool Forced)> ReadRowSecurityFlagsAsync(string schema, string table)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "select relrowsecurity, relforcerowsecurity from pg_class where oid = @qualified::regclass", connection);
        command.Parameters.AddWithValue("qualified", $"{schema}.{table}");
        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        return (reader.GetBoolean(0), reader.GetBoolean(1));
    }

    private async Task<bool> HasCompletePolicyAsync(string schema, string table)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "select qual, with_check from pg_policies where schemaname = @schema and tablename = @table",
            connection);
        command.Parameters.AddWithValue("schema", schema);
        command.Parameters.AddWithValue("table", table);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            if (!await reader.IsDBNullAsync(0) && !await reader.IsDBNullAsync(1))
            {
                return true;
            }
        }

        return false;
    }
}
