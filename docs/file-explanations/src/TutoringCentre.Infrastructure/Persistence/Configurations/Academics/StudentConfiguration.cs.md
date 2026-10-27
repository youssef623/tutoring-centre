# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps the Student aggregate root to academics.students. The parent links (`StudentParentLink`) are persisted through the private backing field behind `Student.Parents`, read and written by EF directly — the aggregate's public surface stays read-only, exactly as Day 37 left it.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 88 lines and 4,106 bytes. Source hash: `d185c48111fc86a1f2e93a6b198dbe07f61c7b00899d63e857dbc45b6efd1d1e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L13): `internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>`
- [Line 19](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L19): `public void Configure(EntityTypeBuilder<Student> builder)`
- [Line 75](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L75): `private static string StatusToDatabase(StudentStatus status) => status switch`
- [Line 82](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L82): `private static StudentStatus StatusFromDatabase(string value) => value switch`

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
/// Maps the Student aggregate root to academics.students. The parent links (<see cref="StudentParentLink"/>)
/// are persisted through the private backing field behind <see cref="Student.Parents"/>, read and written by
/// EF directly — the aggregate's public surface stays read-only, exactly as Day 37 left it.
/// </summary>
internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    private const int StatusMaxLength = 10;
    private const int PhoneMaxLength = 16;
    private const string TableName = "students";

    public void Configure(EntityTypeBuilder<Student> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table =>
            {
                table.HasCheckConstraint("ck_students_status", "status IN ('active','archived')");
                table.HasCheckConstraint("ck_students_phone_format", PhoneFormatCheck.SqlOrNull);
            });

        builder.HasKey(student => student.Id);
        builder.Property(student => student.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(student => student.FullName).IsRequired().HasMaxLength(Student.NameMaxLength);
        builder.Property(student => student.SearchName).IsRequired().HasMaxLength(Student.NameMaxLength);
        builder.Property(student => student.SchoolYear).HasMaxLength(Student.SchoolYearMaxLength);
        builder.Property(student => student.LinksRevision).IsRequired();

        builder.Property(student => student.Phone).HasConversion(PhoneNumberConverters.Optional).HasMaxLength(PhoneMaxLength);

        builder
            .Property(student => student.Status)
            .HasConversion(new ValueConverter<StudentStatus, string>(
                status => StatusToDatabase(status),
                value => StatusFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(StatusMaxLength);

        builder
            .HasIndex(student => new { student.CentreId, student.SearchName, student.Id })
            .HasDatabaseName($"ix_{TableName}_centre_search_name_id");

        // The child link table: EF reads and writes Student.Parents through its backing field, since the
        // property itself exposes only a read-only view (IReadOnlyCollection<StudentParentLink>).
        builder
            .HasMany(student => student.Parents)
            .WithOne() // StudentParentLink carries StudentId as a scalar only — no navigation back to Student.
            .HasForeignKey(link => new { link.CentreId, link.StudentId })
            .HasPrincipalKey(student => new { student.CentreId, student.Id })
            .HasConstraintName("fk_student_parents_students")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(student => student.Parents).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string StatusToDatabase(StudentStatus status) => status switch
    {
        StudentStatus.Active => "active",
        StudentStatus.Archived => "archived",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported student status."),
    };

    private static StudentStatus StatusFromDatabase(string value) => value switch
    {
        "active" => StudentStatus.Active,
        "archived" => StudentStatus.Archived,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported status value in the database."),
    };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
