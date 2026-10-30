using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Conversations;

public sealed class ChannelAccount : Entity, ITenantOwned
{
    private ChannelAccount() { }
    private ChannelAccount(Guid centreId, ChannelProvider provider, string externalAccountId, PhoneNumber displayPhone)
    {
        CentreId = centreId;
        Provider = provider;
        ExternalAccountId = externalAccountId;
        DisplayPhone = displayPhone;
    }

    public Guid CentreId { get; }
    public ChannelProvider Provider { get; }
    public string ExternalAccountId { get; } = null!;
    public PhoneNumber DisplayPhone { get; } = null!;
    public string Status { get; private set; } = "active";

    public static Result<ChannelAccount> Register(Guid centreId, ChannelProvider provider, string externalAccountId, PhoneNumber displayPhone)
    {
        ArgumentNullException.ThrowIfNull(displayPhone);
        return centreId == Guid.Empty || !Enum.IsDefined(provider) || !Conversation.IsDigits(externalAccountId, 64)
            ? Result<ChannelAccount>.Failure(Conversation.Invalid())
            : Result<ChannelAccount>.Success(new ChannelAccount(centreId, provider, externalAccountId, displayPhone));
    }

    public void Disable() => Status = "disabled";
}
