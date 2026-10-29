using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class RecordInboundMessageHandler(IChannelAccountRepository accounts, IInboundMessageStore store,
    IOutbox outbox, ICurrentActor actor, IClock clock) : ICommandHandler<RecordInboundMessageCommand, RecordInboundMessageResult>
{
    public async Task<Result<RecordInboundMessageResult>> HandleAsync(RecordInboundMessageCommand command, CancellationToken cancellationToken)
    {
        var account = await accounts.FindAsync(command.ChannelAccountId, cancellationToken);
        if (account is null || account.Status != "active")
            return Result<RecordInboundMessageResult>.Failure(Error.NotFound("conversations.not_found", "Channel account not found."));

        var now = clock.UtcNow;
        var phone = PhoneNumber.Create("+" + command.CounterpartAddress);
        var conversation = Conversation.Open(actor.Actor.CentreId!.Value, account.Id, command.CounterpartAddress,
            phone.IsSuccess ? phone.Value : null, command.ProviderTimestamp);
        if (conversation.IsFailure) return Result<RecordInboundMessageResult>.Failure(conversation.Error!);
        var message = InboundMessage.Receive(actor.Actor.CentreId.Value, conversation.Value.Id, account.Provider,
            command.ProviderMessageId, command.Type, command.Text, command.ProviderTimestamp, now);
        if (message.IsFailure) return Result<RecordInboundMessageResult>.Failure(message.Error!);

        var conversationId = await store.UpsertConversationAsync(conversation.Value, cancellationToken);
        var messageId = await store.TryInsertMessageAsync(message.Value, conversationId, cancellationToken);
        if (messageId is null) return Result<RecordInboundMessageResult>.Success(new RecordInboundMessageResult.Duplicate());
        outbox.Enqueue(new InboundMessageReceived(messageId.Value));
        return Result<RecordInboundMessageResult>.Success(new RecordInboundMessageResult.Recorded(messageId.Value));
    }
}
