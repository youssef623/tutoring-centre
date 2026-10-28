using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_cards",
                schema: "attendance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    issued_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    revocation_reason = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_cards", x => x.id);
                    table.UniqueConstraint("ux_student_cards_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_student_cards_hash_length", "octet_length(token_hash) = 32");
                    table.CheckConstraint("ck_student_cards_reason", "revocation_reason IN ('reissued','lost','damaged','withdrawn')");
                    table.CheckConstraint("ck_student_cards_revocation", "(status = 'revoked') = (revoked_at IS NOT NULL AND revocation_reason IS NOT NULL)");
                    table.CheckConstraint("ck_student_cards_status", "status IN ('active','revoked')");
                    table.ForeignKey(
                        name: "fk_student_cards_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_cards_issued_by",
                        column: x => x.issued_by_user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_cards_revoked_by",
                        column: x => x.revoked_by_user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_cards_students",
                        columns: x => new { x.centre_id, x.student_id },
                        principalSchema: "academics",
                        principalTable: "students",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_student_cards_active_student",
                schema: "attendance",
                table: "student_cards",
                columns: new[] { "centre_id", "student_id" },
                unique: true,
                filter: "status = 'active'");

            migrationBuilder.CreateIndex(
                name: "ux_student_cards_centre_token_hash",
                schema: "attendance",
                table: "student_cards",
                columns: new[] { "centre_id", "token_hash" },
                unique: true);
            migrationBuilder.Sql($"revoke delete on attendance.student_cards from {DatabaseRoles.App};");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Attendance, "student_cards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Attendance, "student_cards");
            migrationBuilder.DropTable(
                name: "student_cards",
                schema: "attendance");
        }
    }
}
