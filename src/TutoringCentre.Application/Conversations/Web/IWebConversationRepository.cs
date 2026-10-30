using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations.Web;

public interface IWebConversationRepository
{
    Task<ChannelAccount?> GetChannelAsync(CancellationToken cancellationToken);
    Task<Guid> GetOrCreateAsync(Conversation conversation, CancellationToken cancellationToken);
    Task<Guid?> GetMessageIdAsync(Guid conversationId, string providerMessageId, CancellationToken cancellationToken);
}
