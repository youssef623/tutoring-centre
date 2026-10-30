using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.Assistant;
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class AgentConversationRepository(AppDbContext db) : IAgentConversationRepository
{
    public Task<InboundMessage?> GetInboundAsync(Guid id, CancellationToken cancellationToken) =>
        db.Set<InboundMessage>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<Conversation?> GetConversationAsync(Guid id, CancellationToken cancellationToken) =>
        db.Set<Conversation>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<IReadOnlyList<AssistantHistoryEntry>> GetHistoryAsync(InboundMessage inbound, int count, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(inbound);
        var users = await db.Set<InboundMessage>().Where(x => x.ConversationId == inbound.ConversationId && x.Id != inbound.Id
                && (x.ReceivedAt < inbound.ReceivedAt || (x.ReceivedAt == inbound.ReceivedAt && x.Id.CompareTo(inbound.Id) < 0)) && x.TextBody != null)
            .OrderByDescending(x => x.ReceivedAt).ThenByDescending(x => x.Id).Take(count)
            .Select(x => new { At = x.ReceivedAt, x.Id, Text = x.TextBody! }).ToListAsync(cancellationToken);
        var replies = await db.Set<OutboundMessage>().Where(x => x.ConversationId == inbound.ConversationId
                && EF.Property<DateTimeOffset>(x, "CreatedAt") <= inbound.ReceivedAt && x.ReplyToInboundId != inbound.Id)
            .OrderByDescending(x => EF.Property<DateTimeOffset>(x, "CreatedAt")).ThenByDescending(x => x.Id).Take(count)
            .Select(x => new { At = EF.Property<DateTimeOffset>(x, "CreatedAt"), x.Id, x.Text }).ToListAsync(cancellationToken);
        return users.Select(x => (x.At, x.Id, Message: new AssistantHistoryEntry(false, x.Text)))
            .Concat(replies.Select(x => (x.At, x.Id, Message: new AssistantHistoryEntry(true, x.Text))))
            .OrderBy(x => x.At).ThenBy(x => x.Id).TakeLast(count).Select(x => x.Message).ToArray();
    }
}
