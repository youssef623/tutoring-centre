using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Assistant.Tools;
namespace TutoringCentre.Application.Assistant;

/// <summary>The sole orchestration call site for the model. All limits are server controlled.</summary>
public sealed class AgentLoop(ILanguageModel model, AssistantOptions options, ToolGateway? gateway = null, ToolRegistry? registry = null)
{
    public Task<AgentOutcome> RunAsync(ContextOutput context, CancellationToken cancellationToken) =>
        RunWithinBudgetAsync(context, options.TotalRunTimeout, cancellationToken);

    internal async Task<AgentOutcome> RunWithinBudgetAsync(ContextOutput context, TimeSpan remainingBudget, CancellationToken cancellationToken, ToolContext? toolContext = null)
    {
        ArgumentNullException.ThrowIfNull(context);
        var limits = options;
        using var total = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (remainingBudget <= TimeSpan.Zero) return new(null, 0, new(0, 0), "assistant.total_timeout");
        total.CancelAfter(remainingBudget < limits.TotalRunTimeout ? remainingBudget : limits.TotalRunTimeout);
        long input = 0, output = 0;
        var steps = 0;
        AgentOutcome Failed(string code) => new(null, steps, new TokenUsage(input, output), code);
        for (var step = 0; step < limits.MaxSteps; step++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (input + output >= limits.MaxTotalTokens) return Failed("assistant.token_limit");
            using var call = CancellationTokenSource.CreateLinkedTokenSource(total.Token);
            call.CancelAfter(limits.CallTimeout);
            try
            {
                steps++;
                var request = new ModelRequest(context.System, context.Messages, registry?.Descriptors.Select(x => new ToolDefinition(x.Name, x.Description, x.Schema)).ToArray() ?? [], (int)(limits.MaxTotalTokens - input - output));
                var result = await model.GenerateAsync(request, call.Token).WaitAsync(call.Token);
                cancellationToken.ThrowIfCancellationRequested();
                if (total.IsCancellationRequested) return Failed("assistant.total_timeout");
                if (result.IsFailure) return Failed(result.Error!.Code == "assistant.model_timeout" ? "assistant.call_timeout" : "assistant.model_error");
                var response = result.Value;
                if (response.Usage.InputTokens < 0 || response.Usage.OutputTokens < 0
                    || response.Usage.InputTokens > int.MaxValue - input || response.Usage.OutputTokens > int.MaxValue - output)
                    return Failed("assistant.model_error");
                input += response.Usage.InputTokens; output += response.Usage.OutputTokens;
                if (input + output >= limits.MaxTotalTokens) return Failed("assistant.token_limit");
                if (response.ToolCalls.Count != 0 || response.StopReason == ModelStopReason.ToolCalls)
                {
                    if (gateway is null || toolContext is null) return Failed("assistant.tools_unavailable");
                    var messages = context.Messages.ToList();
                    messages.Add(new ModelMessage(ModelRole.Assistant, response.Text, response.ToolCalls));
                    for (var ordinal = 0; ordinal < response.ToolCalls.Count; ordinal++)
                    {
                        var toolCall = response.ToolCalls[ordinal];
                        var outcome = await gateway.ExecuteAsync(toolContext, steps, ordinal, toolCall.Name, toolCall.Arguments.GetRawText(), total.Token);
                        messages.Add(new ModelMessage(ModelRole.Tool, outcome.Text, ToolCallId: toolCall.Id));
                    }
                    context = context with { Messages = messages };
                    if (toolContext.AttemptedCalls > limits.MaxToolCalls) return Failed("assistant.tool_call_limit");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(response.Text)) return Failed("assistant.model_error");
                if (response.StopReason == ModelStopReason.Other)
                {
                    context = context with { Messages = [.. context.Messages, new ModelMessage(ModelRole.Assistant, response.Text)] };
                    continue;
                }
                var reply = CapReply(response.Text.Trim(), limits.MaxReplyCharacters);
                return new AgentOutcome(reply, steps, new TokenUsage(input, output), GuardReason: reply.Length < response.Text.Trim().Length ? "assistant.reply_capped" : null);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return Failed(total.IsCancellationRequested ? "assistant.total_timeout" : "assistant.call_timeout");
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                return Failed("assistant.model_error");
            }
        }
        return Failed("assistant.step_limit");
    }
    private static string CapReply(string text, int cap)
    {
        var limited = ContextBuilder.Truncate(text, cap);
        if (limited.Length == text.Length) return text;
        var boundary = limited.LastIndexOfAny(['.', '!', '?', '\u061f', '\u3002']);
        return boundary >= 0 ? limited[..(boundary + 1)] : limited.TrimEnd();
    }
}
