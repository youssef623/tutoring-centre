using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Channels.Web;

namespace TutoringCentre.Infrastructure.Tests.Channels;

/// <summary>Every adapter must identify its channel, honor cancellation and keep stored messages immutable.</summary>
public abstract class ChannelSenderContractTests
{
    protected abstract IChannelSender CreateSender();
    protected abstract ChannelProvider Provider { get; }

    [Fact]
    public void SupportsOnlyTheDeclaredProvider()
    {
        var sender = CreateSender();
        Assert.True(sender.Supports(Provider));
        Assert.False(sender.Supports((ChannelProvider)999));
        Assert.False(sender.Supports(Provider == ChannelProvider.Web ? ChannelProvider.Whatsapp : ChannelProvider.Web));
    }

    [Fact]
    public async Task SuccessfulDeliveryReturnsAnOutcomeWithoutChangingTheStoredMessage()
    {
        var sender = CreateSender();
        var (message, account) = CreatePair();
        Assert.IsType<SendOutcome.Success>(await sender.SendAsync(message, account, CancellationToken.None));
        Assert.Equal(OutboundMessageStatus.Queued, message.Status);
        Assert.Null(message.SentAt);
        Assert.Null(message.FailureCode);
        Assert.Equal("hello", message.Text);
    }

    [Fact]
    public async Task RepeatedDeliveryUsesTheSameStoredIdentityAndRemainsSuccessful()
    {
        var sender = CreateSender();
        var (message, account) = CreatePair();
        var id = message.Id;
        Assert.IsType<SendOutcome.Success>(await sender.SendAsync(message, account, CancellationToken.None));
        Assert.IsType<SendOutcome.Success>(await sender.SendAsync(message, account, CancellationToken.None));
        Assert.Equal(id, message.Id);
        Assert.Equal(OutboundMessageStatus.Queued, message.Status);
    }

    [Fact]
    public async Task CancellationIsObservedBeforeDelivery()
    {
        var sender = CreateSender();
        var (message, account) = CreatePair();
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => sender.SendAsync(message, account, cancellation.Token));
        Assert.Equal(OutboundMessageStatus.Queued, message.Status);
    }

    [Fact]
    public async Task CrossCentreAccountIsRefusedWithoutChangingTheMessage()
    {
        var sender = CreateSender();
        var (message, _) = CreatePair();
        var account = Account(Guid.NewGuid(), Provider);
        var failure = Assert.IsType<SendOutcome.Failure>(await sender.SendAsync(message, account, CancellationToken.None));
        Assert.Equal("channel.mismatch", failure.Code);
        Assert.False(failure.Retryable);
        Assert.Equal(OutboundMessageStatus.Queued, message.Status);
    }

    protected (OutboundMessage Message, ChannelAccount Account) CreatePair()
    {
        var centre = Guid.NewGuid();
        return (OutboundMessage.Queue(centre, Guid.NewGuid(), "hello").Value, Account(centre, Provider));
    }

    protected static ChannelAccount Account(Guid centre, ChannelProvider provider) => provider == ChannelProvider.Web
        ? ChannelAccount.RegisterWeb(centre, "web:nile-centre").Value
        : ChannelAccount.Register(centre, provider, "100000000000001", PhoneNumber.Create("+201000000091").Value).Value;
}

public sealed class WebChannelSenderContractTests : ChannelSenderContractTests
{
    protected override IChannelSender CreateSender() => new WebChannelSender();
    protected override ChannelProvider Provider => ChannelProvider.Web;

    [Fact]
    public async Task AProviderMismatchReturnsAPermanentFailure()
    {
        var sender = new WebChannelSender();
        var (message, _) = CreatePair();
        var whatsapp = Account(message.CentreId, ChannelProvider.Whatsapp);
        var failure = Assert.IsType<SendOutcome.Failure>(await sender.SendAsync(message, whatsapp, CancellationToken.None));
        Assert.Equal("channel.mismatch", failure.Code);
        Assert.False(failure.Retryable);
    }
}

public sealed class FakeProviderSenderContractTests : ChannelSenderContractTests
{
    protected override IChannelSender CreateSender() => new FakeProviderSender();
    protected override ChannelProvider Provider => ChannelProvider.Whatsapp;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ConfiguredFailuresRetainTheirRetryability(bool retryable)
    {
        var sender = new FakeProviderSender { Outcome = new SendOutcome.Failure("provider.refused", retryable) };
        var (message, account) = CreatePair();
        var failure = Assert.IsType<SendOutcome.Failure>(await sender.SendAsync(message, account, CancellationToken.None));
        Assert.Equal("provider.refused", failure.Code);
        Assert.Equal(retryable, failure.Retryable);
        Assert.Equal(1, sender.Calls);
        Assert.Equal(OutboundMessageStatus.Queued, message.Status);
    }

    private sealed class FakeProviderSender : IChannelSender
    {
        internal SendOutcome Outcome { get; init; } = new SendOutcome.Success("fake-provider-id");
        internal int Calls { get; private set; }
        public bool Supports(ChannelProvider provider) => provider == ChannelProvider.Whatsapp;
        public Task<SendOutcome> SendAsync(OutboundMessage message, ChannelAccount account, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!Supports(account.Provider) || message.CentreId != account.CentreId)
                return Task.FromResult<SendOutcome>(new SendOutcome.Failure("channel.mismatch", false));
            Calls++;
            return Task.FromResult(Outcome);
        }
    }
}
