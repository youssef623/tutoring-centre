# `tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs`

## Purpose

Declares `HealthEndpointTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 49 lines and 1,695 bytes. Source hash: `22d4d282b879de1ff9b5aae3cd0d308e84991107098b916c7409c7a6b5ecb58e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using Microsoft.AspNetCore.Mvc.Testing;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs#L6): `public sealed class HealthEndpointTests`
- [Line 15](../../../../../tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs#L15): `public async Task Liveness_ReturnsOk()`
- [Line 29](../../../../../tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs#L29): `public async Task Readiness_WhenDatabaseUnreachable_Returns503()`
- [Line 43](../../../../../tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs#L43): `private static WebApplicationFactory<Program> CreateFactory() =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TutoringCentre.Api.Tests.Health;

public sealed class HealthEndpointTests
{
    // Port 1 on loopback has nothing listening, so the connection is refused immediately;
    // Timeout=2 caps the wait on systems that retry before refusing. The value is present (startup validation
    // requires it) but unreachable (readiness must report 503).
    private const string UnreachableDatabase =
        "Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=2";

    [Fact]
    public async Task Liveness_ReturnsOk()
    {
        // Arrange
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Readiness_WhenDatabaseUnreachable_Returns503()
    {
        // Arrange
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/health/ready");

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    // "Testing" = no development auto-migration, no user-secrets, no dev seed: the test controls the configuration.
    private static WebApplicationFactory<Program> CreateFactory() =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting("ConnectionStrings:Postgres", UnreachableDatabase);
        });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
