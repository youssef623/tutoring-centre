using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for Identity's published staff-directory contract. <see cref="Members"/> holds
/// (centreId, userId, role) tuples for every active membership the test wants to simulate; anything not in
/// the list answers false, the same single answer the real adapter gives for an unknown user, a user of
/// another centre and a user with another role.
/// </summary>
public sealed class FakeStaffDirectory : IStaffDirectory
{
    public List<(Guid CentreId, Guid UserId, StaffRole Role)> Members { get; } = [];

    public Task<bool> IsActiveMemberWithRoleAsync(Guid centreId, Guid userId, StaffRole role, CancellationToken ct) =>
        Task.FromResult(Members.Contains((centreId, userId, role)));
}
