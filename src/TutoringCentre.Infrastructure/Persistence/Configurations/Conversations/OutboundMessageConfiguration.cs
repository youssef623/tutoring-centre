using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Conversations;

internal sealed class OutboundMessageConfiguration : IEntityTypeConfiguration<OutboundMessage>
{
    public void Configure(EntityTypeBuilder<OutboundMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("outbound_messages", Schemas.Conversations, table =>
        {
            table.HasCheckConstraint("ck_outbound_messages_status", "status IN ('queued','sent','delivered','read','failed')");
            table.HasCheckConstraint("ck_outbound_messages_failure", "(status = 'failed') = (failure_code IS NOT NULL)");
            table.HasCheckConstraint("ck_outbound_messages_sent", "status NOT IN ('sent','delivered','read') OR sent_at IS NOT NULL");
            table.HasCheckConstraint("ck_outbound_messages_text", "char_length(text) BETWEEN 1 AND 1000");
        });
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned("outbound_messages");
        builder.Property(message => message.ConversationId).IsRequired();
        builder.Property(message => message.Text).HasMaxLength(1000).IsRequired();
        builder.Property(message => message.Status).HasConversion(status => status.ToString().ToLowerInvariant(), status => Enum.Parse<OutboundMessageStatus>(status, true)).HasMaxLength(12).IsRequired();
        builder.Property(message => message.ProviderMessageId).HasMaxLength(128);
        builder.Property(message => message.FailureCode).HasMaxLength(60);
        builder.Property(message => message.SentAt);
        builder.Property(message => message.ReplyToInboundId);
        builder.HasOne<Conversation>().WithMany().HasForeignKey(message => new { message.CentreId, message.ConversationId })
            .HasPrincipalKey(conversation => new { conversation.CentreId, conversation.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_outbound_messages_conversations");
        builder.HasOne<InboundMessage>().WithMany().HasForeignKey(message => new { message.CentreId, message.ReplyToInboundId })
            .HasPrincipalKey(message => new { message.CentreId, message.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_outbound_messages_inbound");
        builder.HasIndex(message => new { message.CentreId, message.ReplyToInboundId }).IsUnique().HasFilter("reply_to_inbound_id IS NOT NULL").HasDatabaseName("ux_outbound_messages_reply");
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.HasIndex("CentreId", "ConversationId", "CreatedAt").HasDatabaseName("ix_outbound_messages_conversation_created");
    }
}
