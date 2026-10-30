using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Tests.Conversations;

public sealed class RecordInboundMessageHandlerTests
{
    [Fact]
    public async Task NewMessage_StoresCandidatesAndEnqueuesReturnedId()
    {
        var f = new HandlerFixture();
        var result = await f.Send();
        Assert.Equal(f.Store.NewId, Assert.IsType<RecordInboundMessageResult.Recorded>(result.Value).MessageId);
        Assert.Equal(f.Store.NewId, Assert.IsType<InboundMessageReceived>(Assert.Single(f.Outbox.Messages)).MessageId);
        Assert.Equal(f.Store.ConversationId, f.Store.InsertConversationId);
        Assert.Equal(f.Account.Id, f.Store.Conversation!.ChannelAccountId);
        Assert.Equal("+201000000091", f.Store.Conversation.CounterpartPhone!.Value);
        Assert.Equal(f.Clock.UtcNow, f.Store.Message!.ReceivedAt);
    }

    [Fact]
    public async Task Duplicate_DoesNotEnqueue()
    {
        var f = new HandlerFixture();
        f.Store.NewId = null;
        Assert.IsType<RecordInboundMessageResult.Duplicate>((await f.Send()).Value);
        Assert.Empty(f.Outbox.Messages);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task InvalidText_NoStoreOrOutbox(string? text)
    {
        var f = new HandlerFixture();
        var result = await f.Send(text: text);
        Assert.Equal("conversations.message_invalid", result.Error!.Code);
        Assert.Null(f.Store.Conversation);
        Assert.Null(f.Store.Message);
        Assert.Empty(f.Outbox.Messages);
    }

    [Fact]
    public async Task NonPhoneAddress_HasNullPhone()
    {
        var f = new HandlerFixture();
        Assert.True((await f.Send(address: "1")).IsSuccess);
        Assert.Null(f.Store.Conversation!.CounterpartPhone);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MissingOrDisabledAccount_ReturnsNotFound(bool disabled)
    {
        var f = new HandlerFixture();
        if (disabled) f.Account.Disable(); else f.Repository.Account = null;
        var result = await f.Send();
        Assert.Equal("conversations.not_found", result.Error!.Code);
        Assert.Null(f.Store.Conversation);
        Assert.Empty(f.Outbox.Messages);
    }

    [Theory]
    [InlineData("", "id")]
    [InlineData("abc", "id")]
    [InlineData("123456789012345678901", "id")]
    [InlineData("1", "")]
    [InlineData("1", "bad\n")]
    public void Validator_RejectsMalformedIdentifiers(string address, string id)
    {
        var validator = new RecordInboundMessageValidator();
        Assert.False(validator.Validate(new RecordInboundMessageCommand(Guid.NewGuid(), address, id, InboundMessageType.Text, "hello", DateTimeOffset.UtcNow)).IsValid);
    }

    private sealed class HandlerFixture
    {
        internal FakeClock Clock { get; } = new();
        internal ChannelAccount Account { get; } = ChannelAccount.Register(Guid.NewGuid(), ChannelProvider.Whatsapp, "1", PhoneNumber.Create("+201000000091").Value).Value;
        internal AccountRepository Repository { get; }
        internal MessageStore Store { get; } = new();
        internal TestOutbox Outbox { get; } = new();
        private readonly RecordInboundMessageHandler handler;

        internal HandlerFixture()
        {
            Repository = new AccountRepository { Account = Account };
            var actor = new CurrentActorContext();
            actor.Set(new SystemActor(Account.CentreId));
            handler = new(Repository, Store, Outbox, actor, Clock);
        }

        internal Task<Result<RecordInboundMessageResult>> Send(string address = "201000000091", string? text = "hello") =>
            handler.HandleAsync(new(Account.Id, address, "provider-id", InboundMessageType.Text, text, Clock.UtcNow), CancellationToken.None);
    }

    private sealed class AccountRepository : IChannelAccountRepository
    {
        internal ChannelAccount? Account { get; set; }
        public Task<ChannelAccount?> FindAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Account);
        public void Add(ChannelAccount account) => throw new NotSupportedException();
    }

    private sealed class MessageStore : IInboundMessageStore
    {
        internal Guid? NewId { get; set; } = Guid.NewGuid();
        internal Guid ConversationId { get; } = Guid.NewGuid();
        internal Guid InsertConversationId { get; private set; }
        internal Conversation? Conversation { get; private set; }
        internal InboundMessage? Message { get; private set; }
        public Task<Guid> UpsertConversationAsync(Conversation conversation, CancellationToken cancellationToken)
        {
            Conversation = conversation;
            return Task.FromResult(ConversationId);
        }
        public Task<Guid?> TryInsertMessageAsync(InboundMessage message, Guid conversationId, CancellationToken cancellationToken)
        {
            Message = message;
            InsertConversationId = conversationId;
            return Task.FromResult(NewId);
        }
    }

    private sealed class TestOutbox : IOutbox
    {
        internal List<object> Messages { get; } = [];
        public void Enqueue<TMessage>(TMessage message) where TMessage : class, IOutboxMessage => Messages.Add(message);
    }
}
