using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations;

public interface IInboundMessageStore
{
    Task<Guid> UpsertConversationAsync(Conversation conversation, CancellationToken cancellationToken);
    Task<Guid?> TryInsertMessageAsync(InboundMessage message, Guid conversationId, CancellationToken cancellationToken);
}
