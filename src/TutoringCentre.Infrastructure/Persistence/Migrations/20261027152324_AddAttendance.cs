using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "attendance");

            migrationBuilder.GrantRuntimeAccess(Schemas.Attendance);

            migrationBuilder.AddUniqueConstraint(
                name: "ux_enrollments_centre_id_student",
                schema: "academics",
                table: "enrollments",
                columns: new[] { "centre_id", "id", "student_id" });

            migrationBuilder.CreateTable(
                name: "attendance_records",
                schema: "attendance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    enrollment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    source = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    marked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    marked_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    correction_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_corrected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_corrected_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    last_correction_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attendance_records", x => x.id);
                    table.UniqueConstraint("ux_attendance_records_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_attendance_records_corrections", "correction_count >= 0 AND (correction_count = 0) = (last_corrected_at IS NULL)");
                    table.CheckConstraint("ck_attendance_records_source", "source IN ('manual','qr')");
                    table.CheckConstraint("ck_attendance_records_status", "status IN ('present','late','absent','excused')");
                    table.ForeignKey(
                        name: "fk_attendance_records_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_attendance_records_corrected_by",
                        column: x => x.last_corrected_by_user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_attendance_records_enrollments",
                        columns: x => new { x.centre_id, x.enrollment_id, x.student_id },
                        principalSchema: "academics",
                        principalTable: "enrollments",
                        principalColumns: new[] { "centre_id", "id", "student_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_attendance_records_marked_by",
                        column: x => x.marked_by_user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_attendance_records_sessions",
                        columns: x => new { x.centre_id, x.session_id },
                        principalSchema: "academics",
                        principalTable: "class_sessions",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_attendance_records_students",
                        columns: x => new { x.centre_id, x.student_id },
                        principalSchema: "academics",
                        principalTable: "students",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_attendance_records_centre_student",
                schema: "attendance",
                table: "attendance_records",
                columns: new[] { "centre_id", "student_id" });

            migrationBuilder.CreateIndex(
                name: "ux_attendance_records_session_student",
                schema: "attendance",
                table: "attendance_records",
                columns: new[] { "centre_id", "session_id", "student_id" },
                unique: true);

            migrationBuilder.Sql($"revoke delete on {Schemas.Attendance}.attendance_records from {DatabaseRoles.App};");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Attendance, "attendance_records");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Attendance, "attendance_records");
            migrationBuilder.DropTable(
                name: "attendance_records",
                schema: "attendance");

            migrationBuilder.RevokeRuntimeAccess(Schemas.Attendance);
            migrationBuilder.DropSchema(name: "attendance");

            migrationBuilder.DropUniqueConstraint(
                name: "ux_enrollments_centre_id_student",
                schema: "academics",
                table: "enrollments");
        }
    }
}
