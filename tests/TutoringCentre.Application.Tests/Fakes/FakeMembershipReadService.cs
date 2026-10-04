using TutoringCentre.Application.Identity;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for the three canned responses a handler test needs.</summary>
public sealed class FakeMembershipReadService : IMembershipReadService
{
    public StaffProfileDto? Profile { get; set; }

    public ActiveMembershipDto? ActiveMembership { get; set; }

    public SessionStateDto? SessionState { get; set; }

    public Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct) => Task.FromResult(Profile);

    public Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct) =>
        Task.FromResult(ActiveMembership);

    public Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct) =>
        Task.FromResult(SessionState);
}
