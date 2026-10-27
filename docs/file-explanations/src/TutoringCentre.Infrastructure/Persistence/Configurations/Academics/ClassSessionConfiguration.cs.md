# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps ClassSession to academics.class_sessions. The start and end are UTC timestamptz instants; the local date is a date in the centre's zone, kept beside them so the timetable selects by calendar day. The group, teacher and rule references are composite foreign keys through (centre_id, id) with no navigation and no cascade: a session is never deleted, only cancelled. The unique (centre_id, schedule_rule_id, local_date) makes generation repeatable. The two exclusion constraints that forbid double-booking cannot be expressed in EF's model; they are written as SQL in the AddSchedule migration (ADR 0012).

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 121 lines and 5,704 bytes. Source hash: `0ff0e17fce7224216c935aba83ce15eb5bebac64990dc1e108e53d30acef93dd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L17): `internal sealed class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>`
- [Line 22](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L22): `public void Configure(EntityTypeBuilder<ClassSession> builder)`
- [Line 108](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L108): `private static string StatusToDatabase(ClassSessionStatus status) => status switch`
- [Line 112](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L112): `_ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported class session status."),`
- [Line 115](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L115): `private static ClassSessionStatus StatusFromDatabase(string value) => value switch`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 31 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
