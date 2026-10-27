# `src/TutoringCentre.Api/Program.cs`

## Purpose

The process composition root registers application and infrastructure services, JSON contracts, telemetry, authentication, and request protections. It selects migrate, seed, and reset-password CLI paths before HTTP execution, then installs the ordered middleware chain and maps all feature endpoint groups. Static files precede explicit routing; trusted forwarded headers run before scheme- and client-dependent behavior. Changes here affect every request and the build-time OpenAPI host.

## Where it fits

Category: **source**. [Open current file](../../../../src/TutoringCentre.Api/Program.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 252 lines and 11,690 bytes. Source hash: `8a22a3fce66ffc0651f89ef79b83891c665dd5f48a0316374ef3d8ecd4be17c9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Reflection;`
- Line 2: `using System.Text.Json;`
- Line 3: `using System.Text.Json.Serialization;`
- Line 4: `using Microsoft.AspNetCore.Diagnostics.HealthChecks;`
- Line 5: `using Microsoft.Extensions.Options;`
- Line 6: `using Microsoft.OpenApi;`
- Line 7: `using Serilog;`
- Line 8: `using Serilog.Events;`
- Line 9: `using Serilog.Formatting.Compact;`
- Line 10: `using TutoringCentre.Api.Auth;`
- Line 11: `using TutoringCentre.Api.Cli;`
- Line 12: `using TutoringCentre.Api.Endpoints;`
- Line 13: `using TutoringCentre.Api.Http;`
- Line 14: `using TutoringCentre.Api.Logging;`
- Line 15: `using TutoringCentre.Api.Telemetry;`
- Line 16: `using TutoringCentre.Application;`
- Line 17: `using TutoringCentre.Application.Platform;`
- Line 18: `using TutoringCentre.Domain.Common;`
- Line 19: `using TutoringCentre.Infrastructure;`
- Line 20: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 252](../../../../src/TutoringCentre.Api/Program.cs#L252): `public partial class Program;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs:21](../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L21)
- [tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs:43](../../../../tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs#L43)
- [tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs:28](../../../../tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs#L28)
- [tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs:55](../../../../tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs#L55)
- [tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs:34](../../../../tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs#L34)
- [tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs:13](../../../../tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Endpoints;
using TutoringCentre.Api.Http;
using TutoringCentre.Api.Logging;
using TutoringCentre.Api.Telemetry;
using TutoringCentre.Application;
using TutoringCentre.Application.Platform;
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

builder.Services.AddSingleton<IDemoModeProvider, DemoModeProvider>();

builder.Services.AddApiTelemetry(builder.Configuration);

builder.Services.AddHealthChecks();
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
builder.Services.AddApiProblemDetails();
builder.Services.AddApiAuthentication();
builder.Services.AddApiAntiforgery();
builder.Services.AddLoginRateLimiting(builder.Configuration);
builder.Services.AddChangePasswordRateLimiting();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Unknown members fail the request (400 request.malformed) instead of being silently ignored.
    options.SerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    // Local times of day travel as "HH:mm" only and instants leave as UTC with "Z" (Day 53); DateOnly's default
    // "yyyy-MM-dd" already matches the contract.
    options.SerializerOptions.Converters.Add(new HourMinuteTimeOnlyJsonConverter());
    options.SerializerOptions.Converters.Add(new UtcInstantJsonConverter());
});

// Outside Development, minimal APIs answer a bad request body with an empty 400 instead of throwing.
// Throwing in every environment lets GlobalExceptionHandler return the uniform `request.malformed` Problem Details (discrepancy D10).
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

builder.Services.AddOpenApi(options =>
```

The excerpt stops at line 90; use the original file for the remaining 162 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
