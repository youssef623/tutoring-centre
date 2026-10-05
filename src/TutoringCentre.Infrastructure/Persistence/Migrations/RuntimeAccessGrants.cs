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
