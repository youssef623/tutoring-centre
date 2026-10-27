using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// Maps Enrollment to academics.enrollments. The student and group references are composite foreign keys through
/// (centre_id, id), so an enrollment can never join a student and a group of different centres, even through
/// privileged SQL; no navigation property to either. The partial unique index is the race-proof form of "not
/// already enrolled": at most one active row per student and group, while ended rows stay as history so a student
/// can re-enroll. The checks make an impossible state (ended without a time or a reason, or ending before it
/// started) unstorable even by a bug. Capacity is deliberately not here — "at most N active rows for this group"
/// is not expressible as a unique or check constraint; it is enforced under a row lock on the group (ADR 0011).
/// </summary>
internal sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    private const int StatusMaxLength = 10;
    private const int EndReasonMaxLength = 12;
    private const string TableName = "enrollments";

    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table =>
            {
                table.HasCheckConstraint("ck_enrollments_status", "status IN ('active','ended')");
                table.HasCheckConstraint("ck_enrollments_end_reason", "end_reason IS NULL OR end_reason IN ('withdrawn','transferred')");
                table.HasCheckConstraint(
                    "ck_enrollments_end_consistency",
                    "(status = 'ended') = (ended_at IS NOT NULL) AND (status = 'ended') = (end_reason IS NOT NULL)");
                table.HasCheckConstraint("ck_enrollments_end_after_start", "ended_at IS NULL OR ended_at >= enrolled_at");
            });

        builder.HasKey(enrollment => enrollment.Id);
        builder.Property(enrollment => enrollment.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);
        builder.HasAlternateKey(enrollment => new { enrollment.CentreId, enrollment.Id, enrollment.StudentId })
            .HasName("ux_enrollments_centre_id_student");

        builder.Property(enrollment => enrollment.StudentId).IsRequired();
        builder.Property(enrollment => enrollment.GroupId).IsRequired();
        builder.Property(enrollment => enrollment.EnrolledAt).IsRequired();
        builder.Property(enrollment => enrollment.EndedAt);

        builder
            .Property(enrollment => enrollment.Status)
            .HasConversion(new ValueConverter<EnrollmentStatus, string>(
                status => StatusToDatabase(status),
                value => StatusFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(StatusMaxLength);

        builder
            .Property(enrollment => enrollment.EndReason)
            .HasConversion(new ValueConverter<EnrollmentEndReason, string>(
                reason => EndReasonToDatabase(reason),
                value => EndReasonFromDatabase(value)))
            .HasMaxLength(EndReasonMaxLength);

        builder
            .HasOne<Student>()
            .WithMany()
            .HasForeignKey(enrollment => new { enrollment.CentreId, enrollment.StudentId })
            .HasPrincipalKey(student => new { student.CentreId, student.Id })
            .HasConstraintName($"fk_{TableName}_students")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<Group>()
            .WithMany()
            .HasForeignKey(enrollment => new { enrollment.CentreId, enrollment.GroupId })
            .HasPrincipalKey(group => new { group.CentreId, group.Id })
            .HasConstraintName($"fk_{TableName}_groups")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(enrollment => new { enrollment.CentreId, enrollment.GroupId, enrollment.Status })
            .HasDatabaseName($"ix_{TableName}_centre_group_status");

        builder
            .HasIndex(enrollment => new { enrollment.CentreId, enrollment.StudentId })
            .HasDatabaseName($"ix_{TableName}_centre_student");

        // Partial: only active rows compete, so ended enrollments stay as history and re-enrolling is allowed.
        builder
            .HasIndex(enrollment => new { enrollment.CentreId, enrollment.StudentId, enrollment.GroupId })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_active_student_group")
            .HasFilter("status = 'active'");

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string StatusToDatabase(EnrollmentStatus status) => status switch
    {
        EnrollmentStatus.Active => "active",
        EnrollmentStatus.Ended => "ended",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported enrollment status."),
    };

    private static EnrollmentStatus StatusFromDatabase(string value) => value switch
    {
        "active" => EnrollmentStatus.Active,
        "ended" => EnrollmentStatus.Ended,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported status value in the database."),
    };

    private static string EndReasonToDatabase(EnrollmentEndReason reason) => reason switch
    {
        EnrollmentEndReason.Withdrawn => "withdrawn",
        EnrollmentEndReason.Transferred => "transferred",
        _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unsupported enrollment end reason."),
    };

    private static EnrollmentEndReason EndReasonFromDatabase(string value) => value switch
    {
        "withdrawn" => EnrollmentEndReason.Withdrawn,
        "transferred" => EnrollmentEndReason.Transferred,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported end reason value in the database."),
    };
}
