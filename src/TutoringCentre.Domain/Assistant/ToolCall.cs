using TutoringCentre.Domain.Common;
namespace TutoringCentre.Domain.Assistant;

/// <summary>Append-only, content-free evidence of a single gateway decision.</summary>
public sealed class ToolCall : Entity, ITenantOwned
{
    private ToolCall() { }
    public Guid CentreId { get; private set; }
    public Guid AgentRunId { get; private set; }
    public int Step { get; private set; }
    public int Ordinal { get; private set; }
    public string ToolName { get; private set; } = "";
    public ToolDecision Decision { get; private set; }
    public string? ReasonCode { get; private set; }
    public string ArgumentSummary { get; private set; } = "";
    public string? ResultSummary { get; private set; }
    public int DurationMs { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public static ToolCall Record(Guid centreId, Guid agentRunId, int step, int ordinal, string toolName,
        ToolDecision decision, string? reasonCode, string argumentSummary, string? resultSummary, int durationMs, DateTimeOffset now)
    {
        if (centreId == Guid.Empty || agentRunId == Guid.Empty || step < 0 || ordinal < 0 || durationMs < 0
            || !Enum.IsDefined(decision) || (decision != ToolDecision.Allowed && string.IsNullOrWhiteSpace(reasonCode)))
            throw new ArgumentException("Invalid tool decision evidence.");
        return new() { CentreId = centreId, AgentRunId = agentRunId, Step = step, Ordinal = ordinal,
            ToolName = Cut(toolName, 60), Decision = decision, ReasonCode = reasonCode is null ? null : Cut(reasonCode, 60),
            ArgumentSummary = Cut(argumentSummary, 200), ResultSummary = resultSummary is null ? null : Cut(resultSummary, 200),
            DurationMs = durationMs, CreatedAt = now };
    }
    private static string Cut(string value, int length) => string.Concat(value.EnumerateRunes().Take(length).Select(x => x.ToString()));
}
