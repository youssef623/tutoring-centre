using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace TutoringCentre.Api.Http;

/// <summary>
/// Lets the app learn the real client address and scheme when it sits behind a reverse proxy, trusting that
/// information only from configured, known proxy networks (Task 34.3) — a header is a claim, only evidence
/// when it comes from a machine this app is told to trust.
/// </summary>
public static class ForwardedHeadersSetup
{
    public static IServiceCollection AddProxyOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<ProxyOptions>().Bind(configuration.GetSection("Proxy")).ValidateOnStart();
        services.AddSingleton<IValidateOptions<ProxyOptions>, ProxyOptionsValidator>();

        return services;
    }

    /// <summary>
    /// First in the pipeline, unconditionally: only X-Forwarded-For and X-Forwarded-Proto are processed, one
    /// hop deep (a client cannot add its own forwarded-for hop in front of the real proxy's). With no networks
    /// configured — the Development/Testing default — KnownIPNetworks and KnownProxies are both empty, which
    /// means the middleware trusts no immediate connection at all (not even loopback, ASP.NET Core's own
    /// built-in default), so it is a no-op: headers are read but never applied.
    /// </summary>
    public static WebApplication UseTrustedForwardedHeaders(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var proxyOptions = app.Services.GetRequiredService<IOptions<ProxyOptions>>().Value;

        var forwardedHeadersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            ForwardLimit = 1,
        };
        forwardedHeadersOptions.KnownIPNetworks.Clear();
        forwardedHeadersOptions.KnownProxies.Clear();
        foreach (var network in proxyOptions.KnownNetworks)
        {
            forwardedHeadersOptions.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(network));
        }

        app.UseForwardedHeaders(forwardedHeadersOptions);

        // After forwarded-header processing, so this is the real client address when the direct connection
        // was a trusted proxy, and the proxy's own address otherwise. Every log line — the request-completion
        // line included — carries it, the same way CorrelationIdMiddleware exposes the correlation id.
        app.Use((context, next) =>
        {
            using (LogContext.PushProperty("ClientIp", context.Connection.RemoteIpAddress?.ToString() ?? "unknown"))
            {
                return next(context);
            }
        });

        return app;
    }
}
