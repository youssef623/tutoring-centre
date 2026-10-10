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
                value => EntityTypeToDatabase(value),
                value => EntityTypeFromDatabase(value)))
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
    {
        AuditActorType.Staff => "staff",
        AuditActorType.System => "system",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported audit actor type."),
    };

    private static AuditActorType ActorTypeFromDatabase(string value) => value switch
    {
        "staff" => AuditActorType.Staff,
        "system" => AuditActorType.System,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported actor type value in the database."),
    };

    private static string ActionToDatabase(AuditAction value) => value switch
    {
        AuditAction.Created => "created",
        AuditAction.Updated => "updated",
        AuditAction.Deleted => "deleted",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported audit action."),
    };

    private static AuditAction ActionFromDatabase(string value) => value switch
    {
        "created" => AuditAction.Created,
        "updated" => AuditAction.Updated,
        "deleted" => AuditAction.Deleted,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported action value in the database."),
    };

    private static string EntityTypeToDatabase(AuditEntityType value) => value switch
    {
        AuditEntityType.Subject => "subject",
        AuditEntityType.Membership => "membership",
        AuditEntityType.Centre => "centre",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported audit entity type."),
    };

    private static AuditEntityType EntityTypeFromDatabase(string value) => value switch
    {
        "subject" => AuditEntityType.Subject,
        "membership" => AuditEntityType.Membership,
        "centre" => AuditEntityType.Centre,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported entity type value in the database."),
    };
}
