using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Infrastructure.Persistence.Configurations;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Maps TenantProbeChild to probe.tenant_probe_children, created directly by SQL in PostgresFixture — never by a migration.</summary>
internal sealed class TenantProbeChildConfiguration : IEntityTypeConfiguration<TenantProbeChild>
{
    private const int LabelMaxLength = 50;

    public void Configure(EntityTypeBuilder<TenantProbeChild> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tenant_probe_children", "probe");

        builder.HasKey(child => child.Id);
        builder.Property(child => child.Id).ValueGeneratedNever();

        builder.Property(child => child.Label).IsRequired().HasMaxLength(LabelMaxLength);

        builder.ConfigureTenantOwned("tenant_probe_children");

        // Task 21.6: composite foreign key (centre_id, probe_id) -> probe.tenant_probes(centre_id, id) — targets
        // the alternate key ConfigureTenantOwned gave TenantProbe, so a child can never name a probe that exists,
        // but in a different centre.
        builder
            .HasOne<TenantProbe>()
            .WithMany()
            .HasForeignKey(child => new { child.CentreId, child.ProbeId })
            .HasPrincipalKey(probe => new { probe.CentreId, probe.Id })
            .HasConstraintName("fk_tenant_probe_children_tenant_probes")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
