using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Conversations;

/// <summary>The only pre-tenant lookup: resolve the receiving provider account to IDs, never phone or sender data.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class InboundChannelResolver(AppDbContext db) : IInboundChannelResolver
{
    public Task<ResolvedChannel?> ResolveAsync(ChannelProvider provider, string externalAccountId, CancellationToken cancellationToken = default) =>
        db.Set<ChannelAccount>().IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Provider == provider && x.ExternalAccountId == externalAccountId && x.Status == "active")
            .Select(x => new ResolvedChannel(x.CentreId, x.Id)).SingleOrDefaultAsync(cancellationToken);
}
