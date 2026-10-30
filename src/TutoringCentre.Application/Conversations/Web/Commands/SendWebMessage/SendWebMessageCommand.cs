using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Conversations.Web.Commands.SendWebMessage;

public sealed record SendWebMessageCommand(Guid ClientMessageId, string Text) : ICommand<SendWebMessageResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.PortalChat;
}

public sealed record SendWebMessageResult(Guid MessageId, bool AlreadyRecorded);

internal sealed class SendWebMessageValidator : AbstractValidator<SendWebMessageCommand>
{
    public SendWebMessageValidator()
    {
        RuleFor(command => command.ClientMessageId).NotEmpty();
        RuleFor(command => command.Text).Must(text => !string.IsNullOrWhiteSpace(text) && text.Trim().EnumerateRunes().Count() <= 1000);
    }
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class SendWebMessageHandler(ICurrentActor actor, IWebConversationRepository conversations, IInboundMessageStore messages,
    IOutbox outbox, IClock clock) : ICommandHandler<SendWebMessageCommand, SendWebMessageResult>
{
    public async Task<Result<SendWebMessageResult>> HandleAsync(SendWebMessageCommand command, CancellationToken cancellationToken)
    {
        if (actor.Actor is not IPortalActor portal)
            return Result<SendWebMessageResult>.Failure(Error.Forbidden("chat.portal_actor_required", "A portal session is required."));
        var account = await conversations.GetChannelAsync(cancellationToken);
        if (account is null) return Result<SendWebMessageResult>.Failure(Error.Rule("chat.unavailable", "Web chat is unavailable."));
        var subject = new PortalConversationSubject(portal.SubjectKind == PortalSubjectKind.Parent ? PortalConversationSubjectKind.Parent : PortalConversationSubjectKind.Student, portal.SubjectId);
        var now = clock.UtcNow;
        var opened = Conversation.OpenForPortal(portal.CentreId!.Value, account.Id, portal.UserId, subject, now);
        if (opened.IsFailure) return Result<SendWebMessageResult>.Failure(opened.Error!);
        var conversationId = await conversations.GetOrCreateAsync(opened.Value, cancellationToken);
        // The inbound store's unique key spans the centre. Namespace a client ID by
        // the server-resolved conversation so different families cannot collide.
        var providerId = $"{conversationId:N}:{command.ClientMessageId:N}";
        var received = InboundMessage.Receive(portal.CentreId.Value, conversationId, ChannelProvider.Web, providerId,
            InboundMessageType.Text, command.Text.Trim(), now, now);
        if (received.IsFailure) return Result<SendWebMessageResult>.Failure(received.Error!);
        var inserted = await messages.TryInsertMessageAsync(received.Value, conversationId, cancellationToken);
        if (inserted is { } messageId)
        {
            // Reuse the existing Month 4 processing intent; its agent consumer arrives with Day 78.
            outbox.Enqueue(new InboundMessageReceived(messageId));
            return Result<SendWebMessageResult>.Success(new(messageId, false));
        }
        var existingId = await conversations.GetMessageIdAsync(conversationId, providerId, cancellationToken);
        return existingId is { } existing
            ? Result<SendWebMessageResult>.Success(new(existing, true))
            : Result<SendWebMessageResult>.Failure(Error.Rule("chat.record_failed", "The message could not be recorded."));
    }
}
