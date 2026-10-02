using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TutoringCentre.Api.Tests.Health;

public sealed class HealthEndpointTests
{
    // Port 1 on loopback has nothing listening, so the connection is refused immediately;
    // Timeout=2 caps the wait on systems that retry before refusing. The value is present (startup validation
    // requires it) but unreachable (readiness must report 503).
    private const string UnreachableDatabase =
        "Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=2";

    [Fact]
    public async Task Liveness_ReturnsOk()
    {
        // Arrange
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Readiness_WhenDatabaseUnreachable_Returns503()
    {
        // Arrange
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/health/ready");

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    // "Testing" = no development auto-migration, no user-secrets, no dev seed: the test controls the configuration.
    private static WebApplicationFactory<Program> CreateFactory() =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting("ConnectionStrings:Postgres", UnreachableDatabase);
        });
}
