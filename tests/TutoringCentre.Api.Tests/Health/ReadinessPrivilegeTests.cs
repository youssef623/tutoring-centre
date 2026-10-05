using System.Net;
using Microsoft.AspNetCore.Hosting;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Health;

/// <summary>The API test half of Task 19.7's eight tests: the runtime-role tripwire (Day 19) fails readiness closed.</summary>
[Collection(ApiCollection.Name)]
public sealed class ReadinessPrivilegeTests(ApiFactory factory)
{
    [Fact]
    public async Task GetReady_WithOwnerLoginAsRuntimeConnection_Returns503()
    {
        await using var ownerFactory = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("ConnectionStrings:Postgres", factory.OwnerConnectionString));
        using var client = ownerFactory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
