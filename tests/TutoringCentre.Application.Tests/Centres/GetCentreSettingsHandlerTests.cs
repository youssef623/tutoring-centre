using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Centres.Queries.GetCentreSettings;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class GetCentreSettingsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsTheActorsOwnCentreSettings()
    {
        var centreId = Guid.CreateVersion7();
        var readService = new FakeCentreReadService();
        var dto = new CentreSettingsDto("Nile Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar, 7);
        readService.SettingsByCentreId[centreId] = dto;
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), centreId, StaffRole.Owner) };
        var handler = new GetCentreSettingsHandler(currentActor, readService);

        var result = await handler.HandleAsync(new GetCentreSettingsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(dto, result.Value);
    }

    [Fact]
    public async Task HandleAsync_MissingCentre_Throws()
    {
        var readService = new FakeCentreReadService();
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), Guid.CreateVersion7(), StaffRole.Owner) };
        var handler = new GetCentreSettingsHandler(currentActor, readService);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new GetCentreSettingsQuery(), CancellationToken.None));
    }
}
