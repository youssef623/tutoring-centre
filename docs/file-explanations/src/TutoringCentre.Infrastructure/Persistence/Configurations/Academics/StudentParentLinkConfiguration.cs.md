# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps StudentParentLink, the Student aggregate's child entity, to academics.student_parents. The relationship to Student is declared from `StudentConfiguration` (the aggregate root's own backing field); only the relationship to Parent is declared here, with no navigation back — a link knows a ParentId, never a Parent object, so no handler can reach across aggregates through it.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 91 lines and 4,089 bytes. Source hash: `ebd7ee5ab414521236b1a800ea245c8199b6403ccb86cb8bd99872754f5f65e4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs#L14): `internal sealed class StudentParentLinkConfiguration : IEntityTypeConfiguration<StudentParentLink>`
- [Line 19](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs#L19): `public void Configure(EntityTypeBuilder<StudentParentLink> builder)`
- [Line 74](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs#L74): `private static string RelationshipToDatabase(ParentRelationship value) => value switch`
- [Line 83](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs#L83): `private static ParentRelationship RelationshipFromDatabase(string value) => value switch`

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

/// <summary>
/// Maps StudentParentLink, the Student aggregate's child entity, to academics.student_parents. The
/// relationship to Student is declared from <see cref="StudentConfiguration"/> (the aggregate root's own
/// backing field); only the relationship to Parent is declared here, with no navigation back — a link knows
/// a ParentId, never a Parent object, so no handler can reach across aggregates through it.
/// </summary>
internal sealed class StudentParentLinkConfiguration : IEntityTypeConfiguration<StudentParentLink>
{
    private const int RelationshipMaxLength = 10;
    private const string TableName = "student_parents";

    public void Configure(EntityTypeBuilder<StudentParentLink> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table => table.HasCheckConstraint("ck_student_parents_relationship", "relationship IN ('father','mother','guardian','other')"));

        builder.HasKey(link => link.Id);
        builder.Property(link => link.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(link => link.StudentId).IsRequired();
        builder.Property(link => link.ParentId).IsRequired();
        builder.Property(link => link.IsPrimary).IsRequired();

        builder
            .Property(link => link.Relationship)
            .HasConversion(new ValueConverter<ParentRelationship, string>(
                relationship => RelationshipToDatabase(relationship),
                value => RelationshipFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(RelationshipMaxLength);

        builder
            .HasOne<Parent>()
            .WithMany()
            .HasForeignKey(link => new { link.CentreId, link.ParentId })
            .HasPrincipalKey(parent => new { parent.CentreId, parent.Id })
            .HasConstraintName("fk_student_parents_parents")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(link => new { link.CentreId, link.ParentId })
            .HasDatabaseName($"ix_{TableName}_centre_parent");

        builder
            .HasIndex(link => new { link.CentreId, link.StudentId, link.ParentId })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_student_parent");

        // Partial unique index: only rows where is_primary is true, so at most one primary link per student.
        builder
            .HasIndex(link => new { link.CentreId, link.StudentId })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_one_primary")
            .HasFilter("is_primary");

        // Shadow CreatedAt only, set by TimestampInterceptor: no UpdatedAt and no row version — the aggregate
        // is versioned at its root (Student.LinksRevision + the students row's own xmin).
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
    }

    private static string RelationshipToDatabase(ParentRelationship value) => value switch
    {
        ParentRelationship.Father => "father",
        ParentRelationship.Mother => "mother",
        ParentRelationship.Guardian => "guardian",
        ParentRelationship.Other => "other",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported parent relationship."),
    };

    private static ParentRelationship RelationshipFromDatabase(string value) => value switch
    {
        "father" => ParentRelationship.Father,
        "mother" => ParentRelationship.Mother,
        "guardian" => ParentRelationship.Guardian,
        "other" => ParentRelationship.Other,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported relationship value in the database."),
    };
```

The excerpt stops at line 90; use the original file for the remaining 1 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
