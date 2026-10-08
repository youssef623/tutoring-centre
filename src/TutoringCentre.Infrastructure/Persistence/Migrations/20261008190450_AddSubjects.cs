using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "academics");

            migrationBuilder.CreateTable(
                name: "subjects",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)

                    // No "xmin" column here (Task 23.2): it is PostgreSQL's own system column on every table,
                    // not something a migration creates. Declaring it explicitly would fail — "xmin" is a
                    // reserved system column name — so EF's generated column entry for it is removed by hand.
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subjects", x => x.id);
                    table.UniqueConstraint("ux_subjects_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_subjects_status", "status IN ('active','archived')");
                    table.ForeignKey(
                        name: "fk_subjects_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_subjects_centre_normalized_name",
                schema: "academics",
                table: "subjects",
                columns: new[] { "centre_id", "normalized_name" },
                unique: true);

            // Grants, then the revoke that narrows them, then row-level security — in that order, after the
            // table exists, so the table never exists without its policy (Task 23.3). GrantRuntimeAccess only
            // sets up schema usage and default privileges for academics tables created after this point; since
            // subjects already exists by now, it needs its own explicit grant.
            migrationBuilder.GrantRuntimeAccess(Schemas.Academics);
            migrationBuilder.Sql($"grant select, insert, update, delete on {Schemas.Academics}.subjects to {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke delete on {Schemas.Academics}.subjects from {DatabaseRoles.App};");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "subjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "subjects");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Academics}.subjects from {DatabaseRoles.App};");
            migrationBuilder.RevokeRuntimeAccess(Schemas.Academics);

            migrationBuilder.DropTable(
                name: "subjects",
                schema: "academics");
        }
    }
}
