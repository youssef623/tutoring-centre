using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class OutboundMessageRepository(AppDbContext db, ICurrentActor actor) : IOutboundMessageRepository
{
    public Task<OutboundMessage?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken) =>
        db.Set<OutboundMessage>().FromSqlInterpolated($"SELECT * FROM conversations.outbound_messages WHERE centre_id = {actor.CentreId!.Value} AND id = {id} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

    public Task<ChannelAccount?> GetChannelAsync(Guid conversationId, CancellationToken cancellationToken) =>
        (from conversation in db.Set<Conversation>()
         join account in db.Set<ChannelAccount>() on new { conversation.CentreId, Id = conversation.ChannelAccountId } equals new { account.CentreId, account.Id }
         where conversation.CentreId == actor.CentreId && account.CentreId == actor.CentreId && conversation.Id == conversationId && account.Status == "active"
         select account).SingleOrDefaultAsync(cancellationToken);

    public void Add(OutboundMessage message) => db.Set<OutboundMessage>().Add(message);
}
