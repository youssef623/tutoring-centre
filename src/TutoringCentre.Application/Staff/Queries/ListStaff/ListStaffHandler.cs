using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Staff.Queries.ListStaff;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListStaffHandler(ICurrentActor currentActor, IStaffReadService readService)
    : IQueryHandler<ListStaffQuery, IReadOnlyList<StaffMemberDto>>
{
    public async Task<Result<IReadOnlyList<StaffMemberDto>>> HandleAsync(ListStaffQuery query, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;
        var currentUserId = currentActor.Actor is StaffActor staffActor ? staffActor.UserId : Guid.Empty;

        var items = await readService.ListAsync(centreId, currentUserId, cancellationToken);
        return Result<IReadOnlyList<StaffMemberDto>>.Success(items);
    }
}
