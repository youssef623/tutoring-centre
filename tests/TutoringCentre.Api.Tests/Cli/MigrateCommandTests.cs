using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Cli;

[Collection(ApiCollection.Name)]
public sealed class MigrateCommandTests(ApiFactory factory)
{
    [Fact]
    public async Task RunAsync_CalledTwice_SecondRunIsANoOp()
    {
        var historyCountBefore = await factory.ScalarAsync<long>("select count(*) from platform.__ef_migrations_history");

        var firstExitCode = await MigrateCommand.RunAsync(factory.Services);
        var secondExitCode = await MigrateCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(
            historyCountBefore,
            await factory.ScalarAsync<long>("select count(*) from platform.__ef_migrations_history"));
    }
}
