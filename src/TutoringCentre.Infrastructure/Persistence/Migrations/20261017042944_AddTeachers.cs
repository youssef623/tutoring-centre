using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeachers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "teachers",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    search_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teachers", x => x.id);
                    table.UniqueConstraint("ux_teachers_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_teachers_phone_format", "phone IS NULL OR phone ~ '^\\+[1-9][0-9]{7,14}$'");
                    table.CheckConstraint("ck_teachers_status", "status IN ('active','archived')");
                    table.ForeignKey(
                        name: "fk_teachers_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_teachers_centre_search_name",
                schema: "academics",
                table: "teachers",
                columns: new[] { "centre_id", "search_name" });

            migrationBuilder.CreateIndex(
                name: "ux_teachers_centre_user",
                schema: "academics",
                table: "teachers",
                columns: new[] { "centre_id", "user_id" },
                unique: true,
                filter: "user_id IS NOT NULL");

            // Composite FK into identity.memberships, added as a raw operation rather than through the EF
            // model (TeacherConfiguration has no HasOne/HasForeignKey for it — see that type's remarks):
            // Postgres accepts ux_memberships_centre_user, a plain unique index, as a valid FK target without
            // identity.memberships needing an EF-declared key. Not checked while user_id is null (standard SQL
            // composite-FK behaviour), which is exactly "no account linked yet". Nothing in identity changes.
            migrationBuilder.AddForeignKey(
                name: "fk_teachers_memberships",
                schema: "academics",
                table: "teachers",
                columns: new[] { "centre_id", "user_id" },
                principalSchema: "identity",
                principalTable: "memberships",
                principalColumns: new[] { "centre_id", "user_id" },
                onDelete: ReferentialAction.Restrict);

            // Grants, then the revoke that narrows them, then row-level security — in that order, after the
            // table exists, so the table never exists without its policy (following AddSubjects/AddStudentsAndParents).
            migrationBuilder.Sql($"grant select, insert, update, delete on {Schemas.Academics}.teachers to {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke delete on {Schemas.Academics}.teachers from {DatabaseRoles.App};");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "teachers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "teachers");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Academics}.teachers from {DatabaseRoles.App};");

            migrationBuilder.DropTable(
                name: "teachers",
                schema: "academics");
        }
    }
}
