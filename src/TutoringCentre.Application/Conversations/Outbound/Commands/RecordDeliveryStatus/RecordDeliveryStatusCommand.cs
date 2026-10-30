using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Conversations.Outbound.Commands.RecordDeliveryStatus;

public sealed record RecordDeliveryStatusCommand(Guid OutboundMessageId, OutboundMessageStatus Status) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ConversationsIngest;
}

public sealed record DeliveryStatusRecorded(Guid OutboundMessageId, OutboundMessageStatus Status) : IOutboxMessage
{
    public static string TypeName => "conversations.delivery_status.v1";
}

internal sealed class RecordDeliveryStatusValidator : AbstractValidator<RecordDeliveryStatusCommand>
{
    public RecordDeliveryStatusValidator()
    {
        RuleFor(command => command.OutboundMessageId).NotEmpty();
        RuleFor(command => command.Status).IsInEnum();
    }
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class RecordDeliveryStatusHandler(IOutboundMessageRepository messages, IClock clock) : ICommandHandler<RecordDeliveryStatusCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RecordDeliveryStatusCommand command, CancellationToken cancellationToken)
    {
        var message = await messages.GetForUpdateAsync(command.OutboundMessageId, cancellationToken);
        if (message is null) return Result<Unit>.Failure(Error.NotFound("outbound.not_found", "Message not found."));
        var result = command.Status switch
        {
            OutboundMessageStatus.Sent => message.MarkSent(clock.UtcNow),
            OutboundMessageStatus.Delivered => message.MarkDelivered(),
            OutboundMessageStatus.Read => message.MarkRead(),
            OutboundMessageStatus.Failed => message.MarkFailed("provider.failed"),
            OutboundMessageStatus.Queued when message.Status == OutboundMessageStatus.Queued => Result.Success(),
            _ => Result.Failure(Error.Rule("outbound.invalid_transition", "Delivery cannot move backwards.")),
        };
        return result.IsFailure ? Result<Unit>.Failure(result.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
