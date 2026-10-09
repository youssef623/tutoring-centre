using TutoringCentre.Application.Staff;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>Records the centre and current-user arguments the handler passed, so a test can prove they came from the actor, not a parameter.</summary>
public sealed class FakeStaffReadService : IStaffReadService
{
    public List<StaffMemberDto> Items { get; set; } = [];

    public Guid? LastCentreIdRequested { get; private set; }

    public Guid? LastCurrentUserIdRequested { get; private set; }

    public Task<IReadOnlyList<StaffMemberDto>> ListAsync(Guid centreId, Guid currentUserId, CancellationToken ct)
    {
        LastCentreIdRequested = centreId;
        LastCurrentUserIdRequested = currentUserId;
        return Task.FromResult<IReadOnlyList<StaffMemberDto>>(Items);
    }
}
