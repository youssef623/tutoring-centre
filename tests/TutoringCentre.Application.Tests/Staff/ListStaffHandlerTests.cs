using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class ListStaffHandlerTests
{
    [Fact]
    public async Task HandleAsync_PassesTheActorsCentreAndUserToTheReadService()
    {
        var centreId = Guid.CreateVersion7();
        var userId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, centreId, StaffRole.Owner));
        var readService = new FakeStaffReadService();
        var handler = new ListStaffHandler(actorContext, readService);

        var result = await handler.HandleAsync(new ListStaffQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(centreId, readService.LastCentreIdRequested);
        Assert.Equal(userId, readService.LastCurrentUserIdRequested);
    }
}
