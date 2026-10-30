using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Queries.ListPortalAccounts;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity.Portal;

public sealed class ListPortalAccountsHandlerTests
{
    [Fact]
    public async Task QueryPassesOnlyActorCentreAndRequestedSubjectToReadService()
    {
        var centre = Guid.NewGuid();
        var subject = Guid.NewGuid();
        var actor = new CurrentActorContext();
        actor.Set(new StaffActor(Guid.NewGuid(), centre, StaffRole.Owner));
        var reads = new CapturingReadService();
        var result = await new ListPortalAccountsHandler(actor, reads).HandleAsync(new(PortalSubjectKind.Parent, subject), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        Assert.Equal((centre, PortalSubjectKind.Parent, subject), reads.Arguments);
    }

    [Fact]
    public void ValidatorRequiresDefinedKindAndSubject()
    {
        var validator = new ListPortalAccountsValidator();
        Assert.True(validator.Validate(new ListPortalAccountsQuery(PortalSubjectKind.Student, Guid.NewGuid())).IsValid);
        Assert.False(validator.Validate(new ListPortalAccountsQuery((PortalSubjectKind)99, Guid.NewGuid())).IsValid);
        Assert.False(validator.Validate(new ListPortalAccountsQuery(PortalSubjectKind.Parent, Guid.Empty)).IsValid);
    }

    private sealed class CapturingReadService : IPortalAccountReadService
    {
        public (Guid, PortalSubjectKind, Guid) Arguments { get; private set; }
        public Task<IReadOnlyList<PortalAccountDto>> ListAsync(Guid centreId, PortalSubjectKind kind, Guid subjectId, CancellationToken ct)
        {
            Arguments = (centreId, kind, subjectId);
            return Task.FromResult<IReadOnlyList<PortalAccountDto>>([]);
        }
    }
}
