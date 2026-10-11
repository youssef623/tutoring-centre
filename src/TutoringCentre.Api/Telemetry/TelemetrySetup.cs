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
