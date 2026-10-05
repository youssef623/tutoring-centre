using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Maps TenantProbe to probe.tenant_probes, created directly by SQL in PostgresFixture — never by a migration.</summary>
internal sealed class TenantProbeConfiguration : IEntityTypeConfiguration<TenantProbe>
{
    private const int LabelMaxLength = 50;

    public void Configure(EntityTypeBuilder<TenantProbe> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tenant_probes", "probe");

        builder.HasKey(probe => probe.Id);
        builder.Property(probe => probe.Id).ValueGeneratedNever();

        builder.Property(probe => probe.CentreId).IsRequired();

        builder.Property(probe => probe.Label).IsRequired().HasMaxLength(LabelMaxLength);
    }
}
