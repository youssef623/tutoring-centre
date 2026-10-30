using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddToolCalls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "tool_calls",
                schema: "assistant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step = table.Column<int>(type: "integer", nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    tool_name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    decision = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    reason_code = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    argument_summary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    result_summary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    duration_ms = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ux_tool_calls_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_tool_calls_decision", "decision IN ('allowed','denied','invalid','failed')");
                    table.CheckConstraint("ck_tool_calls_duration", "duration_ms >= 0");
                    table.CheckConstraint("ck_tool_calls_reason", "decision = 'allowed' OR reason_code IS NOT NULL");
                    table.ForeignKey(
                        name: "fk_tool_calls_agent_runs_centre_id_agent_run_id",
                        columns: x => new { x.centre_id, x.agent_run_id },
                        principalSchema: "assistant",
                        principalTable: "agent_runs",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tool_calls_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tool_calls_centre_id_agent_run_id",
                schema: "assistant",
                table: "tool_calls",
                columns: new[] { "centre_id", "agent_run_id" });

            migrationBuilder.CreateIndex(
                name: "ix_tool_calls_centre_id_agent_run_id_step_ordinal",
                schema: "assistant",
                table: "tool_calls",
                columns: new[] { "centre_id", "agent_run_id", "step", "ordinal" },
                unique: true);
            migrationBuilder.EnableTenantRowLevelSecurity("assistant", "tool_calls");
            migrationBuilder.Sql("revoke all on assistant.tool_calls from tutoring_app; grant select, insert on assistant.tool_calls to tutoring_app;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tool_calls",
                schema: "assistant");


        }
    }
}
