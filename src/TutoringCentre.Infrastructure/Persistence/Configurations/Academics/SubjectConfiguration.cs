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
