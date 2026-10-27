# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps `AuditEntry` to audit.audit_entries, per the Day 31 contract. No DDL beyond the column shape lives here: row-level security and the append-only grant are in migration AddAuditLog (Task 31.3).

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 120 lines and 5,023 bytes. Source hash: `facb17c07616c814d187e5c4d2fddbea3cd387c33cc903cce8421cc128e2bb90`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Infrastructure.Audit;`
- Line 5: `using TutoringCentre.Infrastructure.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L11): `internal sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>`
- [Line 19](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L19): `public void Configure(EntityTypeBuilder<AuditEntry> builder)`
- [Line 90](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L90): `private static string ActorTypeToDatabase(AuditActorType value) => value switch`
- [Line 97](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L97): `private static AuditActorType ActorTypeFromDatabase(string value) => value switch`
- [Line 104](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L104): `private static string ActionToDatabase(AuditAction value) => value switch`
- [Line 112](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L112): `private static AuditAction ActionFromDatabase(string value) => value switch`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Infrastructure.Audit;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Audit;

/// <summary>Maps <see cref="AuditEntry"/> to audit.audit_entries, per the Day 31 contract. No DDL beyond the
/// column shape lives here: row-level security and the append-only grant are in migration AddAuditLog (Task 31.3).</summary>
internal sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    private const int ActorTypeMaxLength = 10;
    private const int ActionMaxLength = 10;
    private const int EntityTypeMaxLength = 60;
    private const int CorrelationIdMaxLength = 64;
    private const string TableName = "audit_entries";

    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Audit,
            table =>
            {
                table.HasCheckConstraint("ck_audit_entries_actor_type", "actor_type IN ('staff','system')");
                table.HasCheckConstraint("ck_audit_entries_action", "action IN ('created','updated','deleted')");
                table.HasCheckConstraint("ck_audit_entries_actor", "(actor_type = 'staff') = (actor_user_id IS NOT NULL)");
            });

        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the application

        builder.ConfigureTenantOwned(TableName);

        builder.Property(entry => entry.OccurredAt).IsRequired();

        builder
            .Property(entry => entry.ActorType)
            .HasConversion(new ValueConverter<AuditActorType, string>(
                value => ActorTypeToDatabase(value),
                value => ActorTypeFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(ActorTypeMaxLength);

        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(entry => entry.ActorUserId)
            .HasConstraintName($"fk_{TableName}_users")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(entry => entry.Action)
            .HasConversion(new ValueConverter<AuditAction, string>(
                value => ActionToDatabase(value),
                value => ActionFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(ActionMaxLength);

        builder
            .Property(entry => entry.EntityType)
            .HasConversion(new ValueConverter<AuditEntityType, string>(
                value => AuditEntityTypeNames.ToDatabase(value),
                value => AuditEntityTypeNames.FromDatabase(value)))
            .IsRequired()
            .HasMaxLength(EntityTypeMaxLength);

        builder.Property(entry => entry.EntityId).IsRequired();

        builder.Property(entry => entry.Changes).HasColumnType("jsonb").IsRequired();

        builder.Property(entry => entry.CorrelationId).HasMaxLength(CorrelationIdMaxLength);

        builder
            .HasIndex(entry => new { entry.CentreId, entry.OccurredAt, entry.Id })
            .HasDatabaseName($"ix_{TableName}_centre_occurred")
            .IsDescending(false, true, true);

        builder
            .HasIndex(entry => new { entry.CentreId, entry.EntityType, entry.EntityId, entry.OccurredAt })
            .HasDatabaseName($"ix_{TableName}_centre_entity")
            .IsDescending(false, false, false, true);

        // No timestamps convention and no row version: an audit row never changes after it is written.
    }

    private static string ActorTypeToDatabase(AuditActorType value) => value switch
```

The excerpt stops at line 90; use the original file for the remaining 30 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
