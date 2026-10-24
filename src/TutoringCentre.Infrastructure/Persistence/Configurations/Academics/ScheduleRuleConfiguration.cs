using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// Maps ScheduleRule to academics.schedule_rules. Every value is local: the weekday as a smallint (0 = Sunday,
/// matching <see cref="DayOfWeek"/>), the start as a <c>time</c> and the effective range as <c>date</c>s — never a
/// timestamp, because a rule says "18:00 in the centre's zone", not an instant. The group reference is a composite
/// foreign key through (centre_id, id), so a rule can never point at another centre's group; no navigation.
/// </summary>
internal sealed class ScheduleRuleConfiguration : IEntityTypeConfiguration<ScheduleRule>
{
    private const string TableName = "schedule_rules";

    public void Configure(EntityTypeBuilder<ScheduleRule> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table =>
            {
                table.HasCheckConstraint("ck_schedule_rules_day", "day_of_week BETWEEN 0 AND 6");
                table.HasCheckConstraint(
                    "ck_schedule_rules_duration",
                    $"duration_minutes BETWEEN {ScheduleRule.MinDurationMinutes} AND {ScheduleRule.MaxDurationMinutes}");
                table.HasCheckConstraint("ck_schedule_rules_dates", "effective_to IS NULL OR effective_to >= effective_from");
            });

        builder.HasKey(rule => rule.Id);
        builder.Property(rule => rule.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(rule => rule.GroupId).IsRequired();
        builder.Property(rule => rule.DayOfWeek).HasConversion<short>().HasColumnType("smallint").IsRequired();
        builder.Property(rule => rule.StartTime).HasColumnType("time").IsRequired();
        builder.Property(rule => rule.DurationMinutes).IsRequired();
        builder.Property(rule => rule.Room).HasMaxLength(ScheduleRule.RoomMaxLength);
        builder.Property(rule => rule.EffectiveFrom).HasColumnType("date").IsRequired();
        builder.Property(rule => rule.EffectiveTo).HasColumnType("date");

        builder
            .HasOne<Group>()
            .WithMany()
            .HasForeignKey(rule => new { rule.CentreId, rule.GroupId })
            .HasPrincipalKey(group => new { group.CentreId, group.Id })
            .HasConstraintName($"fk_{TableName}_groups")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(rule => new { rule.CentreId, rule.GroupId })
            .HasDatabaseName($"ix_{TableName}_centre_group");

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }
}
