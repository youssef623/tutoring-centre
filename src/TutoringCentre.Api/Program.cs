using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Endpoints;
using TutoringCentre.Api.Http;
using TutoringCentre.Api.Logging;
using TutoringCentre.Application;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Build-time OpenAPI generation (Microsoft.Extensions.ApiDescription.Server) runs this entry point inside GetDocument.Insider.
// Startup side effects must not run there, but every endpoint must still be registered or the document comes out empty.
var isDocumentGeneration = Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";
if (isDocumentGeneration)
{
    // Satisfies fail-fast options validation without a real database or proxy; nothing connects or listens
    // during generation, and this entry point runs with no ASPNETCORE_ENVIRONMENT set — the framework's own
    // default, Production — so Task 34.3's "Production needs a non-empty Proxy:KnownNetworks" check would
    // otherwise fail the build itself.
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:Postgres"] = "Host=localhost;Database=openapi_generation",
        ["Proxy:KnownNetworks:0"] = "127.0.0.1/32",
    });
}

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

// Short to start (raised once DNS/certificates are proven stable in Azure — Day 36); Production only, since
// HSTS instructs the *browser* to remember "always HTTPS" for this host, which would break a plain-http
// Development run for the length of the max-age.
builder.Services.AddHsts(options => options.MaxAge = TimeSpan.FromMinutes(5));

builder.Services.AddProxyOptions(builder.Configuration);

// Bound and validated at start like every other option here, even though a bare bool has nothing to fail on:
// a misconfigured container fails at start, not at first request, as a rule for every setting, not case by case.
builder.Services.AddOptions<DemoOptions>().Bind(builder.Configuration.GetSection("Demo")).ValidateOnStart();

builder.Services.AddHealthChecks();
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
builder.Services.AddApiProblemDetails();
builder.Services.AddApiAuthentication();
builder.Services.AddApiAntiforgery();
builder.Services.AddLoginRateLimiting();
builder.Services.AddChangePasswordRateLimiting();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Unknown members fail the request (400 request.malformed) instead of being silently ignored.
    options.SerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

// Outside Development, minimal APIs answer a bad request body with an empty 400 instead of throwing.
// Throwing in every environment lets GlobalExceptionHandler return the uniform `request.malformed` Problem Details (discrepancy D10).
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer((document, _, _) =>
    {
        // Identical document on every machine and in CI: no host-specific server URLs, so the staleness check is reliable.
        document.Servers = [];
        return Task.CompletedTask;
    }));

var app = builder.Build();

// CLI mode: `dotnet run --project src/TutoringCentre.Api -- migrate`. The deploy pipeline's own step — the
// running web process never migrates itself in Production (see below).
if (args is ["migrate"])
{
    return await MigrateCommand.RunAsync(app.Services);
}

// CLI mode: `dotnet run --project src/TutoringCentre.Api -- seed`. Gated (Task 34.7): Development and Testing
// always allow it; Production only when Demo:Enabled opts a throwaway instance in. Seeding also still requires
// Seed:Password (DevelopmentIdentitySeeder's own check, unchanged) even once the gate is passed.
if (args is ["seed"])
{
    var demoOptions = app.Services.GetRequiredService<IOptions<DemoOptions>>().Value;
    if (!SeedGate.IsAllowed(app.Environment, demoOptions))
    {
        await Console.Error.WriteLineAsync(
            "Refused: seed only runs in Development, Testing, or when Demo:Enabled is true in configuration.");
        return 1;
    }

    await app.Services.ApplyMigrationsAsync();
    return await SeedCommand.RunAsync(app.Services);
}

// Development convenience only. Production migrations run from the deployment pipeline (Month 2), never at app startup:
// auto-migrating there is risky (several instances racing, no review, long locks).
if (app.Environment.IsDevelopment() && !isDocumentGeneration)
{
    await app.Services.ApplyMigrationsAsync();
}

// Absolutely first: every other middleware (security headers' HSTS/Production check, request logging's client
// address, rate limiting's partition key) must see the real scheme and client address, not the proxy's own.
app.UseTrustedForwardedHeaders();

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseSecurityHeaders();

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

app.UseSpaStaticFiles();

// Explicit, and positioned here rather than left to WebApplication's automatic insertion: routing must not
// select an endpoint for a request before static files has had a chance to serve it. WebApplication's implicit
// UseRouting() runs before any of this file's `Use*` calls (not "right before the first Map call" as the
// no-explicit-call docs can read); without this explicit call, a fallback route can win against an existing
// physical file, since endpoint SELECTION (routing) happens independently of whether a file exists on disk.
app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<ActorMiddleware>();
app.UseAuthorization();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    // Local tooling only; this endpoint never exists outside Development, so anonymous access here is harmless.
    app.MapOpenApi().AllowAnonymous();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false,
}).AllowAnonymous();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("ready"),
}).AllowAnonymous();

var api = app.MapGroup("/api").AddEndpointFilter<AntiforgeryEndpointFilter>();
api.MapPlatformEndpoints();
api.MapAuthEndpoints();
api.MapSubjectEndpoints();
api.MapStaffEndpoints();
api.MapAuditEndpoints();
api.MapCentreSettingsEndpoints();

// Unknown /api/* routes answer with the uniform Problem Details 404, never the SPA's HTML shell.
// Anonymous: a signed-out caller probing an unknown route must see the same 404 as anyone else, not a 401.
app.MapFallback("/api/{**path}", () => Error.NotFound("route.not_found", "The requested route does not exist.").ToProblemResult())
    .ExcludeFromDescription()
    .AllowAnonymous();

// Falls back to index.html for any other GET, so deep links work. A no-op when no build is present
// (Development, where the Vite dev server serves the frontend).
app.MapSpaFallback();

// Whatever the above didn't handle (no build present, or a file-looking path — e.g. a missing /assets/*.js —
// that the SPA fallback deliberately excludes via its own "nonfile" route constraint) gets the same anonymous
// 404 rather than first demanding a session. The explicit unconstrained pattern matters here: the zero-arg
// MapFallback overload defaults to that same "nonfile" constraint, under which a file-looking path matches no
// endpoint at all — and the authorization fallback policy enforces RequireAuthenticatedUser() even then, so
// an unmatched path would 401 instead of reaching this 404. Static files above still wins for any path that
// is an actual existing file, because it runs before routing (and so before any endpoint is even selected).
app.MapFallback("/{**path}", () => Error.NotFound("route.not_found", "The requested route does not exist.").ToProblemResult())
    .ExcludeFromDescription()
    .AllowAnonymous();

app.Run();
return 0;

public partial class Program;
