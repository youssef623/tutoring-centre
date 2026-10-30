namespace TutoringCentre.Application.Assistant.Models;

public sealed record ModelRequest(string System, IReadOnlyList<ModelMessage> Messages, IReadOnlyList<ToolDefinition> Tools, int MaxOutputTokens);
