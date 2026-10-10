using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDataProtectionKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "data_protection_keys",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    friendly_name = table.Column<string>(type: "text", nullable: true),
                    xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_protection_keys", x => x.id);
                });

            // platform's existing default-privileges baseline (AddRuntimeRoleGrants, Task 19.4) already gives
            // tutoring_app SELECT, INSERT and UPDATE the moment this table exists. UPDATE is revoked explicitly:
            // a key ring entry is written once and never modified in place, so tutoring_app ends with SELECT
            // and INSERT only (Task 34.6) — the same shape as the audit log's append-only grant, but narrowed
            // on this one table rather than on the whole schema's future tables, since platform holds other
            // tables (centres) that do need UPDATE.
            migrationBuilder.Sql($"revoke update on platform.data_protection_keys from {DatabaseRoles.App};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"grant update on platform.data_protection_keys to {DatabaseRoles.App};");

            migrationBuilder.DropTable(
                name: "data_protection_keys",
                schema: "platform");
        }
    }
}
