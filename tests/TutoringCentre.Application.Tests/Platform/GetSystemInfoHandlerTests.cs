using TutoringCentre.Application.Platform;
using TutoringCentre.Application.Platform.Queries.GetSystemInfo;

namespace TutoringCentre.Application.Tests.Platform;

public sealed class GetSystemInfoHandlerTests
{
    [Fact]
    public async Task HandleAsync_NoPendingMigrations_ReportsDatabaseUpToDate()
    {
        var handler = new GetSystemInfoHandler(
            new FakeSystemInfoReadService(new SchemaStatus("20261012_InitialPlatform", 0)),
            new FakeDemoModeProvider(false));

        var result = await handler.HandleAsync(new GetSystemInfoQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.DatabaseUpToDate);
        Assert.Equal("20261012_InitialPlatform", result.Value.LatestMigration);
    }

    [Fact]
    public async Task HandleAsync_TwoPendingMigrations_ReportsDatabaseNotUpToDate()
    {
        var handler = new GetSystemInfoHandler(
            new FakeSystemInfoReadService(new SchemaStatus("20261012_InitialPlatform", 2)),
            new FakeDemoModeProvider(false));

        var result = await handler.HandleAsync(new GetSystemInfoQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.DatabaseUpToDate);
    }

    [Fact]
    public async Task HandleAsync_AnyStatus_ReturnsNonEmptyVersion()
    {
        var handler = new GetSystemInfoHandler(
            new FakeSystemInfoReadService(new SchemaStatus(null, 0)),
            new FakeDemoModeProvider(false));

        var result = await handler.HandleAsync(new GetSystemInfoQuery(), CancellationToken.None);

        // Whatever the SDK put in AssemblyInformationalVersionAttribute is returned as-is (Task 34.2): once the
        // Dockerfile passes -p:SourceRevisionId=<git sha> to `dotnet publish`, that is a "+<sha>" suffix here,
        // deliberately kept rather than stripped, so this is how an operator confirms what is actually deployed.
        Assert.False(string.IsNullOrWhiteSpace(result.Value.ApplicationVersion));
        Assert.Null(result.Value.LatestMigration);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task HandleAsync_ReportsDemoModeFromProvider(bool demoEnabled)
    {
        var handler = new GetSystemInfoHandler(
            new FakeSystemInfoReadService(new SchemaStatus(null, 0)),
            new FakeDemoModeProvider(demoEnabled));

        var result = await handler.HandleAsync(new GetSystemInfoQuery(), CancellationToken.None);

        Assert.Equal(demoEnabled, result.Value.DemoMode);
    }

    private sealed class FakeSystemInfoReadService(SchemaStatus status) : ISystemInfoReadService
    {
        public Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken ct) => Task.FromResult(status);
    }

    private sealed class FakeDemoModeProvider(bool isEnabled) : IDemoModeProvider
    {
        public bool IsEnabled => isEnabled;
    }
}
