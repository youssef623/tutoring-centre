using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Domain.Tests.Conversations;

public sealed class ConversationsTests
{
    private static readonly Guid Centre = Guid.NewGuid();
    private static readonly Guid Account = Guid.NewGuid();
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("١٢")]
    public void Channel_RejectsInvalidExternalId(string value) =>
        Invalid(ChannelAccount.Register(Centre, ChannelProvider.Whatsapp, value, PhoneNumber.Create("+201000000091").Value).Error);

    [Fact]
    public void Channel_EnforcesBoundsAndImmutableIdentity()
    {
        var phone = PhoneNumber.Create("+201000000091").Value;
        Invalid(ChannelAccount.Register(Centre, ChannelProvider.Whatsapp, new string('1', 65), phone).Error);
        Invalid(ChannelAccount.Register(Guid.Empty, ChannelProvider.Whatsapp, "1", phone).Error);
        Invalid(ChannelAccount.Register(Centre, (ChannelProvider)99, "1", phone).Error);
        var account = ChannelAccount.Register(Centre, ChannelProvider.Whatsapp, new string('1', 64), phone).Value;
        Assert.Equal("active", account.Status);
        account.Disable();
        account.Disable();
        Assert.Equal("disabled", account.Status);
        Assert.Equal(new string('1', 64), account.ExternalAccountId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sender")]
    [InlineData("123456789012345678901")]
    public void Conversation_RejectsInvalidAddress(string value) => Invalid(Conversation.Open(Centre, Account, value, null, Now).Error);

    [Fact]
    public void Conversation_ResolvesAndClearsParent()
    {
        var conversation = Conversation.Open(Centre, Account, "1", null, Now).Value;
        Assert.Equal(IdentityStatus.Unresolved, conversation.IdentityStatus);
        Assert.Null(conversation.IdentityResolvedAt);
        Invalid(conversation.ResolveToParent(Guid.Empty, Now).Error);
        var parent = Guid.NewGuid();
        Assert.True(conversation.ResolveToParent(parent, Now).IsSuccess);
        Assert.Equal(parent, conversation.ParentId);
        Assert.Equal(IdentityStatus.Parent, conversation.IdentityStatus);
        conversation.MarkUnknown(Now.AddMinutes(1));
        Assert.Null(conversation.ParentId);
        Assert.Equal(IdentityStatus.Unknown, conversation.IdentityStatus);
        Assert.Equal(Now.AddMinutes(1), conversation.IdentityResolvedAt);
        Assert.Equal(Now, conversation.LastMessageAt);
        Invalid(Conversation.Open(Guid.Empty, Account, "1", null, Now).Error);
        Invalid(Conversation.Open(Centre, Guid.Empty, "1", null, Now).Error);
    }

    [Theory]
    [InlineData(InboundMessageType.Text, null)]
    [InlineData(InboundMessageType.Text, "")]
    [InlineData(InboundMessageType.Audio, "text")]
    [InlineData(InboundMessageType.Image, "text")]
    [InlineData(InboundMessageType.Other, "text")]
    public void Message_RejectsInvalidBody(InboundMessageType type, string? text) => Invalid(Receive(type, text).Error);

    [Fact]
    public void Message_CountsCharactersAndEnforcesLength()
    {
        Assert.True(Receive(InboundMessageType.Text, string.Concat(Enumerable.Repeat("😀", 4096))).IsSuccess);
        Invalid(Receive(InboundMessageType.Text, new string('x', 4097)).Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("id\n")]
    [InlineData("é")]
    public void Message_RejectsInvalidProviderId(string id) => Invalid(Receive(InboundMessageType.Text, "hello", id).Error);

    [Fact]
    public void Message_EnforcesProviderIdBoundsAndReferences()
    {
        Assert.True(Receive(InboundMessageType.Text, "hello", new string('~', 128)).IsSuccess);
        Invalid(Receive(InboundMessageType.Text, "hello", new string('a', 129)).Error);
        Invalid(InboundMessage.Receive(Guid.Empty, Account, ChannelProvider.Whatsapp, "id", InboundMessageType.Text, "hello", Now, Now).Error);
        Invalid(InboundMessage.Receive(Centre, Guid.Empty, ChannelProvider.Whatsapp, "id", InboundMessageType.Text, "hello", Now, Now).Error);
        Invalid(InboundMessage.Receive(Centre, Account, (ChannelProvider)99, "id", InboundMessageType.Text, "hello", Now, Now).Error);
        Invalid(Receive((InboundMessageType)99, null).Error);
    }

    [Fact]
    public void Message_ProcessesOnlyOnceAndKeepsReceivedValues()
    {
        var message = Receive(InboundMessageType.Text, "hello").Value;
        Assert.Equal(InboundMessageStatus.Received, message.Status);
        Assert.Null(message.ProcessedAt);
        Assert.True(message.MarkProcessed(Now.AddMinutes(1)).IsSuccess);
        var again = message.MarkProcessed(Now.AddMinutes(2));
        Assert.Equal("conversations.message_already_processed", again.Error!.Code);
        Assert.Equal(ErrorKind.Rule, again.Error.Kind);
        Assert.Equal(Now.AddMinutes(1), message.ProcessedAt);
        Assert.Equal("hello", message.TextBody);
        Assert.Equal(Now, message.ProviderTimestamp);
        Assert.Equal(Now, message.ReceivedAt);
    }

    private static Result<InboundMessage> Receive(InboundMessageType type, string? text, string id = "id") =>
        InboundMessage.Receive(Centre, Account, ChannelProvider.Whatsapp, id, type, text, Now, Now);

    private static void Invalid(Error? error)
    {
        Assert.NotNull(error);
        Assert.Equal("conversations.message_invalid", error.Code);
        Assert.Equal(ErrorKind.Validation, error.Kind);
    }
}
