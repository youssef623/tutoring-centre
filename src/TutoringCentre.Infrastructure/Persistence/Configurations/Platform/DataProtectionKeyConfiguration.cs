using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Platform;

/// <summary>
/// Maps the ASP.NET Core Data Protection key ring (Task 34.6) to platform.data_protection_keys — platform-level,
/// not tenant-owned, so it carries no centre column and no row-level security, and is the one place AppDbContext
/// exposes a DbSet property (<see cref="AppDbContext.DataProtectionKeys"/>) rather than Set&lt;T&gt;(): the
/// <c>IDataProtectionKeyContext</c> interface the Data Protection stack persists through requires exactly that.
/// </summary>
internal sealed class DataProtectionKeyConfiguration : IEntityTypeConfiguration<DataProtectionKey>
{
    public void Configure(EntityTypeBuilder<DataProtectionKey> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("data_protection_keys", Schemas.Platform);
        builder.HasKey(key => key.Id);
    }
}
