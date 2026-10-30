using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Tests.Fakes;

internal sealed class FakeProviderSender : IChannelSender
{
    public ChannelProvider Provider { get; set; } = ChannelProvider.Whatsapp;
    public SendOutcome Outcome { get; set; } = new SendOutcome.Success("fake-provider-id");
    public int Calls { get; private set; }
    public Guid? LastMessageId { get; private set; }
    public Guid? LastChannelId { get; private set; }

    public bool Supports(ChannelProvider provider) => provider == Provider;

    public Task<SendOutcome> SendAsync(OutboundMessage message, ChannelAccount account, CancellationToken cancellationToken)
    {
        Calls++;
        LastMessageId = message.Id;
        LastChannelId = account.Id;
        return Task.FromResult(Outcome);
    }
}
