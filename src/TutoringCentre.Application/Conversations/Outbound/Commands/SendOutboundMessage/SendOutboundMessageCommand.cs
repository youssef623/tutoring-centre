using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Conversations.Outbound.Commands.SendOutboundMessage;

public sealed record SendOutboundMessageCommand(Guid OutboundMessageId) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ConversationsIngest;
}

public sealed record SendOutboundMessageRequested(Guid OutboundMessageId) : IOutboxMessage
{
    public static string TypeName => "conversations.send_outbound.v1";
}

internal sealed class SendOutboundMessageValidator : AbstractValidator<SendOutboundMessageCommand>
{
    public SendOutboundMessageValidator() => RuleFor(command => command.OutboundMessageId).NotEmpty();
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class SendOutboundMessageHandler(IOutboundMessageRepository messages,
    ChannelSenderRegistry senders, IClock clock) : ICommandHandler<SendOutboundMessageCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(SendOutboundMessageCommand command, CancellationToken cancellationToken)
    {
        var message = await messages.GetForUpdateAsync(command.OutboundMessageId, cancellationToken);
        if (message is null) return Result<Unit>.Failure(Error.NotFound("outbound.not_found", "Message not found."));
        if (message.Status != OutboundMessageStatus.Queued) return Result<Unit>.Success(Unit.Value);
        var account = await messages.GetChannelAsync(message.ConversationId, cancellationToken);
        if (account is null) return Result<Unit>.Failure(Error.NotFound("outbound.channel_not_found", "Channel not found."));
        var sender = senders.For(account.Provider);
        if (sender is null) return Result<Unit>.Failure(Error.Rule("outbound.sender_unavailable", "No sender supports this channel."));
        var outcome = await sender.SendAsync(message, account, cancellationToken);
        Result transition;
        switch (outcome)
        {
            case SendOutcome.Success success:
                transition = message.MarkSent(clock.UtcNow, success.ProviderId);
                break;
            case SendOutcome.Failure { Retryable: true }:
                throw new InvalidOperationException("Outbound delivery requires a retry.");
            case SendOutcome.Failure failure:
                transition = message.MarkFailed(failure.Code);
                break;
            default:
                throw new InvalidOperationException("Unknown send outcome.");
        }
        return transition.IsFailure ? Result<Unit>.Failure(transition.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
