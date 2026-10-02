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
}
