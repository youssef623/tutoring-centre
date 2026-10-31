using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Domain.Conversations;
namespace TutoringCentre.Application.Conversations;
public interface IAgentConversationRepository
{
    Task<bool> HasEarlierPendingAsync(Guid inboundId, CancellationToken ct);
    Task<InboundMessage?> GetInboundAsync(Guid id, CancellationToken cancellationToken);
    Task<Conversation?> GetConversationAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<AssistantHistoryEntry>> GetHistoryAsync(InboundMessage inbound, int count, CancellationToken cancellationToken);
}
