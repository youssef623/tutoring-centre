using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations.Outbound;

/// <summary>Adapters use the outbound ID as their idempotency key. Message text must never be logged.</summary>
public interface IChannelSender
{
    bool Supports(ChannelProvider provider);
    Task<SendOutcome> SendAsync(OutboundMessage message, ChannelAccount account, CancellationToken cancellationToken);
}

public abstract record SendOutcome
{
    private SendOutcome() { }
    public sealed record Success(string? ProviderId = null) : SendOutcome;
    public sealed record Failure(string Code, bool Retryable) : SendOutcome;
}

public sealed class ChannelSenderRegistry(IEnumerable<IChannelSender> senders)
{
    public IChannelSender? For(ChannelProvider provider) => senders.SingleOrDefault(sender => sender.Supports(provider));
    public bool Supports(ChannelProvider provider) => For(provider) is not null;
}
