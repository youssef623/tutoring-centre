using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Endpoints;
using TutoringCentre.Api.Http;
using TutoringCentre.Api.Logging;
using TutoringCentre.Application;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Destructure.With<SensitiveDataDestructuringPolicy>()
        .WriteTo.Console(new RenderedCompactJsonFormatter()),
    // Each host gets its own logger instead of overwriting the process-wide static Log.Logger: several
    // WebApplicationFactory hosts (ApiFactory, ConventionsFactory) run concurrently in the test process,
    // and without this the last host to start wins, silently dropping test-only sinks like InMemoryLogSink.
    preserveStaticLogger: true);

builder.Services.AddHealthChecks();
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
builder.Services.AddApiProblemDetails();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Unknown members fail the request (400 request.malformed) instead of being silently ignored.
    options.SerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

// Outside Development, minimal APIs answer a bad request body with an empty 400 instead of throwing.
// Throwing in every environment lets GlobalExceptionHandler return the uniform `request.malformed` Problem Details (discrepancy D10).
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

var app = builder.Build();

// CLI mode: `dotnet run --project src/TutoringCentre.Api -- seed`
if (args is ["seed"])
{
    await app.Services.ApplyMigrationsAsync();
    return await SeedCommand.RunAsync(app.Services);
}

// Development convenience only. Production migrations run from the deployment pipeline (Month 2), never at app startup:
// auto-migrating there is risky (several instances racing, no review, long locks).
if (app.Environment.IsDevelopment())
{
    await app.Services.ApplyMigrationsAsync();
}

app.UseMiddleware<CorrelationIdMiddleware>();

// One line per HTTP request; health probes are noise at Information level.
app.UseSerilogRequestLogging(options => options.GetLevel = (httpContext, _, exception) =>
    exception is not null || httpContext.Response.StatusCode >= 500
        ? LogEventLevel.Error
        : httpContext.Request.Path.StartsWithSegments("/health")
            ? LogEventLevel.Verbose
            : LogEventLevel.Information);

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("ready"),
});

var api = app.MapGroup("/api");
api.MapPlatformEndpoints();

// Unknown /api/* routes answer with the uniform Problem Details 404. Non-API paths stay free for the SPA (Month 2).
app.MapFallback("/api/{**path}", () => Error.NotFound("route.not_found", "The requested route does not exist.").ToProblemResult())
    .ExcludeFromDescription();

app.Run();
return 0;

public partial class Program;
