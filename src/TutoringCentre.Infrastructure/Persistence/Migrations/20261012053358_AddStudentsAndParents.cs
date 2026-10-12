using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentsAndParents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parents",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    search_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parents", x => x.id);
                    table.UniqueConstraint("ux_parents_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_parents_phone_format", "phone ~ '^\\+[1-9][0-9]{7,14}$'");
                    table.ForeignKey(
                        name: "fk_parents_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "students",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    search_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    school_year = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    links_revision = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_students", x => x.id);
                    table.UniqueConstraint("ux_students_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_students_phone_format", "phone IS NULL OR phone ~ '^\\+[1-9][0-9]{7,14}$'");
                    table.CheckConstraint("ck_students_status", "status IN ('active','archived')");
                    table.ForeignKey(
                        name: "fk_students_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_parents",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    relationship = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_parents", x => x.id);
                    table.UniqueConstraint("ux_student_parents_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_student_parents_relationship", "relationship IN ('father','mother','guardian','other')");
                    table.ForeignKey(
                        name: "fk_student_parents_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_parents_parents",
                        columns: x => new { x.centre_id, x.parent_id },
                        principalSchema: "academics",
                        principalTable: "parents",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_parents_students",
                        columns: x => new { x.centre_id, x.student_id },
                        principalSchema: "academics",
                        principalTable: "students",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_parents_centre_search_name",
                schema: "academics",
                table: "parents",
                columns: new[] { "centre_id", "search_name" });

            migrationBuilder.CreateIndex(
                name: "ux_parents_centre_phone",
                schema: "academics",
                table: "parents",
                columns: new[] { "centre_id", "phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_student_parents_centre_parent",
                schema: "academics",
                table: "student_parents",
                columns: new[] { "centre_id", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "ux_student_parents_one_primary",
                schema: "academics",
                table: "student_parents",
                columns: new[] { "centre_id", "student_id" },
                unique: true,
                filter: "is_primary");

            migrationBuilder.CreateIndex(
                name: "ux_student_parents_student_parent",
                schema: "academics",
                table: "student_parents",
                columns: new[] { "centre_id", "student_id", "parent_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_students_centre_search_name_id",
                schema: "academics",
                table: "students",
                columns: new[] { "centre_id", "search_name", "id" });

            // Grants, then the revoke that narrows them, then row-level security on each table — in that
            // order, after every table exists, so no tenant table is ever reachable without its policy
            // (Task 38.3, following AddSubjects). student_parents keeps DELETE: links are removed, and the
            // removal is audited.
            migrationBuilder.Sql($"grant select, insert, update, delete on {Schemas.Academics}.parents to {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke delete on {Schemas.Academics}.parents from {DatabaseRoles.App};");
            migrationBuilder.Sql($"grant select, insert, update, delete on {Schemas.Academics}.students to {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke delete on {Schemas.Academics}.students from {DatabaseRoles.App};");
            migrationBuilder.Sql($"grant select, insert, update, delete on {Schemas.Academics}.student_parents to {DatabaseRoles.App};");

            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "parents");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "students");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "student_parents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "student_parents");
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "students");
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "parents");

            migrationBuilder.Sql($"revoke select, insert, update, delete on {Schemas.Academics}.student_parents from {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Academics}.students from {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Academics}.parents from {DatabaseRoles.App};");

            migrationBuilder.DropTable(
                name: "student_parents",
                schema: "academics");

            migrationBuilder.DropTable(
                name: "parents",
                schema: "academics");

            migrationBuilder.DropTable(
                name: "students",
                schema: "academics");
        }
    }
}
