using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Web;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class WebChatReadService(AppDbContext db, ICurrentActor actor) : IWebChatReadService
{
    public async Task<ChatMessagesPage> ListAsync(string? cursor, int pageSize, CancellationToken cancellationToken)
    {
        if (actor.Actor is not IPortalActor portal) throw new InvalidOperationException("A portal actor is required.");
        var conversations = db.Set<Conversation>().Where(item => item.CentreId == portal.CentreId && item.PortalUserId == portal.UserId)
            .Select(item => item.Id);
        var rows = await ConversationTimeline.ReadAsync(db, portal.CentreId!.Value, conversations, cursor, pageSize, cancellationToken);
        var items = rows.Take(pageSize).Select(item => new ChatMessageDto(item.Id, item.Direction, item.Text, item.Status, item.CreatedAt)).ToArray();
        return new(items, rows.Count > pageSize ? ChatCursor.Format(items[^1].CreatedAt, items[^1].Id) : null);
    }
}
