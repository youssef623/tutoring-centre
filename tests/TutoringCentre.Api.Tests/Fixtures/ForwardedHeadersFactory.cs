using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Serilog.Core;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Trusts 10.0.0.0/8 as the proxy network (Task 34.3), with a simulated direct-connection address per request.</summary>
public sealed class ForwardedHeadersFactory : ApiFactory
{
    public const string TrustedDirectAddress = "10.0.0.1";
    public const string UntrustedDirectAddress = "198.51.100.1";

    public InMemoryLogSink Logs { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseSetting("Proxy:KnownNetworks:0", "10.0.0.0/8");
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IStartupFilter, SimulatedDirectConnectionStartupFilter>();
            services.AddSingleton<ILogEventSink>(Logs);
        });
    }
}
