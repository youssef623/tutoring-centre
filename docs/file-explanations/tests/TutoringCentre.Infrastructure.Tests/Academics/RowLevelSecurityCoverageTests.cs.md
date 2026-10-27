# `tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 23.5: catalogue-driven coverage so a future tenant table that forgets row-level security fails the build by name, rather than relying on someone remembering to add a test for it. Discovers every table from PostgreSQL's own catalogue; never enumerates tables by hand.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 135 lines and 5,978 bytes. Source hash: `afcfb5ba6103d259c037c77d70c6be74da22fbf64e88a138399d4d1408e15753`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Persistence;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs#L12): `public sealed class RowLevelSecurityCoverageTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs#L25): `public async Task EveryTableWithACentreIdColumn_HasForcedRowLevelSecurityAndACompletePolicy()`
- [Line 58](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs#L58): `public async Task TutoringApp_HasNoDeletePrivilegeOnSubjects()`
- [Line 114](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs#L114): `private async Task<bool> HasCompletePolicyAsync(string schema, string table)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 45 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
