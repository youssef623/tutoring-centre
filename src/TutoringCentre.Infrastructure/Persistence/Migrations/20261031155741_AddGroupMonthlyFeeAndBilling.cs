using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupMonthlyFeeAndBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "billing");

            migrationBuilder.AddColumn<decimal>(
                name: "monthly_fee",
                schema: "academics",
                table: "groups",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.CreateTable(
                name: "charges",
                schema: "billing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    enrollment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    teacher_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    billing_month = table.Column<DateOnly>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ux_charges_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_charges_due", "due_date >= billing_month");
                    table.CheckConstraint("ck_charges_kind_amount", "(kind='monthly' AND amount>0) OR (kind='adjustment' AND amount<>0 AND reason IS NOT NULL)");
                    table.CheckConstraint("ck_charges_month", "extract(day from billing_month)=1");
                    table.ForeignKey(
                        name: "fk_charges_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_charges_enrollments",
                        columns: x => new { x.centre_id, x.enrollment_id, x.student_id },
                        principalSchema: "academics",
                        principalTable: "enrollments",
                        principalColumns: new[] { "centre_id", "id", "student_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_charges_groups",
                        columns: x => new { x.centre_id, x.group_id },
                        principalSchema: "academics",
                        principalTable: "groups",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_charges_subjects",
                        columns: x => new { x.centre_id, x.subject_id },
                        principalSchema: "academics",
                        principalTable: "subjects",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_charges_teachers",
                        columns: x => new { x.centre_id, x.teacher_id },
                        principalSchema: "academics",
                        principalTable: "teachers",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_monthly_fees",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    effective_month = table.Column<DateOnly>(type: "date", nullable: false),
                    fee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_monthly_fees", x => x.id);
                    table.UniqueConstraint("ux_group_monthly_fees_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_group_monthly_fees_month", "extract(day from effective_month)=1");
                    table.CheckConstraint("ck_group_monthly_fees_nonnegative", "fee >= 0");
                    table.ForeignKey(
                        name: "fk_group_monthly_fees_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_group_monthly_fees_groups_centre_id_group_id",
                        columns: x => new { x.centre_id, x.group_id },
                        principalSchema: "academics",
                        principalTable: "groups",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_groups_monthly_fee",
                schema: "academics",
                table: "groups",
                sql: "monthly_fee >= 0");

            migrationBuilder.CreateIndex(
                name: "ix_charges_centre_id_enrollment_id_student_id",
                schema: "billing",
                table: "charges",
                columns: new[] { "centre_id", "enrollment_id", "student_id" });

            migrationBuilder.CreateIndex(
                name: "ix_charges_centre_id_group_id",
                schema: "billing",
                table: "charges",
                columns: new[] { "centre_id", "group_id" });

            migrationBuilder.CreateIndex(
                name: "ix_charges_centre_id_subject_id",
                schema: "billing",
                table: "charges",
                columns: new[] { "centre_id", "subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_charges_centre_id_teacher_id",
                schema: "billing",
                table: "charges",
                columns: new[] { "centre_id", "teacher_id" });

            migrationBuilder.CreateIndex(
                name: "ix_charges_student_due",
                schema: "billing",
                table: "charges",
                columns: new[] { "centre_id", "student_id", "due_date" });

            migrationBuilder.CreateIndex(
                name: "ux_charges_monthly",
                schema: "billing",
                table: "charges",
                columns: new[] { "centre_id", "student_id", "subject_id", "billing_month" },
                unique: true,
                filter: "kind='monthly'");

            migrationBuilder.CreateIndex(
                name: "ux_group_monthly_fees_month",
                schema: "academics",
                table: "group_monthly_fees",
                columns: new[] { "centre_id", "group_id", "effective_month" },
                unique: true);

            migrationBuilder.Sql("revoke all privileges on billing.charges from tutoring_app;");
            migrationBuilder.GrantAppendOnly("billing", "charges");
            migrationBuilder.EnableTenantRowLevelSecurity("billing", "charges");
            migrationBuilder.EnableTenantRowLevelSecurity("academics", "group_monthly_fees");
            migrationBuilder.Sql("grant select, insert, update on academics.group_monthly_fees to tutoring_app;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RevokeAppendOnly("billing", "charges");
            migrationBuilder.DropTable(
                name: "charges",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "group_monthly_fees",
                schema: "academics");

            migrationBuilder.DropCheckConstraint(
                name: "ck_groups_monthly_fee",
                schema: "academics",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "monthly_fee",
                schema: "academics",
                table: "groups");
        }
    }
}
