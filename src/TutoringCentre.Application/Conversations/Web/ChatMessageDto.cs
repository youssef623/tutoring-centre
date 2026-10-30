namespace TutoringCentre.Application.Conversations.Web;

public sealed record ChatMessageDto(Guid Id, string Direction, string? Text, string? Status, DateTimeOffset CreatedAt);
public sealed record ChatMessagesPage(IReadOnlyList<ChatMessageDto> Items, string? NextCursor);
