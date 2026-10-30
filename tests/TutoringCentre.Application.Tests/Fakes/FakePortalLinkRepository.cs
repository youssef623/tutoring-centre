using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Fakes;

public sealed class FakePortalLinkRepository : IPortalLinkRepository
{
    public List<PortalLink> Links { get; } = [];
    public Membership? Membership { get; set; }
    public uint? ExpectedVersion { get; private set; }
    public Task<bool> SubjectExistsAsync(Guid centreId, PortalSubjectKind kind, Guid subjectId, CancellationToken ct) =>
        Task.FromResult(Links.Any(link => link.CentreId == centreId
            && (kind == PortalSubjectKind.Parent ? link.ParentId == subjectId : link.StudentId == subjectId)));
    public Task<PortalLink?> GetAsync(Guid linkId, Guid centreId, CancellationToken ct) =>
        Task.FromResult(Links.SingleOrDefault(link => link.Id == linkId && link.CentreId == centreId));
    public Task<Membership?> GetMembershipForUpdateAsync(PortalLink link, uint expectedVersion, CancellationToken ct)
    {
        ExpectedVersion = expectedVersion;
        return Task.FromResult(Membership?.CentreId == link.CentreId && Membership.UserId == link.UserId ? Membership : null);
    }
    public void Add(PortalLink link) => Links.Add(link);
}
