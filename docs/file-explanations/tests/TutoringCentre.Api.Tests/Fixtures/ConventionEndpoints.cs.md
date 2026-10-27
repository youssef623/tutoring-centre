# `tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Test-only endpoints. They exist only in the test host — never in the product or its OpenAPI document.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 2,292 bytes. Source hash: `d24342b03180e19981434a31631ef74ae9d5b1308e49dd8d529d0df3258798af`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Api.Http;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L8): `internal static class ConventionEndpoints`
- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L10): `public static void Map(IEndpointRouteBuilder endpoints)`
- [Line 35](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L35): `private static IResult LogSensitive(ILoggerFactory loggerFactory)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs:15](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Api.Http;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Test-only endpoints. They exist only in the test host — never in the product or its OpenAPI document.</summary>
internal static class ConventionEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        // These endpoints test HTTP/Problem Details conventions unrelated to authentication.
        var group = endpoints.MapGroup("/api/test").AllowAnonymous();

        group.MapGet("/{kind}", async (string kind, Dispatcher dispatcher, CancellationToken ct) =>
            (await dispatcher.SendAsync<ConventionCommand, string>(new ConventionCommand(kind), ct)).ToHttpResult(value => Results.Ok(value)));

        group.MapPost("/name", async (TestNameBody body, Dispatcher dispatcher, CancellationToken ct) =>
            (await dispatcher.SendAsync<TestNameCommand, string>(new TestNameCommand(body.Name), ct)).ToHttpResult(value => Results.Ok(value)));

        group.MapGet("/log-sensitive", LogSensitive);

        group.MapGet("/actor", async (Dispatcher dispatcher, CancellationToken ct) =>
            (await dispatcher.QueryAsync<CurrentActorQuery, CurrentActorDto>(new CurrentActorQuery(), ct)).ToHttpResult(dto => Results.Ok(dto)));

        // Deliberately mapped outside the anonymous /api/test group, with no authorization attribute at all —
        // proves the authorization fallback policy (Day 16, Task 16.5) protects a forgotten endpoint by default.
        endpoints.MapGet("/api/test-default-protection", () => Results.Ok());
    }

    [SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Test-only probe.")]
    [SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "Test-only probe with a fixed, cheap payload.")]
    private static IResult LogSensitive(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Conventions.LogProbe");
        logger.LogInformation("Login attempt {@Request}", new SensitiveProbe("sara@example.test", "hunter2", "+201001234567"));
        return Results.Ok();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
