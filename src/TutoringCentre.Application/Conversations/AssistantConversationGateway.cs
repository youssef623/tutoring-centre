using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Application.Conversations.Outbound.Commands.SendOutboundMessage;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
namespace TutoringCentre.Application.Conversations;
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class AssistantConversationGateway(IAgentConversationRepository messages, ChannelSenderRegistry senders,
    IOutboundMessageRepository outbound, IOutbox outbox) : IAssistantConversationGateway
{
    public async Task<AssistantConversationInput?> LoadAsync(Guid inboundId, CancellationToken cancellationToken)
    {
        var inbound = await messages.GetInboundAsync(inboundId,cancellationToken);
        if (inbound is null) return null;
        var conversation = await messages.GetConversationAsync(inbound.ConversationId,cancellationToken);
        return conversation is null ? null : new(conversation.Id,conversation.ParentId,conversation.StudentId,
            conversation.IdentityStatus == IdentityStatus.Identified && senders.Supports(inbound.Provider),inbound.TextBody ?? "", conversation.PortalUserId);
    }
    public async Task<IReadOnlyList<AssistantHistoryEntry>> GetHistoryAsync(Guid inboundId,int count,CancellationToken cancellationToken)
    {
        var inbound = await messages.GetInboundAsync(inboundId,cancellationToken);
        return inbound is null ? [] : await messages.GetHistoryAsync(inbound,count,cancellationToken);
    }
    public async Task<Result> QueueReplyAsync(Guid inboundId,string reply,CancellationToken cancellationToken)
    {
        var inbound = await messages.GetInboundAsync(inboundId,cancellationToken);
        if(inbound is null) return Result.Failure(Error.NotFound("assistant.inbound_not_found","Message not found."));
        var queued = OutboundMessage.Queue(inbound.CentreId,inbound.ConversationId,reply,inbound.Id);
        if(queued.IsFailure) return Result.Failure(queued.Error!);
        outbound.Add(queued.Value);
        outbox.Enqueue(new SendOutboundMessageRequested(queued.Value.Id));
        return Result.Success();
    }
}
