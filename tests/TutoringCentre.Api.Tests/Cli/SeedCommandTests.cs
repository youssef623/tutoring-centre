using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Cli;

[Collection(ApiCollection.Name)]
public sealed class SeedCommandTests(ApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheTwoDemoCentres()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(2, await factory.ScalarAsync<long>("select count(*) from platform.centres"));
        Assert.Equal(
            "maadi-hub,nile-centre",
            await factory.ScalarAsync<string>("select string_agg(slug, ',' order by slug) from platform.centres"));
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheStaffSetIdempotently()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(5, await factory.ScalarAsync<long>("select count(*) from identity.users"));
        Assert.Equal(6, await factory.ScalarAsync<long>("select count(*) from identity.memberships"));
        Assert.Equal(
            1,
            await factory.ScalarAsync<long>("select count(*) from identity.memberships where status = 'inactive'"));
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoSubjectsIdempotently()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(5, await factory.ScalarAsync<long>("select count(*) from academics.subjects"));
        Assert.Equal(
            3,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.subjects s
                join platform.centres c on c.id = s.centre_id
                where c.slug = 'nile-centre'
                """));
        Assert.Equal(
            2,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.subjects s
                join platform.centres c on c.id = s.centre_id
                where c.slug = 'maadi-hub'
                """));
    }
}
