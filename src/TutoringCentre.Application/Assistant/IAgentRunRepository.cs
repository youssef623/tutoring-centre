using TutoringCentre.Domain.Assistant;
namespace TutoringCentre.Application.Assistant;
public interface IAgentRunRepository
{
    Task<AgentRun?> TryStartAsync(Guid conversationId, Guid inboundMessageId, DateTimeOffset now, CancellationToken cancellationToken);
}
