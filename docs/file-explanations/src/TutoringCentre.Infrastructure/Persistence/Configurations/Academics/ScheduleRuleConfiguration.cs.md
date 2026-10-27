# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps ScheduleRule to academics.schedule_rules. Every value is local: the weekday as a smallint (0 = Sunday, matching `DayOfWeek`), the start as a time and the effective range as dates — never a timestamp, because a rule says "18:00 in the centre's zone", not an instant. The group reference is a composite foreign key through (centre_id, id), so a rule can never point at another centre's group; no navigation.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 65 lines and 3,094 bytes. Source hash: `bf5377e3ccb6a4a19f9f5ab9d1b16521e4a039969eec1d69a6ee3c3e1bc93e22`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs#L14): `internal sealed class ScheduleRuleConfiguration : IEntityTypeConfiguration<ScheduleRule>`
- [Line 18](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs#L18): `public void Configure(EntityTypeBuilder<ScheduleRule> builder)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
