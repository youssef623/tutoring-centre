# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps the Teacher entity to academics.teachers. The optional account link (`Teacher.UserId`) has no EF relationship here: it has no navigation and its target, identity.memberships, has no EF-declared key on (centre_id, user_id) for HasPrincipalKey to reference — only a plain unique index, which Postgres accepts as a foreign key target on its own. The composite foreign key is therefore added as a raw operation directly in the AddTeachers migration (Task 43.2), never through this configuration, so the EF model never describes it and the model snapshot has nothing to drift against identity's own model.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 85 lines and 3,900 bytes. Source hash: `e2a03f87d5f8d8fe178859991f14f1cd7caab60b3968ebae3e87044b7ca7c2ca`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs#L16): `internal sealed class TeacherConfiguration : IEntityTypeConfiguration<Teacher>`
- [Line 22](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs#L22): `public void Configure(EntityTypeBuilder<Teacher> builder)`
- [Line 72](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs#L72): `private static string StatusToDatabase(TeacherStatus status) => status switch`
- [Line 79](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs#L79): `private static TeacherStatus StatusFromDatabase(string value) => value switch`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/read-generated-models.py:36](../../../../../../onboarding-work/read-generated-models.py#L36)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// Maps the Teacher entity to academics.teachers. The optional account link (<see cref="Teacher.UserId"/>) has
/// no EF relationship here: it has no navigation and its target, identity.memberships, has no EF-declared key
/// on (centre_id, user_id) for <c>HasPrincipalKey</c> to reference — only a plain unique index, which
/// Postgres accepts as a foreign key target on its own. The composite foreign key is therefore added as a raw
/// operation directly in the AddTeachers migration (Task 43.2), never through this configuration, so the EF
/// model never describes it and the model snapshot has nothing to drift against identity's own model.
/// </summary>
internal sealed class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    private const int StatusMaxLength = 10;
    private const int PhoneMaxLength = 16;
    private const string TableName = "teachers";

    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table =>
            {
                table.HasCheckConstraint("ck_teachers_status", "status IN ('active','archived')");
                table.HasCheckConstraint("ck_teachers_phone_format", PhoneFormatCheck.SqlOrNull);
            });

        builder.HasKey(teacher => teacher.Id);
        builder.Property(teacher => teacher.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(teacher => teacher.FullName).IsRequired().HasMaxLength(Teacher.NameMaxLength);
        builder.Property(teacher => teacher.SearchName).IsRequired().HasMaxLength(Teacher.NameMaxLength);
        builder.Property(teacher => teacher.Phone).HasConversion(PhoneNumberConverters.Optional).HasMaxLength(PhoneMaxLength);

        // No HasOne/HasForeignKey: see the type's own remarks. The database-level FK is added by hand in the migration.
        builder.Property(teacher => teacher.UserId);

        builder
            .Property(teacher => teacher.Status)
            .HasConversion(new ValueConverter<TeacherStatus, string>(
                status => StatusToDatabase(status),
                value => StatusFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(StatusMaxLength);

        builder
            .HasIndex(teacher => new { teacher.CentreId, teacher.UserId })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_centre_user")
            .HasFilter("user_id IS NOT NULL");

        builder
            .HasIndex(teacher => new { teacher.CentreId, teacher.SearchName })
            .HasDatabaseName($"ix_{TableName}_centre_search_name");

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string StatusToDatabase(TeacherStatus status) => status switch
    {
        TeacherStatus.Active => "active",
        TeacherStatus.Archived => "archived",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported teacher status."),
    };

    private static TeacherStatus StatusFromDatabase(string value) => value switch
    {
        "active" => TeacherStatus.Active,
        "archived" => TeacherStatus.Archived,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported status value in the database."),
    };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
