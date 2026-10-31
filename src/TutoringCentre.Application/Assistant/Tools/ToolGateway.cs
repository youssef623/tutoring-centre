using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Execution;
using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Domain.Assistant;
namespace TutoringCentre.Application.Assistant.Tools;

/// <summary>The only execution door. Model-controlled strings never enter audit summaries.</summary>
public sealed class ToolGateway(ToolRegistry registry, IActorScopeRunner scopes, PortalActorFactory actors,
    IAssistantConversationGateway conversations, IToolCallLog log, AssistantOptions options)
{
    public async Task<ToolOutcome> ExecuteAsync(ToolContext context, int step, int ordinal, string name, string argumentsJson, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);
        var start = Stopwatch.GetTimestamp();
        ITool? tool = null;
        var argumentSummary = "arguments withheld";
        string? resultSummary = null;
        var outcome = new ToolOutcome(ToolDecision.Failed, "tool.internal_error", "Tool could not be completed (tool.internal_error).");
        try
        {
            tool = registry.Find(name);
            context.AttemptedCalls++;
            if (context.AttemptedCalls > options.MaxToolCalls) outcome = Deny("tool.call_limit");
            else if (tool is null) outcome = Deny("tool.unknown");
            else
            {
                var arguments = ToolArguments.Parse(tool.Descriptor.Schema, argumentsJson);
                if (arguments.IsFailure) outcome = new(ToolDecision.Invalid, "tool.arguments_invalid", "Invalid tool arguments (tool.arguments_invalid).");
                else
                {
                    argumentSummary = arguments.Value.HandleSummary();
                    var conversation = await conversations.LoadAsync(context.InboundMessageId, ct);
                    var actor = conversation is null ? null : await actors.ForConversationAsync(conversation, ct);
                    if (actor is null || actor.IsFailure) outcome = Deny("assistant.account_inactive");
                    else
                    {
                        var result = await scopes.RunAsync(actor.Value, async provider =>
                        {
                            var scopedTool = provider.GetRequiredService<ToolRegistry>().Find(name)!;
                            return await scopedTool.ExecuteAsync(context, arguments.Value, ct).WaitAsync(ct);
                        });
                        if (result.IsSuccess)
                        {
                            resultSummary = result.Value.Data is GetAttendance.GetAttendanceToolResult attendance ? $"{attendance.Records.Count} record{(attendance.Records.Count == 1 ? "" : "s")}" : "result returned";
                            context.HasAttendance |= name == "get_attendance";
                            context.ReturnedFacts += JsonSerializer.Serialize(result.Value.Data, JsonSerializerOptions.Web);
                        }
                        outcome = result.IsFailure ? Deny(result.Error!.Code)
                            : new(ToolDecision.Allowed, null, Context.ContextBuilder.Truncate(JsonSerializer.Serialize(result.Value.Data, JsonSerializerOptions.Web), 4000));
                    }
                }
            }
        }
        catch (Exception) { /* no provider or exception text crosses this boundary */ }
        var auditName = System.Text.RegularExpressions.Regex.IsMatch(name, "^[a-z][a-z_]{2,39}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)) ? name : "invalid_name";
        log.Add(ToolCall.Record(context.CentreId, context.AgentRunId, step, ordinal, auditName,
            outcome.Decision, outcome.ReasonCode, argumentSummary, outcome.Decision == ToolDecision.Allowed ? resultSummary : "result withheld",
            (int)Math.Min(int.MaxValue, Stopwatch.GetElapsedTime(start).TotalMilliseconds), DateTimeOffset.UtcNow));
        return outcome;
    }
    private static ToolOutcome Deny(string code) => System.Text.RegularExpressions.Regex.IsMatch(code, "^[a-z][a-z0-9_.-]{0,59}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))
        ? new(ToolDecision.Denied, code, $"Tool request denied ({code}).")
        : new(ToolDecision.Failed, "tool.internal_error", "Tool could not be completed (tool.internal_error).");
}
