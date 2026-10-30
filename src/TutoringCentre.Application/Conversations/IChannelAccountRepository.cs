using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations;

public interface IChannelAccountRepository
{
    Task<ChannelAccount?> FindAsync(Guid id, CancellationToken cancellationToken);
    void Add(ChannelAccount account);
}
