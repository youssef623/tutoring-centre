# tests/TutoringCentre.Infrastructure.Tests/TutoringCentre.Infrastructure.Tests.csproj

## Purpose

Project file for database-backed Infrastructure tests.

## Where It Fits

tests/. Requires a Docker daemon at run time because of Testcontainers.

## Walkthrough

Every test project sets `IsPackable=false` (never published as a NuGet package), adds `<Using Include="Xunit" />` (a global using so test files need no `using Xunit;`), and references `coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit` and `xunit.runner.visualstudio` (with `IncludeAssets`/`PrivateAssets` so the runner is not flowed). Versions come from `Directory.Packages.props`. No code coverage step runs in CI even though `coverlet.collector` is referenced. Extras: `Microsoft.Extensions.Configuration`, `DependencyInjection`, `Logging` (build a real container in `PostgresFixture`), `Microsoft.Extensions.TimeProvider.Testing` (`FakeTimeProvider`), `Respawn`, `Testcontainers.PostgreSql`. References only `TutoringCentre.Infrastructure` (Application and Domain flow transitively).

## Concepts Used

### Testcontainers and Respawn

#### What it means

Testcontainers starts a disposable Docker container (here PostgreSQL 17) for the test run. Respawn deletes rows between tests, which is much faster than recreating the database.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Packages.

#### How it works here

Items.

#### Why it matters here

Start Postgres and reset rows per test.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](Fixtures/PostgresFixture.cs.md)
- [`src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj`](../../src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj.md)
