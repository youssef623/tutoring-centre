# `src/TutoringCentre.Infrastructure/Persistence/Migrations/AppendOnlyAccessGrants.cs`

## Purpose

Provides a narrower schema/default/table grant protocol for append-only data: SELECT and INSERT only. Audit immutability depends on runtime privileges lacking UPDATE and DELETE in addition to application behavior. The reverse operation revokes table grants, future defaults, and schema usage.

## Where it fits

Category: **migration**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppendOnlyAccessGrants.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 35 lines and 1,803 bytes. Source hash: `9e158144916d78e6d0fe353eda64803584013a332861a1ae8ba465df3a48be76`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore.Migrations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppendOnlyAccessGrants.cs#L12): `internal static class AppendOnlyAccessGrants`
- [Line 14](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppendOnlyAccessGrants.cs#L14): `public static void GrantAppendOnly(this MigrationBuilder migrationBuilder, string schema, string table)`
- [Line 25](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppendOnlyAccessGrants.cs#L25): `public static void RevokeAppendOnly(this MigrationBuilder migrationBuilder, string schema, string table)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:27](../../../../../onboarding-work/extend_backend_purposes.py#L27)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

namespace TutoringCentre.Infrastructure.Persistence.Migrations;

/// <summary>
/// What a migration adding an append-only table (Task 31.3's audit log) calls instead of
/// <see cref="RuntimeAccessGrants.GrantRuntimeAccess"/>: schema usage, a default-privileges baseline of SELECT
/// and INSERT only for future tables in the schema, and the same SELECT/INSERT grant on the table itself.
/// Never UPDATE, never DELETE — the row's immutability is enforced by what tutoring_app is never granted, not by
/// application code choosing not to use it.
/// </summary>
internal static class AppendOnlyAccessGrants
{
    public static void GrantAppendOnly(this MigrationBuilder migrationBuilder, string schema, string table)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.Sql($"grant usage on schema {schema} to {DatabaseRoles.App};");
        migrationBuilder.Sql(
            $"alter default privileges for role {DatabaseRoles.Owner} in schema {schema} "
            + $"grant select, insert on tables to {DatabaseRoles.App};");
        migrationBuilder.Sql($"grant select, insert on {schema}.{table} to {DatabaseRoles.App};");
    }

    public static void RevokeAppendOnly(this MigrationBuilder migrationBuilder, string schema, string table)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.Sql($"revoke select, insert on {schema}.{table} from {DatabaseRoles.App};");
        migrationBuilder.Sql(
            $"alter default privileges for role {DatabaseRoles.Owner} in schema {schema} "
            + $"revoke select, insert on tables from {DatabaseRoles.App};");
        migrationBuilder.Sql($"revoke usage on schema {schema} from {DatabaseRoles.App};");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
