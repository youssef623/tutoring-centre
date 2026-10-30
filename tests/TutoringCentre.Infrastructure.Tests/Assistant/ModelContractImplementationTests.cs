using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Tests.Assistant;
using TutoringCentre.Infrastructure.Assistant;

namespace TutoringCentre.Infrastructure.Tests.Assistant;

public sealed class ProviderModelContractTests : ModelContractTests
{
    protected override ILanguageModel CreateModel() => new ProviderLanguageModel(new ContractClient(),
        Options.Create(new AssistantOptions()), NullLogger<ProviderLanguageModel>.Instance);

    private sealed class ContractClient : IChatClient
    {
        public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, "Hello")) { FinishReason = ChatFinishReason.Stop });
        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public object? GetService(Type serviceType, object? serviceKey = null) => null;
        public void Dispose() { }
    }
}

public sealed class ScriptedModelContractTests : ModelContractTests
{
    protected override ILanguageModel CreateModel()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, """[{"text":"Hello","toolCalls":[],"usage":{"inputTokens":1,"outputTokens":1},"stopReason":"completed"}]""");
            return new ScriptedLanguageModel(path);
        }
        finally { File.Delete(path); }
    }
}
