using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ChannelAccountRepository(AppDbContext db) : IChannelAccountRepository
{
    public Task<ChannelAccount?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        db.Set<ChannelAccount>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Add(ChannelAccount account) => db.Set<ChannelAccount>().Add(account);
}
