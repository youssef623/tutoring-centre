namespace TutoringCentre.Application.Assistant.Models;

public sealed record ModelResponse(string? Text, IReadOnlyList<ToolCallRequest> ToolCalls, TokenUsage Usage, ModelStopReason StopReason);
