using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;

public sealed record RecordInboundMessageCommand(Guid ChannelAccountId, string CounterpartAddress, string ProviderMessageId,
    InboundMessageType Type, string? Text, DateTimeOffset ProviderTimestamp)
    : ICommand<RecordInboundMessageResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ConversationsIngest;
}

public abstract record RecordInboundMessageResult
{
    private RecordInboundMessageResult() { }
    public sealed record Recorded(Guid MessageId) : RecordInboundMessageResult;
    public sealed record Duplicate : RecordInboundMessageResult;
}
