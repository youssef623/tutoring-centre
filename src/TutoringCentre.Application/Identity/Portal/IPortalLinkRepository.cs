using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Portal;

public interface IPortalLinkRepository
{
    Task<bool> SubjectExistsAsync(Guid centreId, PortalSubjectKind kind, Guid subjectId, CancellationToken ct);
    Task<PortalLink?> GetAsync(Guid linkId, Guid centreId, CancellationToken ct);
    Task<Membership?> GetMembershipForUpdateAsync(PortalLink link, uint expectedVersion, CancellationToken ct);
    void Add(PortalLink link);
}
