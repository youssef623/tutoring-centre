using System.Text.Json;
namespace TutoringCentre.Application.Assistant.Tools;
public sealed record ToolDescriptor(string Name, string Description, JsonElement Schema);
