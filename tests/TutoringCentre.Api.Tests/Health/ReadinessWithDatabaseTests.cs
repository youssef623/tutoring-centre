using System.Net;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Health;

[Collection(ApiCollection.Name)]
public sealed class ReadinessWithDatabaseTests(ApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetReady_WithReachableDatabase_Returns200()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
