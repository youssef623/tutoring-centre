using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Conversations;

public sealed class Conversation : Entity, ITenantOwned
{
    private Conversation() { }
    private Conversation(Guid centreId, Guid accountId, string address, PhoneNumber? phone, DateTimeOffset at)
    {
        CentreId = centreId;
        ChannelAccountId = accountId;
        CounterpartAddress = address;
        CounterpartPhone = phone;
        LastMessageAt = at;
    }

    private Conversation(Guid centreId, Guid accountId, Guid userId, PortalConversationSubject subject, DateTimeOffset at)
    {
        CentreId = centreId;
        ChannelAccountId = accountId;
        PortalUserId = userId;
        ParentId = subject.Kind == PortalConversationSubjectKind.Parent ? subject.Id : null;
        StudentId = subject.Kind == PortalConversationSubjectKind.Student ? subject.Id : null;
        IdentityStatus = IdentityStatus.Identified;
        IdentityResolvedAt = at;
        LastMessageAt = at;
    }

    public Guid CentreId { get; }
    public Guid ChannelAccountId { get; }
    public string? CounterpartAddress { get; }
    public PhoneNumber? CounterpartPhone { get; }
    public Guid? PortalUserId { get; }
    public Guid? StudentId { get; }
    public Guid? ParentId { get; private set; }
    public IdentityStatus IdentityStatus { get; private set; }
    public DateTimeOffset? IdentityResolvedAt { get; private set; }
    public DateTimeOffset LastMessageAt { get; private set; }

    public static Result<Conversation> Open(Guid centreId, Guid accountId, string counterpartAddress, PhoneNumber? phone, DateTimeOffset firstMessageAt) =>
        centreId == Guid.Empty || accountId == Guid.Empty || !IsDigits(counterpartAddress, 20)
            ? Result<Conversation>.Failure(Invalid())
            : Result<Conversation>.Success(new Conversation(centreId, accountId, counterpartAddress, phone, firstMessageAt));

    public static Result<Conversation> OpenForPortal(Guid centreId, Guid channelAccountId, Guid userId,
        PortalConversationSubject subject, DateTimeOffset at) =>
        centreId == Guid.Empty || channelAccountId == Guid.Empty || userId == Guid.Empty || subject.Id == Guid.Empty || !Enum.IsDefined(subject.Kind)
            ? Result<Conversation>.Failure(Invalid())
            : Result<Conversation>.Success(new Conversation(centreId, channelAccountId, userId, subject, at));

    public Result ResolveToParent(Guid parentId, DateTimeOffset at)
    {
        if (PortalUserId is not null)
            return Result.Failure(Error.Rule("conversations.portal_identity_fixed", "A portal conversation's identity cannot change."));
        if (parentId == Guid.Empty) return Result.Failure(Invalid());
        ParentId = parentId;
        IdentityStatus = IdentityStatus.Parent;
        IdentityResolvedAt = at;
        return Result.Success();
    }

    public void MarkUnknown(DateTimeOffset at)
    {
        if (PortalUserId is not null) return;
        ParentId = null;
        IdentityStatus = IdentityStatus.Unknown;
        IdentityResolvedAt = at;
    }

    internal static bool IsDigits(string? value, int max) => value is { Length: > 0 } && value.Length <= max && value.All(c => c is >= '0' and <= '9');
    internal static Error Invalid() => Error.Validation("conversations.message_invalid", "The conversation or message is invalid.");
}

/// <summary>Identifiers resolved by the portal session, independent of another module's entities.</summary>
public readonly record struct PortalConversationSubject(PortalConversationSubjectKind Kind, Guid Id);

public enum PortalConversationSubjectKind { Parent, Student }
