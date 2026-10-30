using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentRuns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "assistant");

            migrationBuilder.CreateTable(
                name: "agent_runs",
                schema: "assistant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inbound_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    steps = table.Column<int>(type: "integer", nullable: false),
                    input_tokens = table.Column<int>(type: "integer", nullable: false),
                    output_tokens = table.Column<int>(type: "integer", nullable: false),
                    failure_code = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    guard_reason = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ux_agent_runs_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_agent_runs_counts", "steps >= 0 AND input_tokens >= 0 AND output_tokens >= 0");
                    table.CheckConstraint("ck_agent_runs_failure", "(status = 'failed') = (failure_code IS NOT NULL)");
                    table.CheckConstraint("ck_agent_runs_finished", "(status = 'running') = (finished_at IS NULL)");
                    table.CheckConstraint("ck_agent_runs_status", "status IN ('running','completed','failed')");
                    table.ForeignKey(
                        name: "fk_agent_runs_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_agent_runs_conversations",
                        columns: x => new { x.centre_id, x.conversation_id },
                        principalSchema: "conversations",
                        principalTable: "conversations",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_agent_runs_inbound_messages",
                        columns: x => new { x.centre_id, x.inbound_message_id },
                        principalSchema: "conversations",
                        principalTable: "inbound_messages",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agent_runs_centre_id_conversation_id",
                schema: "assistant",
                table: "agent_runs",
                columns: new[] { "centre_id", "conversation_id" });

            migrationBuilder.CreateIndex(
                name: "ux_agent_runs_inbound_message",
                schema: "assistant",
                table: "agent_runs",
                columns: new[] { "centre_id", "inbound_message_id" },
                unique: true);
            migrationBuilder.GrantRuntimeAccess("assistant");
            migrationBuilder.EnableTenantRowLevelSecurity("assistant", "agent_runs");
            migrationBuilder.Sql("grant select, insert, update on assistant.agent_runs to tutoring_app;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_runs",
                schema: "assistant");
        }
    }
}
