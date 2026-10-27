# `src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs`

## Purpose

The migration helper emits enable, force, and tenant-isolation policy SQL using the transaction current-centre setting for both row visibility and write checks. It validates table and schema identifiers before interpolation. Policies provide a database backstop independent of EF filters, so runtime privileges and transaction-local settings remain essential.

## Where it fits

Category: **migration**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 79 lines and 3,614 bytes. Source hash: `40eb02560f8d6e26b4541ea3a1d5819e82c39cc3e5d6e4fc536d16fde776736f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`
- Line 2: `using Microsoft.EntityFrameworkCore.Migrations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs#L14): `internal static class TenantRowLevelSecurity`
- [Line 25](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs#L25): `public static IReadOnlyList<string> BuildEnableStatements(string schema, string table)`
- [Line 36](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs#L36): `public static IReadOnlyList<string> BuildDisableStatements(string schema, string table)`
- [Line 47](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs#L47): `public static void EnableTenantRowLevelSecurity(this MigrationBuilder migrationBuilder, string schema, string table)`
- [Line 56](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs#L56): `public static void DisableTenantRowLevelSecurity(this MigrationBuilder migrationBuilder, string schema, string table)`
- [Line 65](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs#L65): `private static string QualifyTable(string schema, string table)`
- [Line 72](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/TenantRowLevelSecurity.cs#L72): `private static void ValidateIdentifier(string? identifier, string parameterName)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:121](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L121)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs:13](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TutoringCentre.Infrastructure.Persistence.Migrations;

/// <summary>
/// What every migration that adds a tenant-owned table calls (from Day 23) to enforce the same centre_id
/// boundary (Task 21.1's app.current_centre) inside PostgreSQL itself, regardless of how the row is reached —
/// the backstop the EF query filter and write guard (Day 20) cannot be, since both run only inside the
/// application process. FORCE means even tutoring_owner, the table's own owner, is bound by the policy; only an
/// actual superuser or a BYPASSRLS role escapes it, and the Day 19 readiness check fails closed if the runtime
/// connection is ever either.
/// </summary>
internal static class TenantRowLevelSecurity
{
    // Defined exactly once: the migration helper below and PostgresFixture (Infrastructure.Tests) both call
    // BuildEnableStatements, so the probe table proves the exact SQL a real migration runs, not a stand-in for it.
    private const string PolicyName = "tenant_isolation";
    private const string PolicyExpression = "centre_id = nullif(current_setting('app.current_centre', true), '')::uuid";

    // An unquoted Postgres identifier: a letter or underscore, then letters, digits or underscores. Rejects
    // anything that could close the identifier or the statement early — a quote, a semicolon, whitespace.
    private static readonly Regex IdentifierPattern = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    public static IReadOnlyList<string> BuildEnableStatements(string schema, string table)
    {
        var qualifiedTable = QualifyTable(schema, table);
        return
        [
            $"alter table {qualifiedTable} enable row level security;",
            $"alter table {qualifiedTable} force row level security;",
            $"create policy {PolicyName} on {qualifiedTable} for all using ({PolicyExpression}) with check ({PolicyExpression});",
        ];
    }

    public static IReadOnlyList<string> BuildDisableStatements(string schema, string table)
    {
        var qualifiedTable = QualifyTable(schema, table);
        return
        [
            $"drop policy {PolicyName} on {qualifiedTable};",
            $"alter table {qualifiedTable} no force row level security;",
            $"alter table {qualifiedTable} disable row level security;",
        ];
    }

    public static void EnableTenantRowLevelSecurity(this MigrationBuilder migrationBuilder, string schema, string table)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);
        foreach (var statement in BuildEnableStatements(schema, table))
        {
            migrationBuilder.Sql(statement);
        }
    }

    public static void DisableTenantRowLevelSecurity(this MigrationBuilder migrationBuilder, string schema, string table)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);
        foreach (var statement in BuildDisableStatements(schema, table))
        {
            migrationBuilder.Sql(statement);
        }
    }

    private static string QualifyTable(string schema, string table)
    {
        ValidateIdentifier(schema, nameof(schema));
        ValidateIdentifier(table, nameof(table));
        return $"{schema}.{table}";
    }

    private static void ValidateIdentifier(string? identifier, string parameterName)
    {
        if (identifier is null || !IdentifierPattern.IsMatch(identifier))
        {
            throw new ArgumentException($"'{identifier}' is not a safe, unquoted SQL identifier.", parameterName);
        }
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
