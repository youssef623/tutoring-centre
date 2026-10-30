using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations;

public sealed record ResolvedChannel(Guid CentreId, Guid ChannelAccountId);

public interface IInboundChannelResolver
{
    Task<ResolvedChannel?> ResolveAsync(ChannelProvider provider, string externalAccountId, CancellationToken cancellationToken = default);
}
