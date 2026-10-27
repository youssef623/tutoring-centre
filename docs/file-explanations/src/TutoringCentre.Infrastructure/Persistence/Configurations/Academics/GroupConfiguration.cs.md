# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps Group to academics.groups. Both the subject and teacher references are composite foreign keys through (centre_id, id) — the same pattern as StudentParentLink's link to Parent — so a group can never store a reference into another centre's subject or teacher, even through privileged SQL. No navigation property to either: a handler reaches Subject or Teacher only through their own repositories.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 100 lines and 4,241 bytes. Source hash: `99e15834fc192a34755034ab4689312783910e6760250f70725e2e24173ccea5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs#L14): `internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>`
- [Line 19](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs#L19): `public void Configure(EntityTypeBuilder<Group> builder)`
- [Line 87](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs#L87): `private static string StatusToDatabase(GroupStatus status) => status switch`
- [Line 94](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs#L94): `private static GroupStatus StatusFromDatabase(string value) => value switch`

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
/// Maps Group to academics.groups. Both the subject and teacher references are composite foreign keys through
/// (centre_id, id) — the same pattern as StudentParentLink's link to Parent — so a group can never store a
/// reference into another centre's subject or teacher, even through privileged SQL. No navigation property to
/// either: a handler reaches Subject or Teacher only through their own repositories.
/// </summary>
internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    private const int StatusMaxLength = 10;
    private const string TableName = "groups";

    public void Configure(EntityTypeBuilder<Group> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table =>
            {
                table.HasCheckConstraint("ck_groups_status", "status IN ('active','archived')");
                table.HasCheckConstraint("ck_groups_capacity", "capacity BETWEEN 1 AND 200");
            });

        builder.HasKey(group => group.Id);
        builder.Property(group => group.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(group => group.Name).IsRequired().HasMaxLength(Group.NameMaxLength);
        builder.Property(group => group.NormalizedName).IsRequired().HasMaxLength(Group.NameMaxLength);
        builder.Property(group => group.SubjectId).IsRequired();
        builder.Property(group => group.TeacherId).IsRequired();
        builder.Property(group => group.Capacity).IsRequired();

        builder
            .Property(group => group.Status)
            .HasConversion(new ValueConverter<GroupStatus, string>(
                status => StatusToDatabase(status),
                value => StatusFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(StatusMaxLength);

        builder
            .HasOne<Subject>()
            .WithMany()
            .HasForeignKey(group => new { group.CentreId, group.SubjectId })
            .HasPrincipalKey(subject => new { subject.CentreId, subject.Id })
            .HasConstraintName($"fk_{TableName}_subjects")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<Teacher>()
            .WithMany()
            .HasForeignKey(group => new { group.CentreId, group.TeacherId })
            .HasPrincipalKey(teacher => new { teacher.CentreId, teacher.Id })
            .HasConstraintName($"fk_{TableName}_teachers")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(group => new { group.CentreId, group.TeacherId })
            .HasDatabaseName($"ix_{TableName}_centre_teacher");

        builder
            .HasIndex(group => new { group.CentreId, group.SubjectId })
            .HasDatabaseName($"ix_{TableName}_centre_subject");

        builder
            .HasIndex(group => new { group.CentreId, group.NormalizedName })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_centre_normalized_name");

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string StatusToDatabase(GroupStatus status) => status switch
    {
        GroupStatus.Active => "active",
        GroupStatus.Archived => "archived",
```

The excerpt stops at line 90; use the original file for the remaining 10 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
