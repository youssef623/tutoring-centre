using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Conversations.Contracts;
/// <summary>Tenant-scoped conversational data and reply intent. No record lookup tools are exposed.</summary>
public interface IAssistantConversationGateway
{
    Task<AssistantConversationInput?> LoadAsync(Guid inboundId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AssistantHistoryEntry>> GetHistoryAsync(Guid inboundId, int count, CancellationToken cancellationToken);
    Task<Result> QueueReplyAsync(Guid inboundId, string reply, CancellationToken cancellationToken);
}
public sealed record AssistantConversationInput(Guid ConversationId, Guid? ParentId, Guid? StudentId, bool CanReply, string Text);
public sealed record AssistantHistoryEntry(bool IsAssistant, string Text);
