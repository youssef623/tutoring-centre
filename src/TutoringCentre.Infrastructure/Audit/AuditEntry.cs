using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// One append-only row in audit.audit_entries: who changed what, when, and the before/after values. A persistence
/// model, not a Domain entity — it has no behaviour and nothing in the Domain creates or reasons about it;
/// Infrastructure produces it during the command's SaveChanges (Task 31.6). Immutable after construction: rows
/// are never updated once written.
/// </summary>
public sealed record AuditEntry : ITenantOwned
{
    public required Guid Id { get; init; }

    public required Guid CentreId { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public required AuditActorType ActorType { get; init; }

    public Guid? ActorUserId { get; init; }

    public required AuditAction Action { get; init; }

    public required AuditEntityType EntityType { get; init; }

    public required Guid EntityId { get; init; }

    /// <summary>The changes document, as JSON text: <c>{ "&lt;field&gt;": { "before": ..., "after": ... } }</c>. Mapped to jsonb.</summary>
    public required string Changes { get; init; }

    public string? CorrelationId { get; init; }
}
