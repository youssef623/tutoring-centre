using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Application.Conversations.Web;
using TutoringCentre.Application.Conversations.Web.Commands.SendWebMessage;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Conversations.Web;

public sealed class SendWebMessageHandlerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PortalSend_DerivesConversationIdentityFromActorAndTrimsText(bool student)
    {
        var fixture = new HandlerFixture(student);
        var result = await fixture.SendAsync("  hello portal  ");
        Assert.True(result.IsSuccess);
        Assert.Equal(fixture.Store.NewId, result.Value.MessageId);
        Assert.False(result.Value.AlreadyRecorded);
        var conversation = Assert.IsType<Conversation>(fixture.Repository.Conversation);
        Assert.Equal(fixture.CentreId, conversation.CentreId);
        Assert.Equal(fixture.UserId, conversation.PortalUserId);
        Assert.Equal(student ? null : (Guid?)fixture.SubjectId, conversation.ParentId);
        Assert.Equal(student ? fixture.SubjectId : (Guid?)null, conversation.StudentId);
        Assert.Equal(fixture.Repository.Account!.Id, conversation.ChannelAccountId);
        Assert.Equal(IdentityStatus.Identified, conversation.IdentityStatus);
        Assert.Equal(fixture.Clock.UtcNow, conversation.IdentityResolvedAt);
        Assert.Null(conversation.CounterpartAddress);
        Assert.Null(conversation.CounterpartPhone);
        var message = Assert.IsType<InboundMessage>(fixture.Store.Message);
        Assert.Equal("hello portal", message.TextBody);
        Assert.Equal(ChannelProvider.Web, message.Provider);
        Assert.Equal(fixture.ClientId.ToString("D"), message.ProviderMessageId);
        Assert.Equal(fixture.Repository.ConversationId, message.ConversationId);
        Assert.Equal(fixture.Repository.ConversationId, fixture.Store.InsertConversationId);
        Assert.Equal(fixture.Clock.UtcNow, message.ReceivedAt);
        Assert.Equal(fixture.Clock.UtcNow, message.ProviderTimestamp);
        Assert.Equal(result.Value.MessageId, Assert.IsType<InboundMessageReceived>(Assert.Single(fixture.Outbox.Messages)).MessageId);
        Assert.Equal(0, fixture.Repository.ExistingMessageReads);
    }

    [Fact]
    public async Task Replay_ReturnsExistingIdOnlyWithinActorsConversationWithoutAnotherIntent()
    {
        var fixture = new HandlerFixture();
        fixture.Store.NewId = null;
        fixture.Repository.ExistingMessageId = Guid.NewGuid();
        var result = await fixture.SendAsync();
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.AlreadyRecorded);
        Assert.Equal(fixture.Repository.ExistingMessageId, result.Value.MessageId);
        Assert.Equal(fixture.Repository.ConversationId, fixture.Repository.LookedUpConversationId);
        Assert.Equal(fixture.ClientId.ToString("D"), fixture.Repository.LookedUpProviderId);
        Assert.Empty(fixture.Outbox.Messages);
    }

    [Fact]
    public async Task CollisionOutsideActorsConversation_ReturnsConflictWithoutIntentOrMessageId()
    {
        var fixture = new HandlerFixture();
        fixture.Store.NewId = null;
        fixture.Repository.ExistingMessageId = null;
        var result = await fixture.SendAsync();
        Assert.True(result.IsFailure);
        Assert.Equal("chat.client_message_conflict", result.Error?.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error?.Kind);
        Assert.Empty(fixture.Outbox.Messages);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NonPortalActor_IsRefusedBeforeReadingChannel(bool system)
    {
        var fixture = new HandlerFixture(nonPortal: system ? new SystemActor(Guid.NewGuid()) : new StaffActor(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Owner));
        var result = await fixture.SendAsync();
        Assert.Equal("chat.portal_actor_required", result.Error?.Code);
        Assert.Equal(0, fixture.Repository.ChannelReads);
        Assert.Null(fixture.Repository.Conversation);
        Assert.Null(fixture.Store.Message);
        Assert.Empty(fixture.Outbox.Messages);
    }

    [Fact]
    public async Task MissingWebChannel_ReturnsUnavailableWithoutOpeningConversation()
    {
        var fixture = new HandlerFixture();
        fixture.Repository.Account = null;
        var result = await fixture.SendAsync();
        Assert.Equal("chat.unavailable", result.Error?.Code);
        Assert.Equal(1, fixture.Repository.ChannelReads);
        Assert.Null(fixture.Repository.Conversation);
        Assert.Null(fixture.Store.Message);
        Assert.Empty(fixture.Outbox.Messages);
    }

    [Fact]
    public void Validator_RejectsEmptyClientId()
    {
        var result = new SendWebMessageValidator().Validate(new SendWebMessageCommand(Guid.Empty, "hello"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SendWebMessageCommand.ClientMessageId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\n ")]
    public void Validator_RejectsMissingOrWhitespaceText(string? text) =>
        Assert.False(new SendWebMessageValidator().Validate(new SendWebMessageCommand(Guid.NewGuid(), text!)).IsValid);

    [Fact]
    public void Validator_CountsTrimmedUnicodeRunesRatherThanUtf16Units()
    {
        var validator = new SendWebMessageValidator();
        var rune = char.ConvertFromUtf32(0x1F600);
        var thousand = string.Concat(Enumerable.Repeat(rune, 1000));
        Assert.True(validator.Validate(new SendWebMessageCommand(Guid.NewGuid(), $" \t{thousand}\n ")).IsValid);
        Assert.False(validator.Validate(new SendWebMessageCommand(Guid.NewGuid(), thousand + rune)).IsValid);
        Assert.False(validator.Validate(new SendWebMessageCommand(Guid.NewGuid(), new string('x', 1001))).IsValid);
    }

    private sealed class HandlerFixture
    {
        internal Guid CentreId { get; } = Guid.NewGuid();
        internal Guid UserId { get; } = Guid.NewGuid();
        internal Guid SubjectId { get; } = Guid.NewGuid();
        internal Guid ClientId { get; } = Guid.NewGuid();
        internal FakeClock Clock { get; } = new();
        internal WebRepository Repository { get; }
        internal MessageStore Store { get; } = new();
        internal TestOutbox Outbox { get; } = new();
        private readonly SendWebMessageHandler _handler;

        internal HandlerFixture(bool student = false, Actor? nonPortal = null)
        {
            Repository = new WebRepository { Account = ChannelAccount.RegisterWeb(CentreId, "web:nile-centre").Value };
            var actor = new CurrentActorContext();
            Actor portal = student ? new StudentActor(UserId, CentreId, SubjectId) : new ParentActor(UserId, CentreId, SubjectId);
            actor.Set(nonPortal ?? portal);
            _handler = new(actor, Repository, Store, Outbox, Clock);
        }

        internal Task<Result<SendWebMessageResult>> SendAsync(string text = "hello") =>
            _handler.HandleAsync(new(ClientId, text), CancellationToken.None);
    }

    private sealed class WebRepository : IWebConversationRepository
    {
        internal ChannelAccount? Account { get; set; }
        internal Conversation? Conversation { get; private set; }
        internal Guid ConversationId { get; } = Guid.NewGuid();
        internal Guid? ExistingMessageId { get; set; }
        internal Guid LookedUpConversationId { get; private set; }
        internal string? LookedUpProviderId { get; private set; }
        internal int ChannelReads { get; private set; }
        internal int ExistingMessageReads { get; private set; }

        public Task<ChannelAccount?> GetChannelAsync(CancellationToken cancellationToken)
        {
            ChannelReads++;
            return Task.FromResult(Account);
        }

        public Task<Guid> GetOrCreateAsync(Conversation conversation, CancellationToken cancellationToken)
        {
            Conversation = conversation;
            return Task.FromResult(ConversationId);
        }

        public Task<Guid?> GetMessageIdAsync(Guid conversationId, string providerMessageId, CancellationToken cancellationToken)
        {
            ExistingMessageReads++;
            LookedUpConversationId = conversationId;
            LookedUpProviderId = providerMessageId;
            return Task.FromResult(ExistingMessageId);
        }
    }

    private sealed class MessageStore : IInboundMessageStore
    {
        internal Guid? NewId { get; set; } = Guid.NewGuid();
        internal InboundMessage? Message { get; private set; }
        internal Guid InsertConversationId { get; private set; }

        public Task<Guid> UpsertConversationAsync(Conversation conversation, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Web messages must use the web conversation repository.");

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
