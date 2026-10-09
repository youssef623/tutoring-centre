using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for the staff write-side port. <see cref="Memberships"/> seeds existing rows for any number of centres; every method honours the centre argument it is given, so a scoping bug in a handler shows up as a wrong-centre test failing instead of passing by accident.</summary>
public sealed class FakeMembershipRepository : IMembershipRepository
{
    public List<Membership> Memberships { get; } = [];

    public List<Membership> Added { get; } = [];

    private IEnumerable<Membership> All => Memberships.Concat(Added);

    public Task<Membership?> GetForUpdateAsync(Guid id, Guid centreId, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(All.FirstOrDefault(membership => membership.Id == id && membership.CentreId == centreId));

    public Task<bool> ExistsForUserAsync(Guid userId, Guid centreId, CancellationToken ct) =>
        Task.FromResult(All.Any(membership => membership.UserId == userId && membership.CentreId == centreId));

    public Task<int> LockActiveOwnersAsync(Guid centreId, CancellationToken ct) =>
        Task.FromResult(All.Count(membership =>
            membership.CentreId == centreId && membership.Role == StaffRole.Owner && membership.Status == MembershipStatus.Active));

    public void Add(Membership membership) => Added.Add(membership);
}
