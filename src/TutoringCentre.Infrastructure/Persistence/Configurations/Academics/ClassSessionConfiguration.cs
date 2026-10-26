using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// Maps ClassSession to academics.class_sessions. The start and end are UTC <c>timestamptz</c> instants; the
/// local date is a <c>date</c> in the centre's zone, kept beside them so the timetable selects by calendar day.
/// The group, teacher and rule references are composite foreign keys through (centre_id, id) with no navigation
/// and no cascade: a session is never deleted, only cancelled. The unique (centre_id, schedule_rule_id,
/// local_date) makes generation repeatable. The two exclusion constraints that forbid double-booking cannot be
/// expressed in EF's model; they are written as SQL in the AddSchedule migration (ADR 0012).
/// </summary>
internal sealed class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    private const int StatusMaxLength = 10;
    private const string TableName = "class_sessions";

    public void Configure(EntityTypeBuilder<ClassSession> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table =>
            {
                table.HasCheckConstraint("ck_class_sessions_status", "status IN ('scheduled','cancelled')");
                table.HasCheckConstraint("ck_class_sessions_range", "ends_at > starts_at");
            });

        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(session => session.GroupId).IsRequired();
        builder.Property(session => session.TeacherId).IsRequired();
        builder.Property(session => session.ScheduleRuleId).IsRequired();
        builder.Property(session => session.LocalDate).HasColumnType("date").IsRequired();
        builder.Property(session => session.StartsAt).IsRequired();
        builder.Property(session => session.EndsAt).IsRequired();
        builder.Property(session => session.Room).HasMaxLength(ScheduleRule.RoomMaxLength);

        builder
            .Property(session => session.Status)
            .HasConversion(new ValueConverter<ClassSessionStatus, string>(
                status => StatusToDatabase(status),
                value => StatusFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(StatusMaxLength);

        builder
            .HasOne<Group>()
            .WithMany()
            .HasForeignKey(session => new { session.CentreId, session.GroupId })
            .HasPrincipalKey(group => new { group.CentreId, group.Id })
            .HasConstraintName($"fk_{TableName}_groups")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<Teacher>()
            .WithMany()
            .HasForeignKey(session => new { session.CentreId, session.TeacherId })
            .HasPrincipalKey(teacher => new { teacher.CentreId, teacher.Id })
            .HasConstraintName($"fk_{TableName}_teachers")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<ScheduleRule>()
            .WithMany()
            .HasForeignKey(session => new { session.CentreId, session.ScheduleRuleId })
            .HasPrincipalKey(rule => new { rule.CentreId, rule.Id })
            .HasConstraintName($"fk_{TableName}_rules")
            .OnDelete(DeleteBehavior.Restrict);

        // The timetable's index: one centre, a range of local dates, ordered by start instant.
        builder
            .HasIndex(session => new { session.CentreId, session.LocalDate, session.StartsAt })
            .HasDatabaseName($"ix_{TableName}_centre_date");

        builder
            .HasIndex(session => new { session.CentreId, session.GroupId, session.LocalDate })
            .HasDatabaseName($"ix_{TableName}_centre_group_date");

        // Not in the table design, but EF adds an index for every foreign key no other index covers, and the
        // teacher key is the only uncovered one; named here so it follows the project's naming. It also serves
        // the restrict check on academics.teachers.
        builder
            .HasIndex(session => new { session.CentreId, session.TeacherId })
            .HasDatabaseName($"ix_{TableName}_centre_teacher");

        builder
            .HasIndex(session => new { session.CentreId, session.ScheduleRuleId, session.LocalDate })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_rule_date");

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string StatusToDatabase(ClassSessionStatus status) => status switch
    {
        ClassSessionStatus.Scheduled => "scheduled",
        ClassSessionStatus.Cancelled => "cancelled",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported class session status."),
    };

    private static ClassSessionStatus StatusFromDatabase(string value) => value switch
    {
        "scheduled" => ClassSessionStatus.Scheduled,
        "cancelled" => ClassSessionStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported status value in the database."),
    };
}
