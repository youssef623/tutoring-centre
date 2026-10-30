using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Identity.Portal.Queries.ListPortalAccounts;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ListPortalAccountsHandler(ICurrentActor actor, IPortalAccountReadService reads)
    : IQueryHandler<ListPortalAccountsQuery, IReadOnlyList<PortalAccountDto>>
{
    public async Task<Result<IReadOnlyList<PortalAccountDto>>> HandleAsync(ListPortalAccountsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyList<PortalAccountDto>>.Success(await reads.ListAsync(actor.Actor.CentreId!.Value, query.Kind, query.SubjectId, cancellationToken));
}
