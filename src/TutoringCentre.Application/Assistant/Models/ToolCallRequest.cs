using System.Text.Json;

namespace TutoringCentre.Application.Assistant.Models;

public sealed record ToolCallRequest(string Id, string Name, JsonElement Arguments);
