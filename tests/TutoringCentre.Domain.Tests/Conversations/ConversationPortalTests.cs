using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Domain.Tests.Conversations;

public sealed class ConversationPortalTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 30, 12, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(PortalConversationSubjectKind.Parent)]
    [InlineData(PortalConversationSubjectKind.Student)]
    public void OpenForPortal_IdentifiesExactlyOneSubjectWithoutAnAddress(PortalConversationSubjectKind kind)
    {
        var centreId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var result = Conversation.OpenForPortal(centreId, accountId, userId, new(kind, subjectId), Now);

        Assert.True(result.IsSuccess);
        var conversation = result.Value;
        Assert.NotEqual(Guid.Empty, conversation.Id);
        Assert.Equal(centreId, conversation.CentreId);
        Assert.Equal(accountId, conversation.ChannelAccountId);
        Assert.Equal(userId, conversation.PortalUserId);
        Assert.Equal(kind == PortalConversationSubjectKind.Parent ? subjectId : (Guid?)null, conversation.ParentId);
        Assert.Equal(kind == PortalConversationSubjectKind.Student ? subjectId : (Guid?)null, conversation.StudentId);
        Assert.Null(conversation.CounterpartAddress);
        Assert.Null(conversation.CounterpartPhone);
        Assert.Equal(IdentityStatus.Identified, conversation.IdentityStatus);
        Assert.Equal(Now, conversation.IdentityResolvedAt);
        Assert.Equal(Now, conversation.LastMessageAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void OpenForPortal_RejectsEveryEmptyIdentifierForBothKinds(int position)
    {
        var ids = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToArray();
        ids[position] = Guid.Empty;
        foreach (var kind in Enum.GetValues<PortalConversationSubjectKind>())
        {
            var result = Conversation.OpenForPortal(ids[0], ids[1], ids[2], new(kind, ids[3]), Now);
            Assert.True(result.IsFailure);
            Assert.Equal("conversations.message_invalid", result.Error?.Code);
        }
    }

    [Fact]
    public void OpenForPortal_RejectsUndefinedSubjectKind()
    {
        var result = Conversation.OpenForPortal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            new((PortalConversationSubjectKind)99, Guid.NewGuid()), Now);
        Assert.True(result.IsFailure);
        Assert.Equal("conversations.message_invalid", result.Error?.Code);
    }

    [Theory]
    [InlineData(PortalConversationSubjectKind.Parent)]
    [InlineData(PortalConversationSubjectKind.Student)]
    public void PhoneResolutionCannotReplacePortalIdentity(PortalConversationSubjectKind kind)
    {
        var subjectId = Guid.NewGuid();
        var conversation = Conversation.OpenForPortal(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new(kind, subjectId), Now).Value;
        var result = conversation.ResolveToParent(Guid.NewGuid(), Now.AddMinutes(1));
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.Rule, result.Error?.Kind);
        Assert.Equal("conversations.portal_identity_fixed", result.Error?.Code);
        conversation.MarkUnknown(Now.AddMinutes(2));

        Assert.Equal(kind == PortalConversationSubjectKind.Parent ? subjectId : (Guid?)null, conversation.ParentId);
        Assert.Equal(kind == PortalConversationSubjectKind.Student ? subjectId : (Guid?)null, conversation.StudentId);
        Assert.Equal(IdentityStatus.Identified, conversation.IdentityStatus);
        Assert.Equal(Now, conversation.IdentityResolvedAt);
        Assert.Equal(Now, conversation.LastMessageAt);
    }

    [Fact]
    public void ExistingPhoneConversationStillResolvesAndBecomesUnknown()
    {
        var phone = PhoneNumber.Create("+201000000001").Value;
        var conversation = Conversation.Open(Guid.NewGuid(), Guid.NewGuid(), "201000000001", phone, Now).Value;
        Assert.Equal("201000000001", conversation.CounterpartAddress);
        Assert.Equal(phone, conversation.CounterpartPhone);
        Assert.Null(conversation.PortalUserId);
        Assert.Null(conversation.StudentId);
        Assert.Equal(IdentityStatus.Unresolved, conversation.IdentityStatus);

        var parentId = Guid.NewGuid();
        Assert.True(conversation.ResolveToParent(parentId, Now.AddMinutes(1)).IsSuccess);
        Assert.Equal(parentId, conversation.ParentId);
        Assert.Equal(IdentityStatus.Parent, conversation.IdentityStatus);
        conversation.MarkUnknown(Now.AddMinutes(2));
        Assert.Null(conversation.ParentId);
        Assert.Equal(IdentityStatus.Unknown, conversation.IdentityStatus);
        Assert.Equal(Now.AddMinutes(2), conversation.IdentityResolvedAt);
    }

    [Theory]
    [InlineData(nameof(Conversation.PortalUserId))]
    [InlineData(nameof(Conversation.StudentId))]
    [InlineData(nameof(Conversation.CounterpartAddress))]
    public void PortalIdentityPropertiesExposeNoSetters(string name) =>
        Assert.Null(typeof(Conversation).GetProperty(name)!.SetMethod);
}
