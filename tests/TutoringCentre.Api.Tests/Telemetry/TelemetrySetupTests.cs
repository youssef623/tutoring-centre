using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using TutoringCentre.Api.Telemetry;

namespace TutoringCentre.Api.Tests.Telemetry;

public sealed class TelemetrySetupTests
{
    [Fact]
    public void NoEndpointConfigured_RegistersNoTracerProvider()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration([]);

        services.AddApiTelemetry(configuration);
        var provider = services.BuildServiceProvider();

        Assert.Null(provider.GetService<TracerProvider>());
    }

    [Fact]
    public void EndpointConfigured_RegistersATracerProvider()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration([new("Otel:Endpoint", "http://localhost:4318")]);

        services.AddApiTelemetry(configuration);
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<TracerProvider>());
    }

    private static IConfiguration BuildConfiguration(KeyValuePair<string, string?>[] entries) =>
        new ConfigurationBuilder().AddInMemoryCollection(entries).Build();
}
