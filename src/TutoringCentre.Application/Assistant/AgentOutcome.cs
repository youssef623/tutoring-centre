using TutoringCentre.Application.Assistant.Models;
namespace TutoringCentre.Application.Assistant;
public sealed record AgentOutcome(string? Reply, int Steps, TokenUsage Usage, string? FailureCode = null, string? GuardReason = null);
