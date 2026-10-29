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

    public Guid CentreId { get; }
    public Guid ChannelAccountId { get; }
    public string CounterpartAddress { get; } = null!;
    public PhoneNumber? CounterpartPhone { get; }
    public Guid? ParentId { get; private set; }
    public IdentityStatus IdentityStatus { get; private set; }
    public DateTimeOffset? IdentityResolvedAt { get; private set; }
    public DateTimeOffset LastMessageAt { get; private set; }

    public static Result<Conversation> Open(Guid centreId, Guid accountId, string counterpartAddress, PhoneNumber? phone, DateTimeOffset firstMessageAt) =>
        centreId == Guid.Empty || accountId == Guid.Empty || !IsDigits(counterpartAddress, 20)
            ? Result<Conversation>.Failure(Invalid())
            : Result<Conversation>.Success(new Conversation(centreId, accountId, counterpartAddress, phone, firstMessageAt));

    public Result ResolveToParent(Guid parentId, DateTimeOffset at)
    {
        if (parentId == Guid.Empty) return Result.Failure(Invalid());
        ParentId = parentId;
        IdentityStatus = IdentityStatus.Parent;
        IdentityResolvedAt = at;
        return Result.Success();
    }

    public void MarkUnknown(DateTimeOffset at)
    {
        ParentId = null;
        IdentityStatus = IdentityStatus.Unknown;
        IdentityResolvedAt = at;
    }

    internal static bool IsDigits(string? value, int max) => value is { Length: > 0 } && value.Length <= max && value.All(c => c is >= '0' and <= '9');
    internal static Error Invalid() => Error.Validation("conversations.message_invalid", "The conversation or message is invalid.");
}
