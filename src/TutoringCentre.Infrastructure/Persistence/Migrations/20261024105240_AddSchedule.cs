using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, before any table: the exclusion constraints below need btree_gist for "=" on uuid inside a
            // GiST index. A trusted contrib extension, so tutoring_owner (CREATE on the database) installs it
            // without a superuser. If the server does not ship it, this statement fails and the migration's
            // transaction rolls back before any table exists.
            migrationBuilder.Sql("create extension if not exists btree_gist;");

            migrationBuilder.CreateTable(
                name: "schedule_rules",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<short>(type: "smallint", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    room = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schedule_rules", x => x.id);
                    table.UniqueConstraint("ux_schedule_rules_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_schedule_rules_dates", "effective_to IS NULL OR effective_to >= effective_from");
                    table.CheckConstraint("ck_schedule_rules_day", "day_of_week BETWEEN 0 AND 6");
                    table.CheckConstraint("ck_schedule_rules_duration", "duration_minutes BETWEEN 30 AND 240");
                    table.ForeignKey(
                        name: "fk_schedule_rules_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_schedule_rules_groups",
                        columns: x => new { x.centre_id, x.group_id },
                        principalSchema: "academics",
                        principalTable: "groups",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "class_sessions",
                schema: "academics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    teacher_id = table.Column<Guid>(type: "uuid", nullable: false),
                    schedule_rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    local_date = table.Column<DateOnly>(type: "date", nullable: false),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    room = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_sessions", x => x.id);
                    table.UniqueConstraint("ux_class_sessions_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_class_sessions_range", "ends_at > starts_at");
                    table.CheckConstraint("ck_class_sessions_status", "status IN ('scheduled','cancelled')");
                    table.ForeignKey(
                        name: "fk_class_sessions_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_class_sessions_groups",
                        columns: x => new { x.centre_id, x.group_id },
                        principalSchema: "academics",
                        principalTable: "groups",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_class_sessions_rules",
                        columns: x => new { x.centre_id, x.schedule_rule_id },
                        principalSchema: "academics",
                        principalTable: "schedule_rules",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_class_sessions_teachers",
                        columns: x => new { x.centre_id, x.teacher_id },
                        principalSchema: "academics",
                        principalTable: "teachers",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_class_sessions_centre_date",
                schema: "academics",
                table: "class_sessions",
                columns: new[] { "centre_id", "local_date", "starts_at" });

            migrationBuilder.CreateIndex(
                name: "ix_class_sessions_centre_group_date",
                schema: "academics",
                table: "class_sessions",
                columns: new[] { "centre_id", "group_id", "local_date" });

            migrationBuilder.CreateIndex(
                name: "ix_class_sessions_centre_teacher",
                schema: "academics",
                table: "class_sessions",
                columns: new[] { "centre_id", "teacher_id" });

            migrationBuilder.CreateIndex(
                name: "ux_class_sessions_rule_date",
                schema: "academics",
                table: "class_sessions",
                columns: new[] { "centre_id", "schedule_rule_id", "local_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_schedule_rules_centre_group",
                schema: "academics",
                table: "schedule_rules",
                columns: new[] { "centre_id", "group_id" });

            // No teacher and no group is in two scheduled sessions at once (ADR 0012). EF cannot model an
            // exclusion constraint, so it is written here. Half-open '[)' so a session ending at 19:00 and one
            // starting at 19:00 do not overlap; partial on status so a cancelled session frees its slot;
            // centre_id first so one centre's schedule can never block another's. The group constraint is created
            // first on purpose: PostgreSQL checks a row's indexes in creation order and reports the first violation,
            // and a session overlapping its own group usually overlaps its teacher too — "this group already meets
            // then" is the more precise answer for that case.
            migrationBuilder.Sql(
                $"alter table {Schemas.Academics}.class_sessions add constraint ex_class_sessions_group_overlap "
                + "exclude using gist (centre_id with =, group_id with =, tstzrange(starts_at, ends_at, '[)') with &&) "
                + "where (status = 'scheduled');");
            migrationBuilder.Sql(
                $"alter table {Schemas.Academics}.class_sessions add constraint ex_class_sessions_teacher_overlap "
                + "exclude using gist (centre_id with =, teacher_id with =, tstzrange(starts_at, ends_at, '[)') with &&) "
                + "where (status = 'scheduled');");

            // Grants, then row-level security, after the tables exist (following AddEnrollments). No delete: rules
            // are ended and sessions are cancelled, never removed.
            migrationBuilder.Sql($"grant select, insert, update on {Schemas.Academics}.schedule_rules to {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke delete on {Schemas.Academics}.schedule_rules from {DatabaseRoles.App};");
            migrationBuilder.Sql($"grant select, insert, update on {Schemas.Academics}.class_sessions to {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke delete on {Schemas.Academics}.class_sessions from {DatabaseRoles.App};");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "schedule_rules");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Academics, "class_sessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "class_sessions");
            migrationBuilder.DisableTenantRowLevelSecurity(Schemas.Academics, "schedule_rules");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Academics}.class_sessions from {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Academics}.schedule_rules from {DatabaseRoles.App};");
            migrationBuilder.Sql($"alter table {Schemas.Academics}.class_sessions drop constraint ex_class_sessions_teacher_overlap;");
            migrationBuilder.Sql($"alter table {Schemas.Academics}.class_sessions drop constraint ex_class_sessions_group_overlap;");

            // btree_gist stays installed: other objects may come to depend on it, and removing an extension is
            // not this migration's decision.
            migrationBuilder.DropTable(
                name: "class_sessions",
                schema: "academics");

            migrationBuilder.DropTable(
                name: "schedule_rules",
                schema: "academics");
        }
    }
}
