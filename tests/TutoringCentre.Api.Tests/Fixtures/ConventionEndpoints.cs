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
