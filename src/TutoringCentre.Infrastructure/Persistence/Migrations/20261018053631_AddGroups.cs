using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groups",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    teacher_id = table.Column<Guid>(type: "uuid", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                    table.UniqueConstraint("ux_groups_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_groups_capacity", "capacity BETWEEN 1 AND 200");
                    table.CheckConstraint("ck_groups_status", "status IN ('active','archived')");
                    table.ForeignKey(
                        name: "fk_groups_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_groups_subjects",
                        columns: x => new { x.centre_id, x.subject_id },
                        principalSchema: "academics",
                        principalTable: "subjects",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_groups_teachers",
                        columns: x => new { x.centre_id, x.teacher_id },
                        principalSchema: "academics",
                        principalTable: "teachers",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_groups_centre_subject",
                schema: "academics",
                table: "groups",
                columns: new[] { "centre_id", "subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_groups_centre_teacher",
                schema: "academics",
                table: "groups",
                columns: new[] { "centre_id", "teacher_id" });

            migrationBuilder.CreateIndex(
                name: "ux_groups_centre_normalized_name",
                schema: "academics",
                table: "groups",
                columns: new[] { "centre_id", "normalized_name" },
                unique: true);

            // Grants, then row-level security — in that order, after the table exists, so the table never
            // exists without its policy (following AddTeachers). No delete grant: groups are archived, never
            // deleted.
            migrationBuilder.Sql($"grant select, insert, update on {Schemas.Academics}.groups to {DatabaseRoles.App};");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "groups");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Academics}.groups from {DatabaseRoles.App};");

            migrationBuilder.DropTable(
                name: "groups",
                schema: "academics");
        }
    }
}
