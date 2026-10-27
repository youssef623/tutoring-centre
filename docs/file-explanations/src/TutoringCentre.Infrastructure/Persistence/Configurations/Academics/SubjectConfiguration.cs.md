# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps the Subject entity to academics.subjects. Lengths mirror the Domain's limits exactly.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 64 lines and 2,655 bytes. Source hash: `17b99d678cb0f139c4b54d9d7aedbf34fc2dfc2d2e28537ea49897258f12ea98`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs#L9): `internal sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>`
- [Line 14](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs#L14): `public void Configure(EntityTypeBuilder<Subject> builder)`
- [Line 51](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs#L51): `private static string StatusToDatabase(SubjectStatus status) => status switch`
- [Line 58](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs#L58): `private static SubjectStatus StatusFromDatabase(string value) => value switch`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>Maps the Subject entity to academics.subjects. Lengths mirror the Domain's limits exactly.</summary>
internal sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    private const int StatusMaxLength = 10;
    private const string TableName = "subjects";

    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table => table.HasCheckConstraint("ck_subjects_status", "status IN ('active','archived')"));

        builder.HasKey(subject => subject.Id);
        builder.Property(subject => subject.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(subject => subject.Name).IsRequired().HasMaxLength(Subject.NameMaxLength);
        builder.Property(subject => subject.NormalizedName).IsRequired().HasMaxLength(Subject.NameMaxLength);

        builder
            .HasIndex(subject => new { subject.CentreId, subject.NormalizedName })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_centre_normalized_name");

        builder
            .Property(subject => subject.Status)
            .HasConversion(new ValueConverter<SubjectStatus, string>(
                status => StatusToDatabase(status),
                value => StatusFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(StatusMaxLength);

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string StatusToDatabase(SubjectStatus status) => status switch
    {
        SubjectStatus.Active => "active",
        SubjectStatus.Archived => "archived",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported subject status."),
    };

    private static SubjectStatus StatusFromDatabase(string value) => value switch
    {
        "active" => SubjectStatus.Active,
        "archived" => SubjectStatus.Archived,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported status value in the database."),
    };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
