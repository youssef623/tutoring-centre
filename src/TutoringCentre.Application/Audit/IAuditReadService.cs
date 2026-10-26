namespace TutoringCentre.Application.Audit;

/// <summary>
/// Read-side port for the audit log. Audit is read-only from the application's point of view: no audit
/// command, repository or Domain type exists. No centre parameter: scoping comes from the acting actor, the
/// same as <c>ISubjectReadService</c> relies on the EF query filter and row-level security underneath.
/// </summary>
public interface IAuditReadService
{
    /// <summary>
    /// Newest first by (occurredAt, id), both descending. Every filter is optional and applied in SQL.
    /// <paramref name="cursor"/>, when given, resumes strictly after the last row of the page it came from.
    /// Fetches <paramref name="pageSize"/> + 1 rows internally to decide whether a next page exists.
    /// </summary>
    Task<AuditPageDto> GetPageAsync(
        string? entityType,
        Guid? entityId,
        Guid? actorUserId,
        DateTimeOffset? occurredFrom,
        DateTimeOffset? occurredTo,
        string? cursor,
        int pageSize,
        CancellationToken ct);
}

/// <summary>One page of the audit log. <see cref="NextCursor"/> is null when there is no further page.</summary>
public sealed record AuditPageDto(IReadOnlyList<AuditEntryDto> Items, string? NextCursor);

/// <summary>One audit row, shaped for the API contract. Never carries the actor's email.</summary>
public sealed record AuditEntryDto(
    Guid Id,
    DateTimeOffset OccurredAt,
    string ActorType,
    Guid? ActorUserId,
    string? ActorDisplayName,
    string Action,
    string EntityType,
    Guid EntityId,
    IReadOnlyList<AuditChangeDto> Changes,
    string? CorrelationId);

/// <summary>One changed field. Values are strings or null — the "changes" document never carries any other audited type today.</summary>
public sealed record AuditChangeDto(string Field, string? Before, string? After);
