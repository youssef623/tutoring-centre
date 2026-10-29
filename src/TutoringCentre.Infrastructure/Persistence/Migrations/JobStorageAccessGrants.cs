using Microsoft.EntityFrameworkCore.Migrations;

namespace TutoringCentre.Infrastructure.Persistence.Migrations;

/// <summary>Hangfire alone needs deletes and sequence access. No runtime schema-changing privileges.</summary>
internal static class JobStorageAccessGrants
{
    public static void GrantJobStorageAccess(this MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);
        migrationBuilder.Sql("grant usage on schema hangfire to tutoring_app;");
        migrationBuilder.Sql("revoke create on schema hangfire, jobs from public, tutoring_app;");
        migrationBuilder.Sql("alter default privileges for role tutoring_owner in schema hangfire grant select, insert, update, delete on tables to tutoring_app;");
        migrationBuilder.Sql("alter default privileges for role tutoring_owner in schema hangfire grant usage, select on sequences to tutoring_app;");
    }
}
