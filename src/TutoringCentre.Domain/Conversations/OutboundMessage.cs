using System.Text;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Conversations;

public enum OutboundMessageStatus { Queued, Sent, Delivered, Read, Failed }

/// <summary>A stored reply whose delivery state can only move forward.</summary>
public sealed class OutboundMessage : Entity, ITenantOwned
{
    private OutboundMessage() { }
    private OutboundMessage(Guid centreId, Guid conversationId, string text, Guid? replyToInboundId)
    {
        CentreId = centreId;
        ConversationId = conversationId;
        Text = text;
        ReplyToInboundId = replyToInboundId;
    }

    public Guid CentreId { get; }
    public Guid ConversationId { get; }
    public string Text { get; } = null!;
    public Guid? ReplyToInboundId { get; }
    public OutboundMessageStatus Status { get; private set; }
    public string? ProviderMessageId { get; private set; }
    public string? FailureCode { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }

    public static Result<OutboundMessage> Queue(Guid centreId, Guid conversationId, string text, Guid? replyToInboundId = null)
    {
        var trimmed = text?.Trim();
        if (centreId == Guid.Empty || conversationId == Guid.Empty || replyToInboundId == Guid.Empty
            || string.IsNullOrEmpty(trimmed) || trimmed.EnumerateRunes().Count() > 1000)
            return Result<OutboundMessage>.Failure(InvalidMessage());
        return Result<OutboundMessage>.Success(new OutboundMessage(centreId, conversationId, trimmed, replyToInboundId));
    }

    public Result MarkSent(DateTimeOffset at, string? providerMessageId = null)
    {
        if (Status == OutboundMessageStatus.Sent) return Result.Success();
        if (Status != OutboundMessageStatus.Queued) return InvalidTransition();
        if (providerMessageId is not null && (providerMessageId.Length is < 1 or > 128
            || providerMessageId.Any(character => character is < ' ' or > '~')))
            return Result.Failure(InvalidMessage());
        Status = OutboundMessageStatus.Sent;
        ProviderMessageId = providerMessageId;
        SentAt = at;
        return Result.Success();
    }

    public Result MarkDelivered()
    {
        if (Status == OutboundMessageStatus.Delivered) return Result.Success();
        if (Status != OutboundMessageStatus.Sent) return InvalidTransition();
        Status = OutboundMessageStatus.Delivered;
        return Result.Success();
    }

    public Result MarkRead()
    {
        if (Status == OutboundMessageStatus.Read) return Result.Success();
        if (Status != OutboundMessageStatus.Delivered) return InvalidTransition();
        Status = OutboundMessageStatus.Read;
        return Result.Success();
    }

    public Result MarkFailed(string reasonCode)
    {
        if (Status == OutboundMessageStatus.Failed) return Result.Success();
        if (Status is not (OutboundMessageStatus.Queued or OutboundMessageStatus.Sent)) return InvalidTransition();
        if (reasonCode is not { Length: > 0 and <= 60 }
            || reasonCode.Any(character => character is not (>= 'a' and <= 'z' or >= '0' and <= '9' or '.' or '_' or '-')))
            return Result.Failure(InvalidMessage());
        Status = OutboundMessageStatus.Failed;
        FailureCode = reasonCode;
        return Result.Success();
    }

    private static Error InvalidMessage() => Error.Validation("outbound.message_invalid", "The outbound message or delivery data is invalid.");
    private static Result InvalidTransition() => Result.Failure(Error.Rule("outbound.invalid_transition", "The delivery state cannot move in that direction."));
}
