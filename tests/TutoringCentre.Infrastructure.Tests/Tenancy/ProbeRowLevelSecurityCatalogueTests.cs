using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.3: confirms the fixture actually applied row-level security to probe.tenant_probes through Postgres's
/// own catalogue, not just that TenantRowLevelSecurityTests' statement text looks right in isolation.
/// </summary>
public sealed class ProbeRowLevelSecurityCatalogueTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task ProbeTable_RowSecurityIsEnabledAndForced()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "select relrowsecurity, relforcerowsecurity from pg_class where oid = 'probe.tenant_probes'::regclass",
            connection);
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        Assert.True(reader.GetBoolean(0));
        Assert.True(reader.GetBoolean(1));
    }

    [Fact]
    public async Task ProbeTable_HasExactlyOneTenantIsolationPolicyForAllCommandsWithNoToClause()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            select cmd, roles, qual, with_check
            from pg_policies
            where schemaname = 'probe' and tablename = 'tenant_probes' and policyname = 'tenant_isolation'
            """,
            connection);
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync(), "No policy named tenant_isolation on probe.tenant_probes.");

        var command_ = reader.GetString(0);
        var roles = (string[])reader.GetValue(1);
        var usingExpression = reader.GetString(2);
        var withCheckExpression = reader.GetString(3);

        Assert.Equal("ALL", command_);
        Assert.Equal(["public"], roles); // no TO clause -> applies to PUBLIC
        Assert.Contains("current_setting", usingExpression, StringComparison.Ordinal);
        Assert.Contains("current_setting", withCheckExpression, StringComparison.Ordinal);
        Assert.False(await reader.ReadAsync(), "More than one policy on probe.tenant_probes.");
    }
}
