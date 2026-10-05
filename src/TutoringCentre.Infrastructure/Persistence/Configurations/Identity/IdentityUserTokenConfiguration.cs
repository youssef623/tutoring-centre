using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Identity;

/// <summary>Renames Identity's default tokens table into identity.user_tokens.</summary>
internal sealed class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("user_tokens", Schemas.Identity);
    }
}
