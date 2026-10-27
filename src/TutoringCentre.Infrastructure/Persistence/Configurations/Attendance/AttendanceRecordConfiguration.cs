using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Attendance;

internal sealed class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    private const string TableName = "attendance_records";

    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable(TableName, Schemas.Attendance, table =>
        {
            table.HasCheckConstraint("ck_attendance_records_status", "status IN ('present','late','absent','excused')");
            table.HasCheckConstraint("ck_attendance_records_source", "source IN ('manual','qr')");
            table.HasCheckConstraint("ck_attendance_records_corrections",
                "correction_count >= 0 AND (correction_count = 0) = (last_corrected_at IS NULL)");
        });
        builder.HasKey(record => record.Id);
        builder.Property(record => record.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned(TableName);
        builder.Property(record => record.SessionId).IsRequired();
        builder.Property(record => record.StudentId).IsRequired();
        builder.Property(record => record.EnrollmentId).IsRequired();
        builder.Property(record => record.Status).HasConversion(new ValueConverter<AttendanceStatus, string>(
            status => StatusToDatabase(status), value => StatusFromDatabase(value))).HasMaxLength(10).IsRequired();
        builder.Property(record => record.Source).HasConversion(new ValueConverter<AttendanceSource, string>(
            source => SourceToDatabase(source), value => SourceFromDatabase(value))).HasMaxLength(10).IsRequired();
        builder.Property(record => record.MarkedAt).IsRequired();
        builder.Property(record => record.MarkedByUserId);
        builder.Property(record => record.CorrectionCount).HasDefaultValue(0).IsRequired();
        // A field-only EF property gives the existing audit pipeline its contracted correctionReason name
        // while the Domain keeps LastCorrectionReason. No interceptor or diff-builder special case is needed.
        builder.Ignore(record => record.LastCorrectionReason);
        builder.Property<string?>("correctionReason").HasField("correctionReason")
            .HasColumnName("last_correction_reason").HasMaxLength(200);
        builder.Property(record => record.LastCorrectedAt);
        builder.Property(record => record.LastCorrectedByUserId);

        builder.HasOne<ClassSession>().WithMany()
            .HasForeignKey(record => new { record.CentreId, record.SessionId })
            .HasPrincipalKey(session => new { session.CentreId, session.Id })
            .HasConstraintName("fk_attendance_records_sessions").OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany()
            .HasForeignKey(record => new { record.CentreId, record.StudentId })
            .HasPrincipalKey(student => new { student.CentreId, student.Id })
            .HasConstraintName("fk_attendance_records_students").OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Enrollment>().WithMany()
            .HasForeignKey(record => new { record.CentreId, record.EnrollmentId, record.StudentId })
            .HasPrincipalKey(enrollment => new { enrollment.CentreId, enrollment.Id, enrollment.StudentId })
            .HasConstraintName("fk_attendance_records_enrollments").OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(record => record.MarkedByUserId)
            .HasConstraintName("fk_attendance_records_marked_by").OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(record => record.LastCorrectedByUserId)
            .HasConstraintName("fk_attendance_records_corrected_by").OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(record => new { record.CentreId, record.SessionId, record.StudentId })
            .IsUnique().HasDatabaseName("ux_attendance_records_session_student");
        builder.HasIndex(record => new { record.CentreId, record.StudentId })
            .HasDatabaseName("ix_attendance_records_centre_student");
        builder.HasRowVersion();
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string StatusToDatabase(AttendanceStatus status) => status switch
    {
        AttendanceStatus.Present => "present",
        AttendanceStatus.Late => "late",
        AttendanceStatus.Absent => "absent",
        AttendanceStatus.Excused => "excused",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported attendance status."),
    };

    private static AttendanceStatus StatusFromDatabase(string value) => value switch
    {
        "present" => AttendanceStatus.Present,
        "late" => AttendanceStatus.Late,
        "absent" => AttendanceStatus.Absent,
        "excused" => AttendanceStatus.Excused,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported attendance status in the database."),
    };

    private static string SourceToDatabase(AttendanceSource source) => source switch
    {
        AttendanceSource.Manual => "manual",
        AttendanceSource.Qr => "qr",
        _ => throw new ArgumentOutOfRangeException(nameof(source), source, "Unsupported attendance source."),
    };

    private static AttendanceSource SourceFromDatabase(string value) => value switch
    {
        "manual" => AttendanceSource.Manual,
        "qr" => AttendanceSource.Qr,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported attendance source in the database."),
    };
}
