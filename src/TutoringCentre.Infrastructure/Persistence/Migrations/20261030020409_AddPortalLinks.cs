using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPortalLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_memberships_role",
                schema: "identity",
                table: "memberships");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_memberships_centre_user",
                schema: "identity",
                table: "memberships",
                columns: new[] { "centre_id", "user_id" });

            migrationBuilder.CreateTable(
                name: "portal_links",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_portal_links", x => x.id);
                    table.UniqueConstraint("ux_portal_links_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_portal_links_role", "role_key IN ('parent','student')");
                    table.CheckConstraint("ck_portal_links_subject", "(role_key = 'parent' AND parent_id IS NOT NULL AND student_id IS NULL) OR (role_key = 'student' AND student_id IS NOT NULL AND parent_id IS NULL)");
                    table.ForeignKey(
                        name: "fk_portal_links_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_portal_links_created_by",
                        column: x => x.created_by,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_portal_links_membership_identity",
                        columns: x => new { x.centre_id, x.user_id },
                        principalSchema: "identity",
                        principalTable: "memberships",
                        principalColumns: new[] { "centre_id", "user_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_portal_links_parents",
                        columns: x => new { x.centre_id, x.parent_id },
                        principalSchema: "academics",
                        principalTable: "parents",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_portal_links_students",
                        columns: x => new { x.centre_id, x.student_id },
                        principalSchema: "academics",
                        principalTable: "students",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_memberships_centre_user_role",
                schema: "identity",
                table: "memberships",
                columns: new[] { "centre_id", "user_id", "role" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_memberships_role",
                schema: "identity",
                table: "memberships",
                sql: "role IN ('owner','teacher','secretary','parent','student')");

            migrationBuilder.CreateIndex(
                name: "ix_portal_links_created_by",
                schema: "identity",
                table: "portal_links",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ux_portal_links_centre_parent",
                schema: "identity",
                table: "portal_links",
                columns: new[] { "centre_id", "parent_id" },
                unique: true,
                filter: "parent_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_portal_links_centre_student",
                schema: "identity",
                table: "portal_links",
                columns: new[] { "centre_id", "student_id" },
                unique: true,
                filter: "student_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_portal_links_centre_user",
                schema: "identity",
                table: "portal_links",
                columns: new[] { "centre_id", "user_id" },
                unique: true);

            migrationBuilder.Sql("alter table identity.portal_links add constraint fk_portal_links_memberships foreign key (centre_id, user_id, role_key) references identity.memberships (centre_id, user_id, role) on delete restrict on update restrict;");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Identity, "portal_links");
            migrationBuilder.Sql("revoke all on identity.portal_links from tutoring_app; grant select, insert on identity.portal_links to tutoring_app;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "portal_links",
                schema: "identity");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_memberships_centre_user",
                schema: "identity",
                table: "memberships");

            migrationBuilder.DropIndex(
                name: "ux_memberships_centre_user_role",
                schema: "identity",
                table: "memberships");

            migrationBuilder.DropCheckConstraint(
                name: "ck_memberships_role",
                schema: "identity",
                table: "memberships");

            migrationBuilder.AddCheckConstraint(
                name: "ck_memberships_role",
                schema: "identity",
                table: "memberships",
                sql: "role IN ('owner','teacher','secretary')");
        }
    }
}
