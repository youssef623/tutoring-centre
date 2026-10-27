# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.3: confirms the fixture actually applied row-level security to probe.tenant_probes through Postgres's own catalogue, not just that TenantRowLevelSecurityTests' statement text looks right in isolation.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 53 lines and 2,417 bytes. Source hash: `29730653c65ee441271eaed5dc8580e9746fcddf139d2722d93fdceeb491e359`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs#L10): `public sealed class ProbeRowLevelSecurityCatalogueTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs#L13): `public async Task ProbeTable_RowSecurityIsEnabledAndForced()`
- [Line 28](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs#L28): `public async Task ProbeTable_HasExactlyOneTenantIsolationPolicyForAllCommandsWithNoToClause()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
