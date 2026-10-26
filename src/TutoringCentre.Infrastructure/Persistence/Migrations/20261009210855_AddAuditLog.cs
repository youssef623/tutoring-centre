using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.CreateTable(
                name: "audit_entries",
                schema: "audit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    actor_type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    actor_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_entries", x => x.id);
                    table.UniqueConstraint("ux_audit_entries_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_audit_entries_action", "action IN ('created','updated','deleted')");
                    table.CheckConstraint("ck_audit_entries_actor", "(actor_type = 'staff') = (actor_user_id IS NOT NULL)");
                    table.CheckConstraint("ck_audit_entries_actor_type", "actor_type IN ('staff','system')");
                    table.ForeignKey(
                        name: "fk_audit_entries_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_audit_entries_users",
                        column: x => x.actor_user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_entries_actor_user_id",
                schema: "audit",
                table: "audit_entries",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_entries_centre_entity",
                schema: "audit",
                table: "audit_entries",
                columns: new[] { "centre_id", "entity_type", "entity_id", "occurred_at" },
                descending: new[] { false, false, false, true });

            migrationBuilder.CreateIndex(
                name: "ix_audit_entries_centre_occurred",
                schema: "audit",
                table: "audit_entries",
                columns: new[] { "centre_id", "occurred_at", "id" },
                descending: new[] { false, true, true });

            // Append-only grant, then row-level security — in that order, after the table exists, so the table
            // never exists without its policy (Task 31.3). SELECT and INSERT only: tutoring_app is never granted
            // UPDATE or DELETE, on this table or on any future table in this schema.
            migrationBuilder.GrantAppendOnly(Schemas.Audit, "audit_entries");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Audit, "audit_entries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Audit, "audit_entries");
            migrationBuilder.RevokeAppendOnly(Schemas.Audit, "audit_entries");

            migrationBuilder.DropTable(
                name: "audit_entries",
                schema: "audit");
        }
    }
}
