using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Conversations;

internal sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("conversations", Schemas.Conversations, table =>
        {
            table.HasCheckConstraint("ck_conversations_identity", "identity_status IN ('unresolved','parent','unknown','identified')");
            table.HasCheckConstraint("ck_conversations_parent", "(portal_user_id IS NULL AND identity_status <> 'identified' AND (identity_status = 'parent') = (parent_id IS NOT NULL) AND student_id IS NULL) OR (portal_user_id IS NOT NULL AND identity_status = 'identified' AND (parent_id IS NOT NULL) <> (student_id IS NOT NULL))");
            table.HasCheckConstraint("ck_conversations_address", "(counterpart_address IS NOT NULL) <> (portal_user_id IS NOT NULL)");
            table.HasCheckConstraint("ck_conversations_subject", "parent_id IS NULL OR student_id IS NULL");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned("conversations");
        builder.Property(x => x.ChannelAccountId).IsRequired();
        builder.Property(x => x.CounterpartAddress).HasMaxLength(20);
        builder.Property(x => x.PortalUserId);
        builder.Property(x => x.StudentId);
        builder.Property(x => x.CounterpartPhone).HasConversion(PhoneNumberConverters.Optional).HasMaxLength(16);
        builder.Property(x => x.IdentityStatus).HasConversion(x => x.ToString().ToLowerInvariant(), x => Enum.Parse<IdentityStatus>(x, true)).HasMaxLength(12).IsRequired();
        builder.Property(x => x.LastMessageAt).IsRequired();
        builder.HasOne<ChannelAccount>().WithMany().HasForeignKey(x => new { x.CentreId, x.ChannelAccountId })
            .HasPrincipalKey(x => new { x.CentreId, x.Id }).HasConstraintName("fk_conversations_channel_accounts").OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Parent>().WithMany().HasForeignKey(x => new { x.CentreId, x.ParentId })
            .HasPrincipalKey(x => new { x.CentreId, x.Id }).HasConstraintName("fk_conversations_parents").OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CentreId, x.ChannelAccountId, x.CounterpartAddress }).IsUnique().HasDatabaseName("ux_conversations_account_address");
        builder.HasOne<Student>().WithMany().HasForeignKey(x => new { x.CentreId, x.StudentId })
            .HasPrincipalKey(x => new { x.CentreId, x.Id }).HasConstraintName("fk_conversations_students").OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CentreId, x.ChannelAccountId, x.PortalUserId }).IsUnique()
            .HasFilter("portal_user_id IS NOT NULL").HasDatabaseName("ux_conversations_account_portal_user");
        builder.HasIndex(x => new { x.CentreId, x.LastMessageAt, x.Id }).IsDescending(false, true, true).HasDatabaseName("ix_conversations_centre_last");
        builder.HasRowVersion();
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }
}
