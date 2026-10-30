using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using TutoringCentre.Application.Assistant.Commands.RunAgent;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Application.Conversations.Inbound.Commands.ProcessInboundMessage;
public sealed record ProcessInboundMessageCommand(Guid InboundMessageId) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ConversationsIngest;
}
internal sealed class ProcessInboundMessageValidator : AbstractValidator<ProcessInboundMessageCommand>
{
    public ProcessInboundMessageValidator() => RuleFor(x => x.InboundMessageId).NotEmpty();
}
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ProcessInboundMessageHandler(IAgentConversationRepository messages, IClock clock,
    ICurrentActor actor, ChannelSenderRegistry senders, IOutbox outbox) : ICommandHandler<ProcessInboundMessageCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ProcessInboundMessageCommand command, CancellationToken cancellationToken)
    {
        if (actor.Actor is not SystemActor) return Result<Unit>.Failure(Error.Forbidden("assistant.system_only", "Only the system may process inbound messages."));
        var inbound = await messages.GetInboundAsync(command.InboundMessageId, cancellationToken);
        if (inbound is null) return Result<Unit>.Failure(Error.NotFound("conversations.message_not_found", "Message not found."));
        if (inbound.Status == InboundMessageStatus.Processed) return Result<Unit>.Success(Unit.Value);
        var conversation = await messages.GetConversationAsync(inbound.ConversationId, cancellationToken);
        if (conversation is null) return Result<Unit>.Failure(Error.NotFound("conversations.not_found", "Conversation not found."));
        if (conversation.IdentityStatus == IdentityStatus.Identified && senders.Supports(inbound.Provider))
            outbox.Enqueue(new RunAgentRequested(inbound.Id));
        var processed = inbound.MarkProcessed(clock.UtcNow);
        return processed.IsFailure ? Result<Unit>.Failure(processed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
