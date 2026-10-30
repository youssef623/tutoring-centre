using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Conversations;

internal sealed class InboundMessageConfiguration : IEntityTypeConfiguration<InboundMessage>
{
    public void Configure(EntityTypeBuilder<InboundMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("inbound_messages", Schemas.Conversations, table =>
        {
            table.HasCheckConstraint("ck_inbound_messages_type", "message_type IN ('text','audio','image','other')");
            table.HasCheckConstraint("ck_inbound_messages_status", "status IN ('received','processed')");
            table.HasCheckConstraint("ck_inbound_messages_text", "(message_type = 'text') = (text_body IS NOT NULL)");
            table.HasCheckConstraint("ck_inbound_messages_length", "char_length(text_body) <= 4096");
            table.HasCheckConstraint("ck_inbound_messages_processed", "(status = 'processed') = (processed_at IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned("inbound_messages");
        builder.Property(x => x.ConversationId).IsRequired();
        builder.Property(x => x.Provider).HasConversion(x => x.ToString().ToLowerInvariant(), x => Enum.Parse<ChannelProvider>(x, true)).HasMaxLength(12).IsRequired();
        builder.Property(x => x.ProviderMessageId).HasMaxLength(128).IsRequired();
        builder.Property(x => x.MessageType).HasConversion(x => x.ToString().ToLowerInvariant(), x => Enum.Parse<InboundMessageType>(x, true)).HasMaxLength(16).IsRequired();
        builder.Property(x => x.ProviderTimestamp).IsRequired();
        builder.Property(x => x.ReceivedAt).IsRequired();
        builder.Property(x => x.TextBody).HasColumnType("text");
        builder.Property(x => x.Status).HasConversion(x => x.ToString().ToLowerInvariant(), x => Enum.Parse<InboundMessageStatus>(x, true)).HasMaxLength(10).IsRequired();
        builder.HasOne<Conversation>().WithMany().HasForeignKey(x => new { x.CentreId, x.ConversationId })
            .HasPrincipalKey(x => new { x.CentreId, x.Id }).HasConstraintName("fk_inbound_messages_conversations").OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CentreId, x.Provider, x.ProviderMessageId }).IsUnique().HasDatabaseName("ux_inbound_messages_provider_id");
        builder.HasIndex(x => new { x.CentreId, x.ConversationId, x.ReceivedAt }).IsDescending(false, false, true).HasDatabaseName("ix_inbound_messages_conversation");
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
    }
}
