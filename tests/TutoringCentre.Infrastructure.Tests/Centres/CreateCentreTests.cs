using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Centres;

public sealed class CreateCentreTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    private static CreateCentreCommand ValidCommand(string slug = "nile-centre") =>
        new("Nile Tutoring Centre", slug, "Africa/Cairo", SupportedLocale.Ar);

    [Fact]
    public async Task SendAsync_SystemActorValidCommand_PersistsCentreWithCreatedAtAndNoUpdatedAt()
    {
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), ValidCommand());

        Assert.True(result.IsSuccess);
        Assert.Equal("nile-centre", result.Value.Slug);
        Assert.Equal(1, await Fixture.CountCentresAsync());
        Assert.Equal(1, await Fixture.ScalarAsync<long>(
            "select count(*) from platform.centres where slug = 'nile-centre' and created_at is not null and updated_at is null"));
    }

    [Fact]
    public async Task SendAsync_SlugAlreadyExists_ReturnsConflictAndKeepsOneRow()
    {
        await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), ValidCommand());

        var second = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), ValidCommand());

        Assert.True(second.IsFailure);
        Assert.Equal("centre.slug_taken", second.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, second.Error.Kind);
        Assert.Equal(1, await Fixture.CountCentresAsync());
    }

    [Fact]
    public async Task SendAsync_AnonymousActor_ReturnsForbiddenAndWritesNothing()
    {
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new AnonymousActor(), ValidCommand());

        Assert.True(result.IsFailure);
        Assert.Equal("centre.create_forbidden", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Equal(0, await Fixture.CountCentresAsync());
    }

    [Fact]
    public async Task SendAsync_InvalidTimeZone_ReturnsDomainErrorAndWritesNothing()
    {
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Mars/Base", SupportedLocale.Ar);

        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), command);

        Assert.True(result.IsFailure);
        Assert.Equal("centre.time_zone_invalid", result.Error!.Code);
        Assert.Equal(0, await Fixture.CountCentresAsync());
    }
}
