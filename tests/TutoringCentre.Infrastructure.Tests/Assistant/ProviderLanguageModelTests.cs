using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Infrastructure.Assistant;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace TutoringCentre.Infrastructure.Tests.Assistant;

public sealed class ProviderLanguageModelTests
{
    private static ModelRequest Request => new("Secret system", [new(ModelRole.User, "Secret prompt")], [], 100);

    [Fact]
    public async Task Success_TranslatesTextUsageAndToolsInBothDirections()
    {
        using var json = JsonDocument.Parse("""{"type":"object","properties":{"id":{"type":"string"}}}""");
        using var fake = new FakeClient((messages, options, _) =>
        {
            Assert.Equal(ChatRole.System, messages.First().Role);
            Assert.Equal(100, options!.MaxOutputTokens);
            Assert.Equal("lookup", Assert.IsAssignableFrom<AIFunctionDeclaration>(Assert.Single(options.Tools!)).Name);
            return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, [
                new TextContent("Reply"), new FunctionCallContent("call-1", "lookup", new Dictionary<string, object?> { ["id"] = "42" })]))
            { Usage = new() { InputTokenCount = 12, OutputTokenCount = 5 }, FinishReason = ChatFinishReason.ToolCalls });
        });
        var request = Request with { Tools = [new("lookup", "Lookup", json.RootElement)] };
        var result = await Model(fake).GenerateAsync(request, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal("Reply", result.Value.Text);
        Assert.Equal(new TokenUsage(12, 5), result.Value.Usage);
        Assert.Equal(ModelStopReason.ToolCalls, result.Value.StopReason);
        Assert.Equal("42", Assert.Single(result.Value.ToolCalls).Arguments.GetProperty("id").GetString());
    }

    [Fact]
    public async Task ProviderException_IsResultWithoutExceptionText()
    {
        using var fake = new FakeClient((_, _, _) => throw new InvalidOperationException("secret provider body"));
        var result = await Model(fake).GenerateAsync(Request, CancellationToken.None);
        Assert.Equal("assistant.model_error", result.Error?.Code);
        Assert.DoesNotContain("secret", result.Error!.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Timeout_IsDistinctFailureEvenIfProviderIgnoresCancellation()
    {
        var completion = new TaskCompletionSource<ChatResponse>();
        using var fake = new FakeClient((_, _, _) => completion.Task);
        var result = await Model(fake, TimeSpan.FromMilliseconds(20)).GenerateAsync(Request, CancellationToken.None);
        Assert.Equal("assistant.model_timeout", result.Error?.Code);
        completion.SetResult(new(new ChatMessage(ChatRole.Assistant, "late reply")));
    }

    [Fact]
    public async Task CancelledCall_IsFailure()
    {
        using var fake = new FakeClient(async (_, _, ct) => { await Task.Delay(Timeout.Infinite, ct); return null!; });
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        var result = await Model(fake).GenerateAsync(Request, cancelled.Token);
        Assert.Equal("assistant.model_cancelled", result.Error?.Code);
    }

    [Fact]
    public async Task CancellationDuringCall_StopsWaitingAndCancelsProviderToken()
    {
        var started = new TaskCompletionSource<CancellationToken>();
        using var fake = new FakeClient(async (_, _, ct) =>
        {
            started.SetResult(ct);
            await Task.Delay(Timeout.Infinite, ct);
            return null!;
        });
        using var cancellation = new CancellationTokenSource();
        var pending = Model(fake).GenerateAsync(Request, cancellation.Token);
        var providerToken = await started.Task;
        await cancellation.CancelAsync();
        Assert.Equal("assistant.model_cancelled", (await pending).Error?.Code);
        Assert.True(providerToken.IsCancellationRequested);
    }

    [Fact]
    public async Task ProviderTimeout_IsTimeoutResult()
    {
        using var fake = new FakeClient((_, _, _) => throw new TimeoutException("provider details"));
        Assert.Equal("assistant.model_timeout", (await Model(fake).GenerateAsync(Request, CancellationToken.None)).Error?.Code);
    }

    private static ProviderLanguageModel Model(IChatClient client, TimeSpan? timeout = null) =>
        new(client, Options.Create(new AssistantOptions { CallTimeout = timeout ?? TimeSpan.FromSeconds(20) }), NullLogger<ProviderLanguageModel>.Instance);

    private sealed class FakeClient(Func<IEnumerable<ChatMessage>, ChatOptions?, CancellationToken, Task<ChatResponse>> response) : IChatClient
    {
        public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default) => response(messages, options, cancellationToken);
        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public object? GetService(Type serviceType, object? serviceKey = null) => null;
        public void Dispose() { }
    }
}
