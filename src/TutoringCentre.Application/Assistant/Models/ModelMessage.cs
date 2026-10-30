namespace TutoringCentre.Application.Assistant.Models;

public enum ModelRole { User, Assistant, Tool }
public sealed record ModelMessage(ModelRole Role, string? Text, IReadOnlyList<ToolCallRequest>? ToolCalls = null, string? ToolCallId = null);
