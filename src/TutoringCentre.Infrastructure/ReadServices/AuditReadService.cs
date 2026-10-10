using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Audit;
using TutoringCentre.Infrastructure.Audit;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of the audit log: a no-tracking projection over audit.audit_entries, joined to users only for the
/// actor's display name (never their email). No tenant predicate written by hand — the EF query filter and
/// row-level security (Day 31) scope this the same as <see cref="SubjectReadService"/>. Orders newest first by
/// (occurredAt, id), both descending, and never by timestamp alone, so a cursor resumes unambiguously even when
/// two rows share a timestamp.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AuditReadService(AppDbContext db) : IAuditReadService
{
    public async Task<AuditPageDto> GetPageAsync(
        string? entityType,
        Guid? entityId,
        Guid? actorUserId,
        DateTimeOffset? occurredFrom,
        DateTimeOffset? occurredTo,
        string? cursor,
        int pageSize,
        CancellationToken ct)
    {
        var entries = db.Set<AuditEntry>().AsNoTracking().AsQueryable();

        if (entityType is not null && Enum.TryParse<AuditEntityType>(entityType, ignoreCase: true, out var parsedEntityType))
        {
            entries = entries.Where(entry => entry.EntityType == parsedEntityType);
        }

        if (entityId is not null)
        {
            entries = entries.Where(entry => entry.EntityId == entityId.Value);
        }

        if (actorUserId is not null)
        {
            entries = entries.Where(entry => entry.ActorUserId == actorUserId.Value);
        }

        if (occurredFrom is not null)
        {
            entries = entries.Where(entry => entry.OccurredAt >= occurredFrom.Value);
        }

        if (occurredTo is not null)
        {
            entries = entries.Where(entry => entry.OccurredAt <= occurredTo.Value);
        }

        if (cursor is not null && AuditCursor.TryDecode(cursor, out var after))
        {
            entries = entries.Where(entry =>
                entry.OccurredAt < after.OccurredAt
                || (entry.OccurredAt == after.OccurredAt && entry.Id.CompareTo(after.Id) < 0));
        }

        var rows = await (
            from entry in entries
            join user in db.Set<ApplicationUser>().AsNoTracking() on entry.ActorUserId equals user.Id into actorUsers
            from actorUser in actorUsers.DefaultIfEmpty()
            orderby entry.OccurredAt descending, entry.Id descending
            select new { entry, ActorDisplayName = (string?)actorUser.DisplayName })
            .Take(pageSize + 1)
            .ToListAsync(ct);

        var hasNextPage = rows.Count > pageSize;
        var page = hasNextPage ? rows.Take(pageSize).ToList() : rows;

        var items = page.Select(row => ToDto(row.entry, row.ActorDisplayName)).ToList();
        var nextCursor = hasNextPage ? AuditCursor.Encode(page[^1].entry.OccurredAt, page[^1].entry.Id) : null;

        return new AuditPageDto(items, nextCursor);
    }

    private static AuditEntryDto ToDto(AuditEntry entry, string? actorDisplayName) => new(
        entry.Id,
        entry.OccurredAt,
        entry.ActorType.ToString().ToLowerInvariant(),
        entry.ActorUserId,
        actorDisplayName,
        entry.Action.ToString().ToLowerInvariant(),
        entry.EntityType.ToString().ToLowerInvariant(),
        entry.EntityId,
        ParseChanges(entry.Changes),
        entry.CorrelationId);

    private static List<AuditChangeDto> ParseChanges(string changesJson)
    {
        using var document = JsonDocument.Parse(changesJson);
        var changes = new List<AuditChangeDto>(document.RootElement.EnumerateObject().Count());

        foreach (var field in document.RootElement.EnumerateObject())
        {
            var before = field.Value.GetProperty("before");
            var after = field.Value.GetProperty("after");
            changes.Add(new AuditChangeDto(
                field.Name,
                before.ValueKind == JsonValueKind.Null ? null : before.GetString(),
                after.ValueKind == JsonValueKind.Null ? null : after.GetString()));
        }

        return changes;
    }
}
