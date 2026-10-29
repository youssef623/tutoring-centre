using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "conversations");

            migrationBuilder.GrantRuntimeAccess(Schemas.Conversations);

            migrationBuilder.CreateTable(
                name: "channel_accounts",
                schema: "conversations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    external_account_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    display_phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channel_accounts", x => x.id);
                    table.UniqueConstraint("ux_channel_accounts_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_channel_accounts_phone", "display_phone ~ '^[+][1-9][0-9]{7,14}$'");
                    table.CheckConstraint("ck_channel_accounts_provider", "provider IN ('whatsapp')");
                    table.CheckConstraint("ck_channel_accounts_status", "status IN ('active','disabled')");
                    table.ForeignKey(
                        name: "fk_channel_accounts_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                schema: "conversations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    counterpart_address = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    counterpart_phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    identity_status = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    identity_resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_message_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conversations", x => x.id);
                    table.UniqueConstraint("ux_conversations_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_conversations_identity", "identity_status IN ('unresolved','parent','unknown')");
                    table.CheckConstraint("ck_conversations_parent", "(identity_status = 'parent') = (parent_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_conversations_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_conversations_channel_accounts",
                        columns: x => new { x.centre_id, x.channel_account_id },
                        principalSchema: "conversations",
                        principalTable: "channel_accounts",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_conversations_parents",
                        columns: x => new { x.centre_id, x.parent_id },
                        principalSchema: "academics",
                        principalTable: "parents",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inbound_messages",
                schema: "conversations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    provider_message_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    message_type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    text_body = table.Column<string>(type: "text", nullable: true),
                    provider_timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbound_messages", x => x.id);
                    table.UniqueConstraint("ux_inbound_messages_centre_id_id", x => new { x.centre_id, x.id });
                    table.CheckConstraint("ck_inbound_messages_length", "char_length(text_body) <= 4096");
                    table.CheckConstraint("ck_inbound_messages_processed", "(status = 'processed') = (processed_at IS NOT NULL)");
                    table.CheckConstraint("ck_inbound_messages_status", "status IN ('received','processed')");
                    table.CheckConstraint("ck_inbound_messages_text", "(message_type = 'text') = (text_body IS NOT NULL)");
                    table.CheckConstraint("ck_inbound_messages_type", "message_type IN ('text','audio','image','other')");
                    table.ForeignKey(
                        name: "fk_inbound_messages_centres",
                        column: x => x.centre_id,
                        principalSchema: "platform",
                        principalTable: "centres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_inbound_messages_conversations",
                        columns: x => new { x.centre_id, x.conversation_id },
                        principalSchema: "conversations",
                        principalTable: "conversations",
                        principalColumns: new[] { "centre_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_channel_accounts_provider_external",
                schema: "conversations",
                table: "channel_accounts",
                columns: new[] { "provider", "external_account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_conversations_centre_last",
                schema: "conversations",
                table: "conversations",
                columns: new[] { "centre_id", "last_message_at", "id" },
                descending: new[] { false, true, true });

            migrationBuilder.CreateIndex(
                name: "ux_conversations_account_address",
                schema: "conversations",
                table: "conversations",
                columns: new[] { "centre_id", "channel_account_id", "counterpart_address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inbound_messages_conversation",
                schema: "conversations",
                table: "inbound_messages",
                columns: new[] { "centre_id", "conversation_id", "received_at" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "ux_inbound_messages_provider_id",
                schema: "conversations",
                table: "inbound_messages",
                columns: new[] { "centre_id", "provider", "provider_message_id" },
                unique: true);
            migrationBuilder.Sql("revoke delete on conversations.channel_accounts, conversations.conversations, conversations.inbound_messages from tutoring_app;");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Conversations, "conversations");
            migrationBuilder.EnableTenantRowLevelSecurity(Schemas.Conversations, "inbound_messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inbound_messages",
                schema: "conversations");

            migrationBuilder.DropTable(
                name: "conversations",
                schema: "conversations");

            migrationBuilder.DropTable(
                name: "channel_accounts",
                schema: "conversations");
            migrationBuilder.RevokeRuntimeAccess(Schemas.Conversations);
            migrationBuilder.DropSchema(name: "conversations");
        }
    }
}
