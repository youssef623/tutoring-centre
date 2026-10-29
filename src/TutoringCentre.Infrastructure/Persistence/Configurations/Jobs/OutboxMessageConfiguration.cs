using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Messaging;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Jobs;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("outbox_messages", Schemas.Jobs, table =>
        {
            table.HasCheckConstraint("ck_outbox_messages_outcome", "outcome IN ('succeeded', 'failed')");
            table.HasCheckConstraint("ck_outbox_messages_processed", "(processed_at IS NULL) = (outcome IS NULL)");
            table.HasCheckConstraint("ck_outbox_messages_attempts", "attempts >= 0");
        });
        builder.HasKey(message => message.Id).HasName("pk_outbox_messages");
        builder.Property(message => message.Id).ValueGeneratedNever();
        builder.Property(message => message.Type).HasMaxLength(100).IsRequired();
        builder.Property(message => message.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(message => message.CorrelationId).HasMaxLength(64);
        builder.Property(message => message.ClaimedBy).HasMaxLength(64);
        builder.Property(message => message.Outcome).HasMaxLength(12);
        builder.Property(message => message.LastError).HasMaxLength(200);
        builder.Property(message => message.Attempts).HasDefaultValue(0);
        builder.HasOne<Centre>().WithMany().HasForeignKey(message => message.CentreId)
            .HasConstraintName("fk_outbox_messages_centres").OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(message => new { message.AvailableAt, message.Id }).HasDatabaseName("ix_outbox_messages_pending")
            .HasFilter("processed_at IS NULL AND dead_lettered_at IS NULL");
        builder.HasIndex(message => message.ProcessedAt).HasDatabaseName("ix_outbox_messages_processed")
            .HasFilter("processed_at IS NOT NULL");
    }
}
