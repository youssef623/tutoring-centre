namespace TutoringCentre.Application.Identity.Portal;

public interface IPortalAccountReadService
{
    Task<IReadOnlyList<PortalAccountDto>> ListAsync(Guid centreId, PortalSubjectKind kind, Guid subjectId, CancellationToken ct);
}
