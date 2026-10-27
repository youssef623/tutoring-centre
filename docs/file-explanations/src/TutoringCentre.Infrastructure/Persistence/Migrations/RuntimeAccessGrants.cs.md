# `src/TutoringCentre.Infrastructure/Persistence/Migrations/RuntimeAccessGrants.cs`

## Purpose

Emits schema USAGE and owner-specific default privileges for future tables, granting SELECT, INSERT, and UPDATE. Existing tables still need explicit grants and DELETE requires a deliberate exception. Reversal removes both default privileges and schema usage, so schema-sharing migration order matters.

## Where it fits

Category: **migration**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/RuntimeAccessGrants.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,730 bytes. Source hash: `023940fba8e35eecf8475f476ea10a3b2f6aee444e6ae658a9685589bf645cd8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore.Migrations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/RuntimeAccessGrants.cs#L13): `internal static class RuntimeAccessGrants`
- [Line 15](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/RuntimeAccessGrants.cs#L15): `public static void GrantRuntimeAccess(this MigrationBuilder migrationBuilder, string schema)`
- [Line 25](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/RuntimeAccessGrants.cs#L25): `public static void RevokeRuntimeAccess(this MigrationBuilder migrationBuilder, string schema)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:26](../../../../../onboarding-work/extend_backend_purposes.py#L26)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

namespace TutoringCentre.Infrastructure.Persistence.Migrations;

/// <summary>
/// What every migration that adds a schema calls: USAGE on the schema, plus a default-privileges baseline (SELECT,
/// INSERT, UPDATE) for tables tutoring_owner creates in it later. <c>GRANT ... ON ALL TABLES</c> only ever covers
/// tables that already exist, so without the default-privileges half, a later migration's new table would silently
/// stay inaccessible to tutoring_app until someone remembered to grant it by hand. Existing tables, and any table
/// that needs something other than this baseline (DELETE, or read-only), are granted explicitly by the migration
/// that creates them instead.
/// </summary>
internal static class RuntimeAccessGrants
{
    public static void GrantRuntimeAccess(this MigrationBuilder migrationBuilder, string schema)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.Sql($"grant usage on schema {schema} to {DatabaseRoles.App};");
        migrationBuilder.Sql(
            $"alter default privileges for role {DatabaseRoles.Owner} in schema {schema} "
            + $"grant select, insert, update on tables to {DatabaseRoles.App};");
    }

    public static void RevokeRuntimeAccess(this MigrationBuilder migrationBuilder, string schema)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.Sql(
            $"alter default privileges for role {DatabaseRoles.Owner} in schema {schema} "
            + $"revoke select, insert, update on tables from {DatabaseRoles.App};");
        migrationBuilder.Sql($"revoke usage on schema {schema} from {DatabaseRoles.App};");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
