# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps the Parent entity to academics.parents. Lengths mirror the Domain's limits exactly.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 51 lines and 1,994 bytes. Source hash: `3b30c78b40f8d57cb05194d2a4af632a886cb07f6e3911ac9783e2baa31ecc1c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs#L8): `internal sealed class ParentConfiguration : IEntityTypeConfiguration<Parent>`
- [Line 13](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs#L13): `public void Configure(EntityTypeBuilder<Parent> builder)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>Maps the Parent entity to academics.parents. Lengths mirror the Domain's limits exactly.</summary>
internal sealed class ParentConfiguration : IEntityTypeConfiguration<Parent>
{
    private const int PhoneMaxLength = 16;
    private const string TableName = "parents";

    public void Configure(EntityTypeBuilder<Parent> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table => table.HasCheckConstraint("ck_parents_phone_format", PhoneFormatCheck.Sql));

        builder.HasKey(parent => parent.Id);
        builder.Property(parent => parent.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(parent => parent.FullName).IsRequired().HasMaxLength(Parent.NameMaxLength);
        builder.Property(parent => parent.SearchName).IsRequired().HasMaxLength(Parent.NameMaxLength);

        builder
            .Property(parent => parent.Phone)
            .HasConversion(PhoneNumberConverters.Required)
            .IsRequired()
            .HasMaxLength(PhoneMaxLength);

        builder
            .HasIndex(parent => new { parent.CentreId, parent.SearchName })
            .HasDatabaseName($"ix_{TableName}_centre_search_name");

        builder
            .HasIndex(parent => new { parent.CentreId, parent.Phone })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_centre_phone");

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
