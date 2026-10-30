using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application;
using TutoringCentre.Application.Assistant.Commands.RunAgent;
using TutoringCentre.Application.Common.Messaging;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class RetryableCodeTests
{
    [Fact]
    public void OnlyRegisteredMessageAndBusyCodeRetry()
    {
        var services = new ServiceCollection(); services.AddApplication();
        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<OutboxRegistry>();
        Assert.True(registry.IsRetryable(RunAgentRequested.TypeName, "assistant.conversation_busy"));
        Assert.False(registry.IsRetryable(RunAgentRequested.TypeName, "assistant.account_inactive"));
        Assert.False(registry.IsRetryable("other", "assistant.conversation_busy"));
    }
}
