using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Conversations;

internal sealed class ChannelAccountConfiguration : IEntityTypeConfiguration<ChannelAccount>
{
    public void Configure(EntityTypeBuilder<ChannelAccount> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("channel_accounts", Schemas.Conversations, table =>
        {
            table.HasCheckConstraint("ck_channel_accounts_provider", "provider IN ('whatsapp','web')");
            table.HasCheckConstraint("ck_channel_accounts_status", "status IN ('active','disabled')");
            table.HasCheckConstraint("ck_channel_accounts_phone", "(provider = 'web' AND display_phone IS NULL) OR (provider = 'whatsapp' AND display_phone IS NOT NULL AND display_phone ~ '^[+][1-9][0-9]{7,14}$')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned("channel_accounts");
        builder.Property(x => x.Provider).HasConversion(x => x.ToString().ToLowerInvariant(), x => Enum.Parse<ChannelProvider>(x, true)).HasMaxLength(12).IsRequired();
        builder.Property(x => x.ExternalAccountId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.DisplayPhone).HasConversion(PhoneNumberConverters.Optional).HasMaxLength(16);
        builder.Property(x => x.Status).HasMaxLength(10).IsRequired();
        builder.HasIndex(x => new { x.Provider, x.ExternalAccountId }).IsUnique().HasDatabaseName("ux_channel_accounts_provider_external");
        builder.HasRowVersion();
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }
}
