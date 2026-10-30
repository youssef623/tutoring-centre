using TutoringCentre.Application.Common.Messaging;

namespace TutoringCentre.Application.Conversations;

public sealed record InboundMessageReceived(Guid MessageId) : IOutboxMessage
{
    public static string TypeName => "conversations.inbound_message_received";
}
