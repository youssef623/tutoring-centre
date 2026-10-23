using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "enrollments",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    enrolled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    end_reason = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enrollments", x => x.id);
                    table.UniqueConstraint("ux_enrollments_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_enrollments_end_after_start", "ended_at IS NULL OR ended_at >= enrolled_at");
                    table.CheckConstraint("ck_enrollments_end_consistency", "(status = 'ended') = (ended_at IS NOT NULL) AND (status = 'ended') = (end_reason IS NOT NULL)");
                    table.CheckConstraint("ck_enrollments_end_reason", "end_reason IS NULL OR end_reason IN ('withdrawn','transferred')");
                    table.CheckConstraint("ck_enrollments_status", "status IN ('active','ended')");
                    table.ForeignKey(
                        name: "fk_enrollments_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_enrollments_groups",
                        columns: x => new { x.centre_id, x.group_id },
                        principalSchema: "academics",
                        principalTable: "groups",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_enrollments_students",
                        columns: x => new { x.centre_id, x.student_id },
                        principalSchema: "academics",
                        principalTable: "students",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_enrollments_centre_group_status",
                schema: "academics",
                table: "enrollments",
                columns: new[] { "centre_id", "group_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_enrollments_centre_student",
                schema: "academics",
                table: "enrollments",
                columns: new[] { "centre_id", "student_id" });

            migrationBuilder.CreateIndex(
                name: "ux_enrollments_active_student_group",
                schema: "academics",
                table: "enrollments",
                columns: new[] { "centre_id", "student_id", "group_id" },
                unique: true,
                filter: "status = 'active'");

            // Grants, then row-level security — in that order, after the table exists, so the table never
            // exists without its policy (following AddGroups). No delete grant: ended enrollments are history.
            migrationBuilder.Sql($"grant select, insert, update on {Schemas.Academics}.enrollments to {DatabaseRoles.App};");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "enrollments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "enrollments");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Academics}.enrollments from {DatabaseRoles.App};");

            migrationBuilder.DropTable(
                name: "enrollments",
                schema: "academics");
        }
    }
}
