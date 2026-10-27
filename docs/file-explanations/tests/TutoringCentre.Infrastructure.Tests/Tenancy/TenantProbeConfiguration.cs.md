# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps TenantProbe to probe.tenant_probes, created directly by SQL in PostgresFixture — never by a migration.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeConfiguration.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,092 bytes. Source hash: `3e7b14e4badb25a765bac219a4bc304bc00ac421c4d0d10fda4f90d2d6d366f4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using TutoringCentre.Infrastructure.Persistence.Configurations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeConfiguration.cs#L8): `internal sealed class TenantProbeConfiguration : IEntityTypeConfiguration<TenantProbe>`
- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeConfiguration.cs#L12): `public void Configure(EntityTypeBuilder<TenantProbe> builder)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs:23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs#L23)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
