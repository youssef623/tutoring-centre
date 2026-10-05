# tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs

## Purpose

Tests liveness and readiness endpoints without a database.

## Where It Fits

Api.Tests/Health; builds its own `WebApplicationFactory<Program>` (not in a DB collection).

## Walkthrough

`UnreachableDatabase = "Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=2"` - value present (so startup validation passes) but unreachable; port 1 refuses immediately. `CreateFactory()` sets environment `Testing` and `UseSetting(...)`. `Liveness_ReturnsOk`: `GET /health` -> 200. `Readiness_WhenDatabaseUnreachable_Returns503`: `GET /health/ready` -> 503. Proves liveness never touches the DB and readiness reports failure.

## Concepts Used

### Health checks: liveness vs readiness

#### What it means

*Liveness* answers "is the process alive?"; *readiness* answers "can it serve traffic (dependencies up)?". Orchestrators restart on liveness failure and stop routing on readiness failure, so the two must not be conflated.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#614-options-validation-and-health-checks](../../../../PROJECT_OVERVIEW2.md#614-options-validation-and-health-checks).)

#### Where it appears in this file

Liveness vs readiness.

#### How it works here

Both tests.

#### Why it matters here

Outage must not look like a dead process.

### In-process hosting with WebApplicationFactory

#### What it means

`WebApplicationFactory<Program>` starts the real application inside the test process with an in-memory test server, so real middleware, DI and routing run without opening a network port. Tests can override configuration and services before the host is built.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Ad-hoc factory.

#### How it works here

`CreateFactory`.

#### Why it matters here

Own configuration per test.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Fake credentials in a connection string (`x`/`x`), not real secrets.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Program.cs`](../../../src/TutoringCentre.Api/Program.cs.md)
- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs.md)
- [`tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs`](ReadinessWithDatabaseTests.cs.md)
