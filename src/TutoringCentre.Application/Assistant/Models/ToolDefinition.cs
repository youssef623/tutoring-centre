using System.Text.Json;

namespace TutoringCentre.Application.Assistant.Models;

public sealed record ToolDefinition(string Name, string Description, JsonElement Parameters);
