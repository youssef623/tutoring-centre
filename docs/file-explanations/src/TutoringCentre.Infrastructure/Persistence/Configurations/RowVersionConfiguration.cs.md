# `src/TutoringCentre.Infrastructure/Persistence/Configurations/RowVersionConfiguration.cs`

## Purpose

The mapping exposes PostgreSQL xmin through the shadow uint property Version, generated on add or update and used as an EF concurrency token. Domain entities need no PostgreSQL-specific field. Versioned adapters must set the client's value as OriginalValue so EF rejects stale updates at save rather than merely trusting a loaded row.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/RowVersionConfiguration.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,041 bytes. Source hash: `ffc9a550b80d2dc104e98220215c2963827cae1169055aa010f725726e00ad4d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/RowVersionConfiguration.cs#L12): `internal static class RowVersionConfiguration`
- [Line 14](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/RowVersionConfiguration.cs#L14): `public static void HasRowVersion<TEntity>(this EntityTypeBuilder<TEntity> builder)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TutoringCentre.Infrastructure.Persistence.Configurations;

/// <summary>
/// What an entity's own IEntityTypeConfiguration calls (the manual's HasRowVersion, Task 23.2) to opt into
/// optimistic concurrency on PostgreSQL's built-in xmin system column: no column is ever created by a migration
/// for it, since xmin already exists on every table. Mapped under the shadow name "Version" rather than "xmin" so
/// callers reading the model never need to know the underlying system column's name.
/// </summary>
internal static class RowVersionConfiguration
{
    public static void HasRowVersion<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .Property<uint>("Version")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsRowVersion();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
