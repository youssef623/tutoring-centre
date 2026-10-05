# tests/TutoringCentre.Api.Tests/TutoringCentre.Api.Tests.csproj

## Purpose

Project file for HTTP-level tests that host the real Api in-process.

## Where It Fits

tests/. Uses `Microsoft.NET.Sdk.Web` (unlike the others) because `WebApplicationFactory` needs the web SDK's content-root conventions.

## Walkthrough

Every test project sets `IsPackable=false` (never published as a NuGet package), adds `<Using Include="Xunit" />` (a global using so test files need no `using Xunit;`), and references `coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit` and `xunit.runner.visualstudio` (with `IncludeAssets`/`PrivateAssets` so the runner is not flowed). Versions come from `Directory.Packages.props`. No code coverage step runs in CI even though `coverlet.collector` is referenced. Extras: `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`), `Respawn`, `Testcontainers.PostgreSql`; references `TutoringCentre.Api`.

## Concepts Used

### In-process hosting with WebApplicationFactory

#### What it means

`WebApplicationFactory<Program>` starts the real application inside the test process with an in-memory test server, so real middleware, DI and routing run without opening a network port. Tests can override configuration and services before the host is built.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Mvc.Testing package.

#### How it works here

Item.

#### Why it matters here

Hosts `Program` in memory.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs`](Fixtures/ApiFactory.cs.md)
- [`src/TutoringCentre.Api/TutoringCentre.Api.csproj`](../../src/TutoringCentre.Api/TutoringCentre.Api.csproj.md)
