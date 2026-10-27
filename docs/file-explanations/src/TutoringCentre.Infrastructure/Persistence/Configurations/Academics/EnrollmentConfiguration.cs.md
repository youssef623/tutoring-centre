# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps Enrollment to academics.enrollments. The student and group references are composite foreign keys through (centre_id, id), so an enrollment can never join a student and a group of different centres, even through privileged SQL; no navigation property to either. The partial unique index is the race-proof form of "not already enrolled": at most one active row per student and group, while ended rows stay as history so a student can re-enroll. The checks make an impossible state (ended without a time or a reason, or ending before it started) unstorable even by a bug. Capacity is deliberately not here — "at most N active rows for this group" is not expressible as a unique or check constraint; it is enforced under a row lock on the group (ADR 0011).

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 130 lines and 6,243 bytes. Source hash: `7c89db853f234d3a4b15345e49ca1721b033b52416e69ee14d110a65fea8c8b4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L17): `internal sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>`
- [Line 23](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L23): `public void Configure(EntityTypeBuilder<Enrollment> builder)`
- [Line 103](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L103): `private static string StatusToDatabase(EnrollmentStatus status) => status switch`
- [Line 110](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L110): `private static EnrollmentStatus StatusFromDatabase(string value) => value switch`
- [Line 117](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L117): `private static string EndReasonToDatabase(EnrollmentEndReason reason) => reason switch`
- [Line 124](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L124): `private static EnrollmentEndReason EndReasonFromDatabase(string value) => value switch`

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
```

The excerpt stops at line 90; use the original file for the remaining 40 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
