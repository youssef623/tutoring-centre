# `tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.3: pure SQL-generation tests for the RLS migration helper — no database involved.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 55 lines and 2,199 bytes. Source hash: `b8ff54c4af851ac8d86a08255355dd08721bb5dd348e74b19a3b758149798298`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Infrastructure.Persistence.Migrations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs#L6): `public sealed class TenantRowLevelSecurityTests`
- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs#L11): `public void BuildEnableStatements_ForATable_IssuesEnableForceAndOnePolicyWithBothClausesAndNoToClause()`
- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs#L25): `public void BuildDisableStatements_ForATable_DropsThePolicyThenUnforcesThenDisables()`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs#L42): `public void BuildEnableStatements_UnsafeSchemaName_Throws(string schema)`
- [Line 51](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs#L51): `public void BuildEnableStatements_UnsafeTableName_Throws(string table)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Infrastructure.Persistence.Migrations;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>Task 21.3: pure SQL-generation tests for the RLS migration helper — no database involved.</summary>
public sealed class TenantRowLevelSecurityTests
{
    private const string Expression = "centre_id = nullif(current_setting('app.current_centre', true), '')::uuid";

    [Fact]
    public void BuildEnableStatements_ForATable_IssuesEnableForceAndOnePolicyWithBothClausesAndNoToClause()
    {
        var statements = TenantRowLevelSecurity.BuildEnableStatements("probe", "tenant_probes");

        Assert.Equal(
            [
                "alter table probe.tenant_probes enable row level security;",
                "alter table probe.tenant_probes force row level security;",
                $"create policy tenant_isolation on probe.tenant_probes for all using ({Expression}) with check ({Expression});",
            ],
            statements);
    }

    [Fact]
    public void BuildDisableStatements_ForATable_DropsThePolicyThenUnforcesThenDisables()
    {
        var statements = TenantRowLevelSecurity.BuildDisableStatements("probe", "tenant_probes");

        Assert.Equal(
            [
                "drop policy tenant_isolation on probe.tenant_probes;",
                "alter table probe.tenant_probes no force row level security;",
                "alter table probe.tenant_probes disable row level security;",
            ],
            statements);
    }

    [Theory]
    [InlineData("probe'; drop table platform.centres; --")]
    [InlineData("probe; select 1;")]
    [InlineData("")]
    public void BuildEnableStatements_UnsafeSchemaName_Throws(string schema)
    {
        Assert.Throws<ArgumentException>(() => TenantRowLevelSecurity.BuildEnableStatements(schema, "tenant_probes"));
    }

    [Theory]
    [InlineData("tenant_probes'; drop table platform.centres; --")]
    [InlineData("tenant_probes; select 1;")]
    [InlineData("")]
    public void BuildEnableStatements_UnsafeTableName_Throws(string table)
    {
        Assert.Throws<ArgumentException>(() => TenantRowLevelSecurity.BuildEnableStatements("probe", table));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
