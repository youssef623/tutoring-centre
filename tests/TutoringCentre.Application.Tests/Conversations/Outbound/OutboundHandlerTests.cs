using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Application.Conversations.Outbound.Commands.RecordDeliveryStatus;
using TutoringCentre.Application.Conversations.Outbound.Commands.SendOutboundMessage;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Tests.Conversations.Outbound;

public sealed class OutboundHandlerTests
{
    [Fact]
    public void RegistryResolvesOnlyExplicitlySupportedProviders()
    {
        var sender = new FakeProviderSender();
        var registry = new ChannelSenderRegistry([sender]);
        Assert.Same(sender, registry.For(ChannelProvider.Whatsapp));
        Assert.True(registry.Supports(ChannelProvider.Whatsapp));
        Assert.Null(registry.For((ChannelProvider)999));
        Assert.False(registry.Supports((ChannelProvider)999));
        Assert.Null(new ChannelSenderRegistry([]).For(ChannelProvider.Whatsapp));
    }

    [Fact]
    public async Task SendCallsAdapterOnceAndKeepsOriginalProviderAndSentTimeOnRepeat()
    {
        var f = new HandlerFixture();
        Assert.True((await f.SendAsync()).IsSuccess);
        Assert.Equal(OutboundMessageStatus.Sent, f.Message.Status);
        Assert.Equal("fake-provider-id", f.Message.ProviderMessageId);
        Assert.Equal(f.Clock.UtcNow, f.Message.SentAt);
        Assert.Equal(f.Message.Id, f.Sender.LastMessageId);
        Assert.Equal(f.Account.Id, f.Sender.LastChannelId);
        Assert.True((await f.SendAsync()).IsSuccess);
        Assert.Equal(1, f.Sender.Calls);
        Assert.Equal(1, f.Repository.ChannelReads);
        Assert.Null(f.Message.FailureCode);
    }

    [Theory]
    [InlineData(OutboundMessageStatus.Sent)]
    [InlineData(OutboundMessageStatus.Delivered)]
    [InlineData(OutboundMessageStatus.Read)]
    [InlineData(OutboundMessageStatus.Failed)]
    public async Task NonQueuedMessagesNeverCallTheAdapterOrReadTheChannel(OutboundMessageStatus status)
    {
        var f = new HandlerFixture();
        if (status == OutboundMessageStatus.Failed)
            Assert.True(f.Message.MarkFailed("provider.refused").IsSuccess);
        else
        {
            Assert.True(f.Message.MarkSent(f.Clock.UtcNow, "already-sent").IsSuccess);
            if (status is OutboundMessageStatus.Delivered or OutboundMessageStatus.Read)
                Assert.True(f.Message.MarkDelivered().IsSuccess);
            if (status == OutboundMessageStatus.Read) Assert.True(f.Message.MarkRead().IsSuccess);
        }
        Assert.True((await f.SendAsync()).IsSuccess);
        Assert.Equal(status, f.Message.Status);
        Assert.Equal(0, f.Sender.Calls);
        Assert.Equal(0, f.Repository.ChannelReads);
    }

    [Fact]
    public async Task PermanentFailureIsTerminalAndRepeatedSendDoesNotRetry()
    {
        var f = new HandlerFixture();
        f.Sender.Outcome = new SendOutcome.Failure("provider.rejected", Retryable: false);
        Assert.True((await f.SendAsync()).IsSuccess);
        Assert.Equal(OutboundMessageStatus.Failed, f.Message.Status);
        Assert.Equal("provider.rejected", f.Message.FailureCode);
        Assert.Null(f.Message.SentAt);
        Assert.Null(f.Message.ProviderMessageId);
        f.Sender.Outcome = new SendOutcome.Success("later-id");
        Assert.True((await f.SendAsync()).IsSuccess);
        Assert.Equal(1, f.Sender.Calls);
        Assert.Equal(OutboundMessageStatus.Failed, f.Message.Status);
    }

    [Fact]
    public async Task RetryableFailureThrowsWithoutMutatingQueuedMessage()
    {
        var f = new HandlerFixture();
        f.Sender.Outcome = new SendOutcome.Failure("provider.timeout", Retryable: true);
        await Assert.ThrowsAsync<InvalidOperationException>(() => f.SendAsync());
        Assert.Equal(OutboundMessageStatus.Queued, f.Message.Status);
        Assert.Null(f.Message.FailureCode);
        Assert.Null(f.Message.SentAt);
        Assert.Null(f.Message.ProviderMessageId);
        f.Sender.Outcome = new SendOutcome.Success();
        Assert.True((await f.SendAsync()).IsSuccess);
        Assert.Equal(OutboundMessageStatus.Sent, f.Message.Status);
        Assert.Equal(2, f.Sender.Calls);
    }

    [Fact]
    public async Task MissingSenderReturnsStableRuleWithoutMutation()
    {
        var f = new HandlerFixture();
        var handler = new SendOutboundMessageHandler(f.Repository, new ChannelSenderRegistry([]), f.Clock);
        var result = await handler.HandleAsync(new(f.Message.Id), CancellationToken.None);
        Assert.Equal("outbound.sender_unavailable", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(OutboundMessageStatus.Queued, f.Message.Status);
        Assert.Equal(0, f.Sender.Calls);
        Assert.Null(f.Message.SentAt);
    }

    [Fact]
    public async Task MissingMessageReturnsNotFoundWithoutReadingChannelOrSending()
    {
        var f = new HandlerFixture();
        f.Repository.Message = null;
        var result = await f.SendAsync();
        Assert.Equal("outbound.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
        Assert.Equal(0, f.Repository.ChannelReads);
        Assert.Equal(0, f.Sender.Calls);
    }

    [Fact]
    public async Task MissingChannelReturnsNotFoundWithoutSendingOrMutating()
    {
        var f = new HandlerFixture();
        f.Repository.Account = null;
        var result = await f.SendAsync();
        Assert.Equal("outbound.channel_not_found", result.Error!.Code);
        Assert.Equal(OutboundMessageStatus.Queued, f.Message.Status);
        Assert.Equal(0, f.Sender.Calls);
    }

    [Fact]
    public async Task DeliveryStatusMovesForwardAndAcceptsRepeatedCallbacks()
    {
        var f = new HandlerFixture();
        Assert.True((await f.StatusAsync(OutboundMessageStatus.Queued)).IsSuccess);
        foreach (var status in new[] { OutboundMessageStatus.Sent, OutboundMessageStatus.Delivered, OutboundMessageStatus.Read })
        {
            Assert.True((await f.StatusAsync(status)).IsSuccess);
            Assert.True((await f.StatusAsync(status)).IsSuccess);
            Assert.Equal(status, f.Message.Status);
        }
        var backwards = await f.StatusAsync(OutboundMessageStatus.Sent);
        Assert.Equal("outbound.invalid_transition", backwards.Error!.Code);
        Assert.Equal(OutboundMessageStatus.Read, f.Message.Status);
        Assert.Equal(f.Clock.UtcNow, f.Message.SentAt);
        Assert.Equal(0, f.Sender.Calls);
    }

    [Theory]
    [InlineData(OutboundMessageStatus.Read)]
    [InlineData(OutboundMessageStatus.Delivered)]
    public async Task DeliveryStatusCannotSkipTheSentState(OutboundMessageStatus target)
    {
        var f = new HandlerFixture();
        var result = await f.StatusAsync(target);
        Assert.Equal("outbound.invalid_transition", result.Error!.Code);
        Assert.Equal(OutboundMessageStatus.Queued, f.Message.Status);
        Assert.Null(f.Message.SentAt);
    }

    [Fact]
    public async Task DeliveryStatusCanFailAndCannotUndoFailure()
    {
        var f = new HandlerFixture();
        Assert.True((await f.StatusAsync(OutboundMessageStatus.Failed)).IsSuccess);
        Assert.True((await f.StatusAsync(OutboundMessageStatus.Failed)).IsSuccess);
        Assert.Equal("provider.failed", f.Message.FailureCode);
        Assert.Equal("outbound.invalid_transition", (await f.StatusAsync(OutboundMessageStatus.Sent)).Error!.Code);
        Assert.Equal("outbound.invalid_transition", (await f.StatusAsync(OutboundMessageStatus.Queued)).Error!.Code);
        Assert.Equal(OutboundMessageStatus.Failed, f.Message.Status);
    }

    [Fact]
    public async Task DeliveryStatusMissingMessageReturnsNotFound()
    {
        var f = new HandlerFixture();
        f.Repository.Message = null;
        var result = await f.StatusAsync(OutboundMessageStatus.Delivered);
        Assert.Equal("outbound.not_found", result.Error!.Code);
        Assert.Equal(0, f.Sender.Calls);
    }

    [Fact]
    public void DeliveryAndSendValidatorsRejectInvalidIdentifiersAndUndefinedStates()
    {
        Assert.False(new SendOutboundMessageValidator().Validate(new SendOutboundMessageCommand(Guid.Empty)).IsValid);
        Assert.False(new RecordDeliveryStatusValidator().Validate(new RecordDeliveryStatusCommand(Guid.Empty, OutboundMessageStatus.Sent)).IsValid);
        Assert.False(new RecordDeliveryStatusValidator().Validate(new RecordDeliveryStatusCommand(Guid.NewGuid(), (OutboundMessageStatus)999)).IsValid);
        Assert.True(new SendOutboundMessageValidator().Validate(new SendOutboundMessageCommand(Guid.NewGuid())).IsValid);
        Assert.True(new RecordDeliveryStatusValidator().Validate(new RecordDeliveryStatusCommand(Guid.NewGuid(), OutboundMessageStatus.Read)).IsValid);
    }

    private sealed class HandlerFixture
    {
        internal FakeClock Clock { get; } = new();
        internal ChannelAccount Account { get; } = ChannelAccount.Register(Guid.NewGuid(), ChannelProvider.Whatsapp, "1", PhoneNumber.Create("+201000000091").Value).Value;
        internal OutboundMessage Message { get; }
        internal FakeProviderSender Sender { get; } = new();
        internal Repository Repository { get; }
        private readonly SendOutboundMessageHandler _send;
        private readonly RecordDeliveryStatusHandler _status;

        internal HandlerFixture()
        {
            Message = OutboundMessage.Queue(Account.CentreId, Guid.NewGuid(), "hello").Value;
            Repository = new Repository { Message = Message, Account = Account };
            _send = new(Repository, new ChannelSenderRegistry([Sender]), Clock);
            _status = new(Repository, Clock);
        }

        internal Task<Result<Unit>> SendAsync() => _send.HandleAsync(new(Message.Id), CancellationToken.None);
        internal Task<Result<Unit>> StatusAsync(OutboundMessageStatus status) => _status.HandleAsync(new(Message.Id, status), CancellationToken.None);
    }

    private sealed class Repository : IOutboundMessageRepository
    {
        internal OutboundMessage? Message { get; set; }
        internal ChannelAccount? Account { get; set; }
        internal int ChannelReads { get; private set; }
        public Task<OutboundMessage?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Message?.Id == id ? Message : null);
        public Task<ChannelAccount?> GetChannelAsync(Guid conversationId, CancellationToken cancellationToken)
        {
            ChannelReads++;
            return Task.FromResult(Account);
        }
        public void Add(OutboundMessage message) => Message = message;
    }
}
