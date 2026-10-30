using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Application.Assistant.Commands.RunAgent;
public sealed record RunAgentCommand(Guid InboundMessageId) : ICommand<RunAgentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ConversationsIngest;
}
public sealed record RunAgentResult(bool AlreadyRan);
public sealed record RunAgentRequested(Guid InboundMessageId) : IOutboxMessage
{
    public static string TypeName => "assistant.run_agent.v1";
}
internal sealed class RunAgentValidator : AbstractValidator<RunAgentCommand>
{
    public RunAgentValidator() => RuleFor(x => x.InboundMessageId).NotEmpty();
}
