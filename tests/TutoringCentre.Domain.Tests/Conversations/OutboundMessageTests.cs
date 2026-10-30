using System.Reflection;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Domain.Tests.Conversations;

public sealed class OutboundMessageTests
{
    private static readonly Guid Centre = Guid.NewGuid();
    private static readonly Guid Conversation = Guid.NewGuid();
    private static readonly DateTimeOffset SentAt = new(2027, 3, 1, 8, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(OutboundMessageStatus.Queued, OutboundMessageStatus.Sent, true)]
    [InlineData(OutboundMessageStatus.Queued, OutboundMessageStatus.Delivered, false)]
    [InlineData(OutboundMessageStatus.Queued, OutboundMessageStatus.Read, false)]
    [InlineData(OutboundMessageStatus.Queued, OutboundMessageStatus.Failed, true)]
    [InlineData(OutboundMessageStatus.Sent, OutboundMessageStatus.Sent, true)]
    [InlineData(OutboundMessageStatus.Sent, OutboundMessageStatus.Delivered, true)]
    [InlineData(OutboundMessageStatus.Sent, OutboundMessageStatus.Read, false)]
    [InlineData(OutboundMessageStatus.Sent, OutboundMessageStatus.Failed, true)]
    [InlineData(OutboundMessageStatus.Delivered, OutboundMessageStatus.Sent, false)]
    [InlineData(OutboundMessageStatus.Delivered, OutboundMessageStatus.Delivered, true)]
    [InlineData(OutboundMessageStatus.Delivered, OutboundMessageStatus.Read, true)]
    [InlineData(OutboundMessageStatus.Delivered, OutboundMessageStatus.Failed, false)]
    [InlineData(OutboundMessageStatus.Read, OutboundMessageStatus.Sent, false)]
    [InlineData(OutboundMessageStatus.Read, OutboundMessageStatus.Delivered, false)]
    [InlineData(OutboundMessageStatus.Read, OutboundMessageStatus.Read, true)]
    [InlineData(OutboundMessageStatus.Read, OutboundMessageStatus.Failed, false)]
    [InlineData(OutboundMessageStatus.Failed, OutboundMessageStatus.Sent, false)]
    [InlineData(OutboundMessageStatus.Failed, OutboundMessageStatus.Delivered, false)]
    [InlineData(OutboundMessageStatus.Failed, OutboundMessageStatus.Read, false)]
    [InlineData(OutboundMessageStatus.Failed, OutboundMessageStatus.Failed, true)]
    public void DeliveryTransitionMatrix(OutboundMessageStatus source, OutboundMessageStatus target, bool allowed)
    {
        var message = At(source);
        var result = Move(message, target);
        Assert.Equal(allowed, result.IsSuccess);
        Assert.Equal(allowed ? target : source, message.Status);
        if (!allowed)
        {
            Assert.Equal("outbound.invalid_transition", result.Error!.Code);
            Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        }
    }

    [Theory]
    [InlineData(OutboundMessageStatus.Queued)]
    [InlineData(OutboundMessageStatus.Sent)]
    [InlineData(OutboundMessageStatus.Delivered)]
    [InlineData(OutboundMessageStatus.Read)]
    [InlineData(OutboundMessageStatus.Failed)]
    public void QueuedIsOnlyAnInitialStateAndHasNoMutationEntryPoint(OutboundMessageStatus source)
    {
        var message = At(source);
        Assert.Null(typeof(OutboundMessage).GetMethod("MarkQueued", BindingFlags.Public | BindingFlags.Instance));
        Assert.Null(typeof(OutboundMessage).GetProperty(nameof(OutboundMessage.Status))!.GetSetMethod());
        Assert.Equal(source, message.Status);
    }

    [Fact]
    public void QueueTrimsTextAndKeepsImmutableIdentifiers()
    {
        var reply = Guid.NewGuid();
        var message = OutboundMessage.Queue(Centre, Conversation, "  hello \n", reply).Value;
        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.Equal(Centre, message.CentreId);
        Assert.Equal(Conversation, message.ConversationId);
        Assert.Equal(reply, message.ReplyToInboundId);
        Assert.Equal("hello", message.Text);
        Assert.Equal(OutboundMessageStatus.Queued, message.Status);
        Assert.Null(message.ProviderMessageId);
        Assert.Null(message.SentAt);
        Assert.Null(message.FailureCode);
        foreach (var name in new[] { nameof(OutboundMessage.CentreId), nameof(OutboundMessage.ConversationId), nameof(OutboundMessage.Text), nameof(OutboundMessage.ReplyToInboundId) })
            Assert.Null(typeof(OutboundMessage).GetProperty(name)!.SetMethod);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\r\n ")]
    public void QueueRejectsMissingText(string? text) => Invalid(OutboundMessage.Queue(Centre, Conversation, text!).Error);

    [Fact]
    public void QueueCountsUnicodeCharactersAfterTrimming()
    {
        var thousand = string.Concat(Enumerable.Repeat("\U0001F600", 1000));
        Assert.Equal(thousand, OutboundMessage.Queue(Centre, Conversation, $" {thousand} ").Value.Text);
        Assert.True(OutboundMessage.Queue(Centre, Conversation, "x").IsSuccess);
        Invalid(OutboundMessage.Queue(Centre, Conversation, thousand + "x").Error);
        Invalid(OutboundMessage.Queue(Centre, Conversation, new string('x', 1001)).Error);
    }

    [Fact]
    public void QueueRejectsEmptyReferences()
    {
        Invalid(OutboundMessage.Queue(Guid.Empty, Conversation, "hello").Error);
        Invalid(OutboundMessage.Queue(Centre, Guid.Empty, "hello").Error);
        Invalid(OutboundMessage.Queue(Centre, Conversation, "hello", Guid.Empty).Error);
        Assert.True(OutboundMessage.Queue(Centre, Conversation, "hello").IsSuccess);
    }

    [Fact]
    public void RepeatedSentDoesNotReplaceProviderIdOrTimestampOrValidateNewData()
    {
        var message = At(OutboundMessageStatus.Sent);
        Assert.True(message.MarkSent(SentAt.AddHours(1), "invalid\n").IsSuccess);
        Assert.Equal("provider-1", message.ProviderMessageId);
        Assert.Equal(SentAt, message.SentAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("bad\n")]
    [InlineData("\u007f")]
    [InlineData("\u00e9")]
    public void SentRejectsInvalidProviderIdWithoutMoving(string providerId)
    {
        var message = At(OutboundMessageStatus.Queued);
        Invalid(message.MarkSent(SentAt, providerId).Error);
        Assert.Equal(OutboundMessageStatus.Queued, message.Status);
        Assert.Null(message.SentAt);
        Assert.Null(message.ProviderMessageId);
    }

    [Fact]
    public void SentEnforcesProviderIdBoundsAndAllowsMissingId()
    {
        var message = At(OutboundMessageStatus.Queued);
        Invalid(message.MarkSent(SentAt, new string('x', 129)).Error);
        Assert.True(message.MarkSent(SentAt, new string('~', 128)).IsSuccess);
        Assert.Equal(new string('~', 128), message.ProviderMessageId);
        var withoutId = At(OutboundMessageStatus.Queued);
        Assert.True(withoutId.MarkSent(SentAt).IsSuccess);
        Assert.Null(withoutId.ProviderMessageId);
        Assert.Equal(SentAt, withoutId.SentAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not a code")]
    [InlineData("Uppercase")]
    [InlineData("bad\n")]
    [InlineData("\u00e9")]
    public void FailedRejectsInvalidReasonWithoutMoving(string? reason)
    {
        var message = At(OutboundMessageStatus.Queued);
        Invalid(message.MarkFailed(reason!).Error);
        Assert.Equal(OutboundMessageStatus.Queued, message.Status);
        Assert.Null(message.FailureCode);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FailedKeepsOriginalDeliveryFactsAndIsTerminal(bool sent)
    {
        var message = At(sent ? OutboundMessageStatus.Sent : OutboundMessageStatus.Queued);
        Assert.True(message.MarkFailed("provider.timeout_1-retry").IsSuccess);
        Assert.Equal("provider.timeout_1-retry", message.FailureCode);
        Assert.Equal(sent ? SentAt : (DateTimeOffset?)null, message.SentAt);
        Assert.Equal(sent ? "provider-1" : null, message.ProviderMessageId);
        Assert.True(message.MarkFailed(null!).IsSuccess);
        Assert.Equal("provider.timeout_1-retry", message.FailureCode);
        Assert.Equal("outbound.invalid_transition", message.MarkSent(SentAt).Error!.Code);
        Assert.Equal(OutboundMessageStatus.Failed, message.Status);
    }

    [Fact]
    public void FailedEnforcesReasonLengthBounds()
    {
        var message = At(OutboundMessageStatus.Queued);
        Invalid(message.MarkFailed(new string('x', 61)).Error);
        Assert.True(message.MarkFailed(new string('x', 60)).IsSuccess);
        Assert.Equal(new string('x', 60), message.FailureCode);
        Assert.True(At(OutboundMessageStatus.Queued).MarkFailed("x").IsSuccess);
    }

    [Fact]
    public void DeliveryProgressKeepsSentTimestampAndProviderIdentifier()
    {
        var message = At(OutboundMessageStatus.Sent);
        Assert.True(message.MarkDelivered().IsSuccess);
        Assert.True(message.MarkDelivered().IsSuccess);
        Assert.True(message.MarkRead().IsSuccess);
        Assert.True(message.MarkRead().IsSuccess);
        Assert.Equal(SentAt, message.SentAt);
        Assert.Equal("provider-1", message.ProviderMessageId);
        Assert.Null(message.FailureCode);
    }

    private static OutboundMessage At(OutboundMessageStatus status)
    {
        var message = OutboundMessage.Queue(Centre, Conversation, "hello").Value;
        if (status is OutboundMessageStatus.Sent or OutboundMessageStatus.Delivered or OutboundMessageStatus.Read)
            Assert.True(message.MarkSent(SentAt, "provider-1").IsSuccess);
        if (status is OutboundMessageStatus.Delivered or OutboundMessageStatus.Read)
            Assert.True(message.MarkDelivered().IsSuccess);
        if (status == OutboundMessageStatus.Read) Assert.True(message.MarkRead().IsSuccess);
        if (status == OutboundMessageStatus.Failed) Assert.True(message.MarkFailed("provider.failed").IsSuccess);
        return message;
    }

    private static Result Move(OutboundMessage message, OutboundMessageStatus target) => target switch
    {
        OutboundMessageStatus.Sent => message.MarkSent(SentAt, "provider-1"),
        OutboundMessageStatus.Delivered => message.MarkDelivered(),
        OutboundMessageStatus.Read => message.MarkRead(),
        OutboundMessageStatus.Failed => message.MarkFailed("provider.failed"),
        _ => throw new ArgumentOutOfRangeException(nameof(target)),
    };

    private static void Invalid(Error? error)
    {
        Assert.NotNull(error);
        Assert.Equal("outbound.message_invalid", error.Code);
        Assert.Equal(ErrorKind.Validation, error.Kind);
    }
}
