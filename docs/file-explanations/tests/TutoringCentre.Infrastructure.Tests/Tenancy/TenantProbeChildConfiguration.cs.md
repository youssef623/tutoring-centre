# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps TenantProbeChild to probe.tenant_probe_children, created directly by SQL in PostgresFixture — never by a migration.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,578 bytes. Source hash: `a384f7ac213b626bd41d56203acce7734cba6a474e9116f12f76055bbb153331`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using TutoringCentre.Infrastructure.Persistence.Configurations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs#L8): `internal sealed class TenantProbeChildConfiguration : IEntityTypeConfiguration<TenantProbeChild>`
- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs#L12): `public void Configure(EntityTypeBuilder<TenantProbeChild> builder)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs:24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs#L24)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
