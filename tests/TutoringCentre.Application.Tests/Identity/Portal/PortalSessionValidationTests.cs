using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Queries.ValidateStaffSession;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity.Portal;

public sealed class PortalSessionValidationTests
{
    [Theory]
    [InlineData(StaffRole.Parent, PortalSubjectKind.Parent)]
    [InlineData(StaffRole.Student, PortalSubjectKind.Student)]
    public async Task ValidatedSubjectTravelsWithSessionResult(StaffRole role, PortalSubjectKind kind)
    {
        var subject = new PortalSubject(kind, Guid.NewGuid());
        var reads = new FakeMembershipReadService { SessionState = new("stamp", true, role, subject) };
        var result = await new ValidateStaffSessionHandler(reads).HandleAsync(new(Guid.NewGuid(), "stamp", Guid.NewGuid(), role), CancellationToken.None);
        Assert.True(result.Value.IsValid);
        Assert.Equal(subject, result.Value.PortalSubject);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task MissingMismatchedOrInactiveSubjectInvalidatesSession(bool mismatch, bool inactive)
    {
        var subject = mismatch ? new PortalSubject(PortalSubjectKind.Student, Guid.NewGuid()) : null;
        if (inactive) subject = new(PortalSubjectKind.Parent, Guid.NewGuid());
        var reads = new FakeMembershipReadService { SessionState = new("stamp", !inactive, StaffRole.Parent, subject) };
        var result = await new ValidateStaffSessionHandler(reads).HandleAsync(new(Guid.NewGuid(), "stamp", Guid.NewGuid(), StaffRole.Parent), CancellationToken.None);
        Assert.False(result.Value.IsValid);
        Assert.Null(result.Value.PortalSubject);
    }
}
