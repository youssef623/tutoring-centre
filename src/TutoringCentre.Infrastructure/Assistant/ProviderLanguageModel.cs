using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace TutoringCentre.Infrastructure.Assistant;

public sealed partial class ProviderLanguageModel(IChatClient client, IOptions<AssistantOptions> options, ILogger<ProviderLanguageModel> logger, string? modelName = null) : ILanguageModel
{
    public async Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (cancellationToken.IsCancellationRequested) return Failure("assistant.model_cancelled");
        var started = Stopwatch.GetTimestamp();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.Value.CallTimeout);
        var outcome = "error";
        long input = 0, output = 0;
        try
        {
            var messages = new List<ChatMessage> { new(ChatRole.System, request.System) };
            foreach (var message in request.Messages)
            {
                var converted = new ChatMessage(message.Role switch { ModelRole.User => ChatRole.User, ModelRole.Assistant => ChatRole.Assistant, _ => ChatRole.Tool }, new List<AIContent>());
                if (message.Role == ModelRole.Tool)
                    converted.Contents.Add(new FunctionResultContent(message.ToolCallId!, message.Text));
                else if (message.Text is not null) converted.Contents.Add(new TextContent(message.Text));
                foreach (var call in message.ToolCalls ?? [])
                    converted.Contents.Add(new FunctionCallContent(call.Id, call.Name, JsonSerializer.Deserialize<Dictionary<string, object?>>(call.Arguments.GetRawText())));
                messages.Add(converted);
            }
            var response = await client.GetResponseAsync(messages, new ChatOptions
            {
                MaxOutputTokens = Math.Min(request.MaxOutputTokens, options.Value.MaxTotalTokens),
                Tools = request.Tools.Select(tool => (AITool)new Declaration(tool)).ToList(),
            }, timeout.Token).WaitAsync(timeout.Token);
            var calls = response.Messages.SelectMany(message => message.Contents).OfType<FunctionCallContent>()
                .Select(call => new ToolCallRequest(call.CallId, call.Name, JsonSerializer.SerializeToElement(call.Arguments ?? new Dictionary<string, object?>()))).ToArray();
            input = response.Usage?.InputTokenCount ?? 0;
            output = response.Usage?.OutputTokenCount ?? 0;
            var reason = response.FinishReason == ChatFinishReason.Length ? ModelStopReason.Length
                : calls.Length > 0 ? ModelStopReason.ToolCalls : response.FinishReason == ChatFinishReason.Stop ? ModelStopReason.Completed : ModelStopReason.Other;
            outcome = "success";
            return Result<ModelResponse>.Success(new(string.IsNullOrEmpty(response.Text) ? null : response.Text, calls, new(input, output), reason));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            outcome = "cancelled";
            return Failure("assistant.model_cancelled");
        }
        catch (OperationCanceledException)
        {
            outcome = "timeout";
            return Failure("assistant.model_timeout");
        }
        catch (TimeoutException)
        {
            outcome = "timeout";
            return Failure("assistant.model_timeout");
        }
        catch (Exception)
        {
            return Failure("assistant.model_error");
        }
        finally
        {
            // Deliberately exclude exception objects and all message/tool content.
            if (logger.IsEnabled(LogLevel.Information))
            {
                var duration = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                LogOutcome(logger, modelName, input, output, duration, outcome);
            }
        }
    }

    private static Result<ModelResponse> Failure(string code) => Result<ModelResponse>.Failure(Error.Rule(code, "The model call did not complete."));

    [LoggerMessage(Level = LogLevel.Information, Message = "Model {Model} input {InputTokens} output {OutputTokens} duration {DurationMs} outcome {Outcome}")]
    private static partial void LogOutcome(ILogger logger, string? model, long inputTokens, long outputTokens, double durationMs, string outcome);

    private sealed class Declaration(ToolDefinition tool) : AIFunctionDeclaration
    {
        public override string Name => tool.Name;
        public override string Description => tool.Description;
        public override JsonElement JsonSchema => tool.Parameters;
    }
}
