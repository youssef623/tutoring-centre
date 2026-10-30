using TutoringCentre.Domain.Common;
namespace TutoringCentre.Domain.Assistant;

/// <summary>Content-free execution record claimed by a unique inbound key.</summary>
public sealed class AgentRun : Entity, ITenantOwned
{
    private AgentRun() { }
    public Guid CentreId { get; private set; }
    public Guid ConversationId { get; private set; }
    public Guid InboundMessageId { get; private set; }
    public AgentRunStatus Status { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public int Steps { get; private set; }
    public int InputTokens { get; private set; }
    public int OutputTokens { get; private set; }
    public string? FailureCode { get; private set; }
    public string? GuardReason { get; private set; }
    public static AgentRun Start(Guid centreId, Guid conversationId, Guid inboundMessageId, DateTimeOffset now)
    {
        if (centreId == Guid.Empty || conversationId == Guid.Empty || inboundMessageId == Guid.Empty)
            throw new ArgumentException("Run identifiers must be nonempty.");
        return new AgentRun { CentreId = centreId, ConversationId = conversationId, InboundMessageId = inboundMessageId, StartedAt = now };
    }
    public Result Complete(int steps, int inputTokens, int outputTokens, DateTimeOffset now, string? guardReason = null) =>
        Finish(null, steps, inputTokens, outputTokens, now, guardReason);
    public Result Fail(string code, int steps, int inputTokens, int outputTokens, DateTimeOffset now) =>
        !ValidCode(code) ? Invalid() : Finish(code, steps, inputTokens, outputTokens, now, null);
    private Result Finish(string? code, int steps, int input, int output, DateTimeOffset now, string? guard)
    {
        if (Status != AgentRunStatus.Running)
            return Result.Failure(Error.Rule("assistant.run_finished", "The run has already finished."));
        if (steps < 0 || input < 0 || output < 0 || now < StartedAt || (guard is not null && !ValidCode(guard))) return Invalid();
        Status = code is null ? AgentRunStatus.Completed : AgentRunStatus.Failed;
        FinishedAt = now; Steps = steps; InputTokens = input; OutputTokens = output; FailureCode = code; GuardReason = guard;
        return Result.Success();
    }
    private static bool ValidCode(string? code) => code is { Length: > 0 and <= 60 }
        && code.All(c => c is >= 'a' and <= 'z' or >= '0' and <= '9' or '.' or '_' or '-');
    private static Result Invalid() => Result.Failure(Error.Validation("assistant.run_invalid", "Run counts, timestamps or codes are invalid."));
}
