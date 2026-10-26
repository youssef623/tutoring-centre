using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Infrastructure.Persistence.Configurations;

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

        builder.Property(probe => probe.Label).IsRequired().HasMaxLength(LabelMaxLength);

        // Task 21.6: centre_id required, restrict FK to platform.centres, unique (centre_id, id) — what the child
        // probe's own composite foreign key targets.
        builder.ConfigureTenantOwned("tenant_probes");
    }
}
