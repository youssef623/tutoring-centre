using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Identity;

/// <summary>Maps ApplicationUser to identity.users. Applied after Identity's own base configuration, so it wins.</summary>
internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    private const int DisplayNameMaxLength = 120;
    private const int PreferredLocaleMaxLength = 2;

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            "users",
            Schemas.Identity,
            table => table.HasCheckConstraint("ck_users_preferred_locale", "preferred_locale IN ('ar','en')"));

        builder.Property(user => user.DisplayName).IsRequired().HasMaxLength(DisplayNameMaxLength);
        builder.Property(user => user.PreferredLocale).IsRequired().HasMaxLength(PreferredLocaleMaxLength);
        builder.Property(user => user.MustChangePassword).IsRequired();

        builder.HasIndex(user => user.NormalizedEmail).IsUnique().HasDatabaseName("ux_users_normalized_email");
    }
}
