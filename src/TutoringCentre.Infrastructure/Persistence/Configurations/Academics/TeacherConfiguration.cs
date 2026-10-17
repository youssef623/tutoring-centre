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
