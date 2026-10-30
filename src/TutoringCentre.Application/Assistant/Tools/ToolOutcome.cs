using TutoringCentre.Domain.Assistant;
namespace TutoringCentre.Application.Assistant.Tools;
public sealed record ToolOutcome(ToolDecision Decision, string? ReasonCode, string Text);
