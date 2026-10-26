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
