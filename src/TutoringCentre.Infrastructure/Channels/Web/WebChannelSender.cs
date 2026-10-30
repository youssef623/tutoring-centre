using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Infrastructure.Channels.Web;

/// <summary>Polling reads the stored message; no external delivery side effect is needed.</summary>
public sealed class WebChannelSender : IChannelSender
{
    public bool Supports(ChannelProvider provider) => provider == ChannelProvider.Web;
    public Task<SendOutcome> SendAsync(OutboundMessage message, ChannelAccount account, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(account);
        cancellationToken.ThrowIfCancellationRequested();
        if (!Supports(account.Provider) || message.CentreId != account.CentreId)
            return Task.FromResult<SendOutcome>(new SendOutcome.Failure("channel.mismatch", false));
        return Task.FromResult<SendOutcome>(new SendOutcome.Success());
    }
}
