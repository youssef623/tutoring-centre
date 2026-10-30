using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Conversations;

public sealed class ChannelAccount : Entity, ITenantOwned
{
    private ChannelAccount() { }
    private ChannelAccount(Guid centreId, ChannelProvider provider, string externalAccountId, PhoneNumber? displayPhone)
    {
        CentreId = centreId;
        Provider = provider;
        ExternalAccountId = externalAccountId;
        DisplayPhone = displayPhone;
    }

    public Guid CentreId { get; }
    public ChannelProvider Provider { get; }
    public string ExternalAccountId { get; } = null!;
    public PhoneNumber? DisplayPhone { get; }
    public string Status { get; private set; } = "active";

    public static Result<ChannelAccount> Register(Guid centreId, ChannelProvider provider, string externalAccountId, PhoneNumber displayPhone)
    {
        ArgumentNullException.ThrowIfNull(displayPhone);
        return centreId == Guid.Empty || provider != ChannelProvider.Whatsapp || !Conversation.IsDigits(externalAccountId, 64)
            ? Result<ChannelAccount>.Failure(Conversation.Invalid())
            : Result<ChannelAccount>.Success(new ChannelAccount(centreId, provider, externalAccountId, displayPhone));
    }

    public static Result<ChannelAccount> RegisterWeb(Guid centreId, string externalAccountId)
    {
        var slug = externalAccountId is { Length: > 4 and <= 64 } && externalAccountId.StartsWith("web:", StringComparison.Ordinal)
            ? externalAccountId[4..] : string.Empty;
        return centreId == Guid.Empty || slug.Length == 0 || slug[0] == '-' || slug[^1] == '-' || slug.Contains("--", StringComparison.Ordinal)
            || slug.Any(c => c is not (>= 'a' and <= 'z' or >= '0' and <= '9' or '-'))
            ? Result<ChannelAccount>.Failure(Conversation.Invalid())
            : Result<ChannelAccount>.Success(new ChannelAccount(centreId, ChannelProvider.Web, externalAccountId, null));
    }

    public void Disable() => Status = "disabled";
}
