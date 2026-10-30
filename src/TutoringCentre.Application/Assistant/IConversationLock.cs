namespace TutoringCentre.Application.Assistant;
public interface IConversationLock
{
    Task<bool> TryAcquireAsync(Guid centreId, Guid conversationId, CancellationToken ct);
}
