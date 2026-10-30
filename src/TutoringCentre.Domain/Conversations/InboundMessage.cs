using System.Text;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Conversations;

public sealed class InboundMessage : Entity, ITenantOwned
{
    private InboundMessage() { }
    private InboundMessage(Guid centreId, Guid conversationId, ChannelProvider provider, string providerMessageId,
        InboundMessageType type, string? text, DateTimeOffset providerTimestamp, DateTimeOffset receivedAt)
    {
        CentreId = centreId;
        ConversationId = conversationId;
        Provider = provider;
        ProviderMessageId = providerMessageId;
        MessageType = type;
        TextBody = text;
        ProviderTimestamp = providerTimestamp;
        ReceivedAt = receivedAt;
    }

    public Guid CentreId { get; }
    public Guid ConversationId { get; }
    public ChannelProvider Provider { get; }
    public string ProviderMessageId { get; } = null!;
    public InboundMessageType MessageType { get; }
    public string? TextBody { get; }
    public DateTimeOffset ProviderTimestamp { get; }
    public DateTimeOffset ReceivedAt { get; }
    public InboundMessageStatus Status { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }

    public static Result<InboundMessage> Receive(Guid centreId, Guid conversationId, ChannelProvider provider,
        string providerMessageId, InboundMessageType type, string? text, DateTimeOffset providerTimestamp, DateTimeOffset receivedAt)
    {
        if (centreId == Guid.Empty || conversationId == Guid.Empty || !Enum.IsDefined(provider) || !Enum.IsDefined(type)
            || providerMessageId is not { Length: > 0 and <= 128 } || providerMessageId.Any(c => c is < ' ' or > '~')
            || (type == InboundMessageType.Text ? string.IsNullOrEmpty(text) : text is not null)
            || (text is not null && text.EnumerateRunes().Count() > 4096))
            return Result<InboundMessage>.Failure(Conversation.Invalid());
        return Result<InboundMessage>.Success(new InboundMessage(centreId, conversationId, provider, providerMessageId, type, text, providerTimestamp, receivedAt));
    }

    public Result MarkProcessed(DateTimeOffset at)
    {
        if (Status == InboundMessageStatus.Processed)
            return Result.Failure(Error.Rule("conversations.message_already_processed", "The message has already been processed."));
        Status = InboundMessageStatus.Processed;
        ProcessedAt = at;
        return Result.Success();
    }
}
