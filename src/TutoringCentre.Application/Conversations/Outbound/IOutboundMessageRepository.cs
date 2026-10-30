using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations.Outbound;

public interface IOutboundMessageRepository
{
    Task<OutboundMessage?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken);
    Task<ChannelAccount?> GetChannelAsync(Guid conversationId, CancellationToken cancellationToken);
    void Add(OutboundMessage message);
}
