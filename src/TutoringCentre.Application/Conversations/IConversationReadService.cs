namespace TutoringCentre.Application.Conversations;

public sealed record ConversationDto(Guid Id, string Address, string Channel, DateTimeOffset LastMessageAt);
public sealed record ConversationMessageDto(Guid Id, string? Text, DateTimeOffset CreatedAt, string Direction,
    string? Status, string? FailureCode, string Channel, DateTimeOffset? SentAt, DateTimeOffset? ProcessedAt, string? CorrelationId);
public sealed record ConversationPage(IReadOnlyList<ConversationDto> Items);
public sealed record ConversationMessagesPage(IReadOnlyList<ConversationMessageDto> Items, string? NextCursor);

public interface IConversationReadService
{
    Task<ConversationPage> ListAsync(CancellationToken cancellationToken);
    Task<ConversationMessagesPage?> GetMessagesAsync(Guid conversationId, string? cursor, int pageSize, CancellationToken cancellationToken);
}
