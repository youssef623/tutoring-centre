# `src/TutoringCentre.Infrastructure/Persistence/Configurations/TenantOwnedConfiguration.cs`

## Purpose

The shared mapping establishes required centre ownership, a Restrict reference to platform.centres, and a composite alternate key on centre and entity ID. Child composite foreign keys can then prove both existence and same-centre ownership. A plain unique index is insufficient as the principal key target used by EF relationship mapping.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/TenantOwnedConfiguration.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,772 bytes. Source hash: `319804ab88c0cb36a1e87bf03d2a8783221e4e7cd3bf4256b55de78aa47b2fed`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using TutoringCentre.Domain.Centres;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/TenantOwnedConfiguration.cs#L15): `internal static class TenantOwnedConfiguration`
- [Line 17](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/TenantOwnedConfiguration.cs#L17): `public static void ConfigureTenantOwned<TEntity>(this EntityTypeBuilder<TEntity> builder, string tableName)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence.Configurations;

/// <summary>
/// What every tenant-owned entity's own IEntityTypeConfiguration calls (the manual's ConfigureTenantOwned, Task
/// 21.6): a required centre_id, a restrict foreign key to platform.centres, and a unique key on (centre_id, id).
/// That composite key is what lets a tenant-owned child's own foreign key reference "this row, in this centre"
/// as a single unit (Task 21.6's child probe) — a plain foreign key on the parent id alone could only ever prove
/// the parent exists somewhere, not that it exists in the same centre as the child.
/// </summary>
internal static class TenantOwnedConfiguration
{
    public static void ConfigureTenantOwned<TEntity>(this EntityTypeBuilder<TEntity> builder, string tableName)
        where TEntity : class, ITenantOwned
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(tableName);

        builder.Property(entity => entity.CentreId).IsRequired();

        builder
            .HasOne<Centre>()
            .WithMany()
            .HasForeignKey(entity => entity.CentreId)
            .HasConstraintName($"fk_{tableName}_centres")
            .OnDelete(DeleteBehavior.Restrict);

        // Alternate key, not just a unique index: only a key (primary or alternate) can be the target of another
        // entity's HasPrincipalKey, which the composite child foreign key (Task 21.6) needs.
        builder
            .HasAlternateKey(nameof(ITenantOwned.CentreId), "Id")
            .HasName($"ux_{tableName}_centre_id_id");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
