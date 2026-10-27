# `src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs`

## Purpose

Source summary (intent, not independent execution proof): Lets the app learn the real client address and scheme when it sits behind a reverse proxy, trusting that information only from configured, known proxy networks (Task 34.3) — a header is a claim, only evidence when it comes from a machine this app is told to trust.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 65 lines and 2,898 bytes. Source hash: `546f68fed3cbbca591c0333abc18091a7a500d8b5151a184b893f6da4856bb69`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.HttpOverrides;`
- Line 2: `using Microsoft.Extensions.Options;`
- Line 3: `using Serilog.Context;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs#L12): `public static class ForwardedHeadersSetup`
- [Line 14](../../../../../src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs#L14): `public static IServiceCollection AddProxyOptions(this IServiceCollection services, IConfiguration configuration)`
- [Line 32](../../../../../src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs#L32): `public static WebApplication UseTrustedForwardedHeaders(this WebApplication app)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
