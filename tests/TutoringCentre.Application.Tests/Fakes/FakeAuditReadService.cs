using TutoringCentre.Application.Audit;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for audit reads. Records every filter it was last asked for, so a handler test can assert they were passed through unchanged.</summary>
public sealed class FakeAuditReadService : IAuditReadService
{
    public AuditPageDto PageToReturn { get; set; } = new([], null);

    public string? LastEntityType { get; private set; }

    public Guid? LastEntityId { get; private set; }

    public Guid? LastActorUserId { get; private set; }

    public DateTimeOffset? LastOccurredFrom { get; private set; }

    public DateTimeOffset? LastOccurredTo { get; private set; }

    public string? LastCursor { get; private set; }

    public int LastPageSize { get; private set; }

    public Task<AuditPageDto> GetPageAsync(
        string? entityType,
        Guid? entityId,
        Guid? actorUserId,
        DateTimeOffset? occurredFrom,
        DateTimeOffset? occurredTo,
        string? cursor,
        int pageSize,
        CancellationToken ct)
    {
        LastEntityType = entityType;
        LastEntityId = entityId;
        LastActorUserId = actorUserId;
        LastOccurredFrom = occurredFrom;
        LastOccurredTo = occurredTo;
        LastCursor = cursor;
        LastPageSize = pageSize;
        return Task.FromResult(PageToReturn);
    }
}
