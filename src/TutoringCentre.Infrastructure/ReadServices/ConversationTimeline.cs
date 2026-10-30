using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

internal sealed record TimelineMessage(Guid Id, string Direction, string? Text, string? Status, string? FailureCode,
    DateTimeOffset CreatedAt, DateTimeOffset? SentAt, DateTimeOffset? ProcessedAt);

internal static class ConversationTimeline
{
    public static async Task<IReadOnlyList<TimelineMessage>> ReadAsync(AppDbContext db, Guid centreId,
        IQueryable<Guid> conversationIds, string? cursor, int pageSize, CancellationToken ct)
    {
        var inbound = db.Set<InboundMessage>().AsNoTracking().Where(item => item.CentreId == centreId && conversationIds.Contains(item.ConversationId));
        var outbound = db.Set<OutboundMessage>().AsNoTracking().Where(item => item.CentreId == centreId && conversationIds.Contains(item.ConversationId));
        if (cursor is not null)
        {
            if (!ChatCursor.TryParse(cursor, out var at, out var id)) throw new ArgumentException("Invalid cursor.", nameof(cursor));
            inbound = inbound.Where(item => EF.Property<DateTimeOffset>(item, "CreatedAt") < at
                || (EF.Property<DateTimeOffset>(item, "CreatedAt") == at && item.Id.CompareTo(id) < 0));
            outbound = outbound.Where(item => EF.Property<DateTimeOffset>(item, "CreatedAt") < at
                || (EF.Property<DateTimeOffset>(item, "CreatedAt") == at && item.Id.CompareTo(id) < 0));
        }
        // Bound each side before merging. Keep enum conversions on their entity projection:
        // a UNION with a null enum branch loses EF's string value converter.
        var incoming = await inbound.OrderByDescending(item => EF.Property<DateTimeOffset>(item, "CreatedAt"))
            .ThenByDescending(item => item.Id).Take(pageSize + 1)
            .Select(item => new { item.Id, item.TextBody, item.Status, CreatedAt = EF.Property<DateTimeOffset>(item, "CreatedAt"), item.ProcessedAt }).ToListAsync(ct);
        var outgoing = await outbound.OrderByDescending(item => EF.Property<DateTimeOffset>(item, "CreatedAt"))
            .ThenByDescending(item => item.Id).Take(pageSize + 1)
            .Select(item => new { item.Id, item.Text, item.Status, item.FailureCode, CreatedAt = EF.Property<DateTimeOffset>(item, "CreatedAt"), item.SentAt }).ToListAsync(ct);
        return incoming.Select(item => new TimelineMessage(item.Id, "inbound", item.TextBody, item.Status.ToString().ToLowerInvariant(), null, item.CreatedAt, null, item.ProcessedAt))
            .Concat(outgoing.Select(item => new TimelineMessage(item.Id, "outbound", item.Text, item.Status.ToString().ToLowerInvariant(), item.FailureCode, item.CreatedAt, item.SentAt, null)))
            .OrderByDescending(item => item.CreatedAt).ThenByDescending(item => item.Id).Take(pageSize + 1).ToArray();
    }
}
