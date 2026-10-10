using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class UpdateCentreSettingsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidUpdate_ChangesNameAndLocale()
    {
        var centre = Centre.Create("Nile Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar).Value;
        var repository = new FakeCentreRepository();
        repository.Existing.Add(centre);
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), centre.Id, StaffRole.Owner) };
        var handler = new UpdateCentreSettingsHandler(currentActor, repository);

        var result = await handler.HandleAsync(
            new UpdateCentreSettingsCommand("Nile Learning Centre", SupportedLocale.En, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Nile Learning Centre", centre.Name);
        Assert.Equal(SupportedLocale.En, centre.DefaultLocale);
    }

    [Fact]
    public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndChangesNothing()
    {
        var centre = Centre.Create("Nile Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar).Value;
        var repository = new FakeCentreRepository();
        repository.Existing.Add(centre);
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), centre.Id, StaffRole.Owner) };
        var handler = new UpdateCentreSettingsHandler(currentActor, repository);

        var result = await handler.HandleAsync(
            new UpdateCentreSettingsCommand("", SupportedLocale.En, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("centre.name_invalid", result.Error!.Code);
        Assert.Equal("Nile Centre", centre.Name);
        Assert.Equal(SupportedLocale.Ar, centre.DefaultLocale);
    }

    [Fact]
    public async Task HandleAsync_MissingCentre_Throws()
    {
        var repository = new FakeCentreRepository();
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), Guid.CreateVersion7(), StaffRole.Owner) };
        var handler = new UpdateCentreSettingsHandler(currentActor, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new UpdateCentreSettingsCommand("Name", SupportedLocale.En, 0), CancellationToken.None));
    }
}
