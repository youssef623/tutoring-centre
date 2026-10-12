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
