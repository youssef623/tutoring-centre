# `src/TutoringCentre.Api/Telemetry/TelemetrySetup.cs`

## Purpose

Source summary (intent, not independent execution proof): Traces, metrics and logs, wired only in this project's composition root (Task 35.6): Domain and Application gain no telemetry dependency. Off entirely unless configuration key Otel:Endpoint is set, so Development and the test suite export nothing and never attempt a network call. The manual's own design targets the Azure Monitor exporter once Azure is provisioned (deferred, no subscription yet); until then this points at a plain OTLP endpoint — a local viewer such as the .NET Aspire Dashboard, or any other OTLP collector — so the same instrumentation is exercised without any cloud resource.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Telemetry/TelemetrySetup.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 71 lines and 3,400 bytes. Source hash: `e06611801d8700cfb4e520918fab0da625ca706e786fb63d77a5a0626835807b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Reflection;`
- Line 2: `using Microsoft.Extensions.Logging;`
- Line 3: `using Npgsql;`
- Line 4: `using OpenTelemetry;`
- Line 5: `using OpenTelemetry.Logs;`
- Line 6: `using OpenTelemetry.Metrics;`
- Line 7: `using OpenTelemetry.Resources;`
- Line 8: `using OpenTelemetry.Trace;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../src/TutoringCentre.Api/Telemetry/TelemetrySetup.cs#L21): `public static class TelemetrySetup`
- [Line 25](../../../../../src/TutoringCentre.Api/Telemetry/TelemetrySetup.cs#L25): `public static IServiceCollection AddApiTelemetry(this IServiceCollection services, IConfiguration configuration)`
- [Line 56](../../../../../src/TutoringCentre.Api/Telemetry/TelemetrySetup.cs#L56): `// Request and response bodies are never part of a log record in this codebase (Month 1); nothing`
- [Line 65](../../../../../src/TutoringCentre.Api/Telemetry/TelemetrySetup.cs#L65): `private static string ReadVersion()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Reflection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TutoringCentre.Api.Telemetry;

/// <summary>
/// Traces, metrics and logs, wired only in this project's composition root (Task 35.6): Domain and Application
/// gain no telemetry dependency. Off entirely unless configuration key Otel:Endpoint is set, so Development
/// and the test suite export nothing and never attempt a network call.
///
/// The manual's own design targets the Azure Monitor exporter once Azure is provisioned (deferred, no
/// subscription yet); until then this points at a plain OTLP endpoint — a local viewer such as the .NET Aspire
/// Dashboard, or any other OTLP collector — so the same instrumentation is exercised without any cloud resource.
/// </summary>
public static class TelemetrySetup
{
    private const string ServiceName = "tutoring-centre-api";

    public static IServiceCollection AddApiTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var endpoint = configuration["Otel:Endpoint"];
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return services;
        }

        var endpointUri = new Uri(endpoint);
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(ServiceName, serviceVersion: ReadVersion());

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(ServiceName, serviceVersion: ReadVersion()))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                    // Health probes are noise, not a request worth tracing (same exclusion as request logging).
                    options.Filter = context => !context.Request.Path.StartsWithSegments("/health"))
                // Npgsql's own ActivitySource: records the SQL statement text, never parameter values (no
                // enrichment callback adds them here) — the same no-secrets rule Month 1's log redaction holds to.
                .AddNpgsql()
                .AddOtlpExporter(otlp => otlp.Endpoint = endpointUri))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(otlp => otlp.Endpoint = endpointUri));

        services.AddLogging(logging => logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);
            // Request and response bodies are never part of a log record in this codebase (Month 1); nothing
            // extra needs excluding here. The dispatcher's existing structured log line (Dispatcher.LogOutcome)
            // flows through this provider unchanged, alongside the console sink Serilog already writes to.
            options.AddOtlpExporter(otlp => otlp.Endpoint = endpointUri);
        }));

        return services;
    }

    private static string ReadVersion()
    {
        var informational = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        return string.IsNullOrWhiteSpace(informational) ? "unknown" : informational;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
