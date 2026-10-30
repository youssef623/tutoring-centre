using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Assistant.Commands.RunAgent;
using TutoringCentre.Application.Common.Cqrs;
namespace TutoringCentre.Application.Assistant;
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class AgentCommandTimeoutPolicy(AssistantOptions? options = null) : ICommandTimeoutPolicy
{
    public TimeSpan? GetTimeout(object command) => command is RunAgentCommand ? (options ?? new AssistantOptions()).TotalRunTimeout : null;
}
