namespace TutoringCentre.Application.Assistant;
public interface IAssistantReadService
{
    Task<ConversationTimelinePage?> GetTimelineAsync(Guid conversationId, string? cursor, int pageSize, CancellationToken ct);
}
public sealed record ConversationTimelinePage(IReadOnlyList<AssistantTimelineItem> Items, string? NextCursor);
public sealed record AssistantTimelineItem(Guid Id, string Kind, DateTimeOffset At, string? Text, string? Status,
    int? Steps = null, string? GuardReason = null, string? FailureCode = null, IReadOnlyList<TimelineToolCall>? ToolCalls = null);
public sealed record TimelineToolCall(string ToolName, string Decision, string? ReasonCode, string ArgumentSummary, string? ResultSummary, int DurationMs);
