using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboundAndWebConversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_conversations_identity",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_conversations_parent",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_channel_accounts_phone",
                schema: "conversations",
                table: "channel_accounts");

            migrationBuilder.DropCheckConstraint(
                name: "ck_channel_accounts_provider",
                schema: "conversations",
                table: "channel_accounts");

            migrationBuilder.AlterColumn<string>(
                name: "counterpart_address",
                schema: "conversations",
                table: "conversations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<Guid>(
                name: "portal_user_id",
                schema: "conversations",
                table: "conversations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "student_id",
                schema: "conversations",
                table: "conversations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "display_phone",
                schema: "conversations",
                table: "channel_accounts",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.CreateTable(
                name: "outbound_messages",
                schema: "conversations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    reply_to_inbound_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    provider_message_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    failure_code = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbound_messages", x => x.id);
                    table.UniqueConstraint("ux_outbound_messages_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_outbound_messages_failure", "(status = 'failed') = (failure_code IS NOT NULL)");
                    table.CheckConstraint("ck_outbound_messages_sent", "status NOT IN ('sent','delivered','read') OR sent_at IS NOT NULL");
                    table.CheckConstraint("ck_outbound_messages_status", "status IN ('queued','sent','delivered','read','failed')");
                    table.CheckConstraint("ck_outbound_messages_text", "char_length(text) BETWEEN 1 AND 1000");
                    table.ForeignKey(
                        name: "fk_outbound_messages_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_outbound_messages_conversations",
                        columns: x => new { x.centre_id, x.conversation_id },
                        principalSchema: "conversations",
                        principalTable: "conversations",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_outbound_messages_inbound",
                        columns: x => new { x.centre_id, x.reply_to_inbound_id },
                        principalSchema: "conversations",
                        principalTable: "inbound_messages",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_conversations_account_portal_user",
                schema: "conversations",
                table: "conversations",
                columns: new[] { "centre_id", "channel_account_id", "portal_user_id" },
                unique: true,
                filter: "portal_user_id IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_conversations_address",
                schema: "conversations",
                table: "conversations",
                sql: "(counterpart_address IS NOT NULL) <> (portal_user_id IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_conversations_identity",
                schema: "conversations",
                table: "conversations",
                sql: "identity_status IN ('unresolved','parent','unknown','identified')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_conversations_parent",
                schema: "conversations",
                table: "conversations",
                sql: "(portal_user_id IS NULL AND identity_status <> 'identified' AND (identity_status = 'parent') = (parent_id IS NOT NULL) AND student_id IS NULL) OR (portal_user_id IS NOT NULL AND identity_status = 'identified' AND (parent_id IS NOT NULL) <> (student_id IS NOT NULL))");

            migrationBuilder.AddCheckConstraint(
                name: "ck_conversations_subject",
                schema: "conversations",
                table: "conversations",
                sql: "parent_id IS NULL OR student_id IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_channel_accounts_phone",
                schema: "conversations",
                table: "channel_accounts",
                sql: "(provider = 'web' AND display_phone IS NULL) OR (provider = 'whatsapp' AND display_phone IS NOT NULL AND display_phone ~ '^[+][1-9][0-9]{7,14}$')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_channel_accounts_provider",
                schema: "conversations",
                table: "channel_accounts",
                sql: "provider IN ('whatsapp','web')");

            migrationBuilder.CreateIndex(
                name: "ix_outbound_messages_conversation_created",
                schema: "conversations",
                table: "outbound_messages",
                columns: new[] { "centre_id", "conversation_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ux_outbound_messages_reply",
                schema: "conversations",
                table: "outbound_messages",
                columns: new[] { "centre_id", "reply_to_inbound_id" },
                unique: true,
                filter: "reply_to_inbound_id IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "fk_conversations_students",
                schema: "conversations",
                table: "conversations",
                columns: new[] { "centre_id", "student_id" },
                principalSchema: "academics",
                principalTable: "students",
                principalColumns: new[] { "centre_id", "id" },
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Conversations, "outbound_messages");
            migrationBuilder.Sql("revoke all on conversations.outbound_messages from tutoring_app; grant select, insert, update on conversations.outbound_messages to tutoring_app;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // A rollback must not erase web conversations or stored replies. Remove/reconcile new data explicitly first.
            migrationBuilder.Sql("""
                ALTER TABLE conversations.outbound_messages NO FORCE ROW LEVEL SECURITY;
                ALTER TABLE conversations.conversations NO FORCE ROW LEVEL SECURITY;
                ALTER TABLE conversations.channel_accounts NO FORCE ROW LEVEL SECURITY;
                DO $$ BEGIN
                  IF EXISTS (SELECT 1 FROM conversations.outbound_messages)
                    OR EXISTS (SELECT 1 FROM conversations.conversations WHERE portal_user_id IS NOT NULL)
                    OR EXISTS (SELECT 1 FROM conversations.channel_accounts WHERE provider = 'web') THEN
                    RAISE EXCEPTION 'Cannot roll back while web-channel or outbound data exists';
                  END IF;
                END $$;
                ALTER TABLE conversations.outbound_messages FORCE ROW LEVEL SECURITY;
                ALTER TABLE conversations.conversations FORCE ROW LEVEL SECURITY;
                ALTER TABLE conversations.channel_accounts FORCE ROW LEVEL SECURITY;
                """);
            migrationBuilder.DropForeignKey(
                name: "fk_conversations_students",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropTable(
                name: "outbound_messages",
                schema: "conversations");

            migrationBuilder.DropIndex(
                name: "ux_conversations_account_portal_user",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_conversations_address",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_conversations_identity",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_conversations_parent",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_conversations_subject",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_channel_accounts_phone",
                schema: "conversations",
                table: "channel_accounts");

            migrationBuilder.DropCheckConstraint(
                name: "ck_channel_accounts_provider",
                schema: "conversations",
                table: "channel_accounts");

            migrationBuilder.DropColumn(
                name: "portal_user_id",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "student_id",
                schema: "conversations",
                table: "conversations");

            migrationBuilder.AlterColumn<string>(
                name: "counterpart_address",
                schema: "conversations",
                table: "conversations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "display_phone",
                schema: "conversations",
                table: "channel_accounts",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_conversations_identity",
                schema: "conversations",
                table: "conversations",
                sql: "identity_status IN ('unresolved','parent','unknown')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_conversations_parent",
                schema: "conversations",
                table: "conversations",
                sql: "(identity_status = 'parent') = (parent_id IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_channel_accounts_phone",
                schema: "conversations",
                table: "channel_accounts",
                sql: "display_phone ~ '^[+][1-9][0-9]{7,14}$'");

            migrationBuilder.AddCheckConstraint(
                name: "ck_channel_accounts_provider",
                schema: "conversations",
                table: "channel_accounts",
                sql: "provider IN ('whatsapp')");
        }
    }
}
