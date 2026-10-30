using TutoringCentre.Application.Assistant.Handles;
namespace TutoringCentre.Application.Assistant.Tools;
/// <summary>Server-owned run identity and capabilities; never populated from tool arguments.</summary>
public sealed record ToolContext(Guid CentreId, Guid AgentRunId, Guid InboundMessageId, HandleTable Handles)
{
    internal int AttemptedCalls { get; set; }
}
