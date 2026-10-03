# tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs

## Purpose

Tests that readiness returns 200 when a real database is reachable.

## Where It Fits

Api.Tests/Health; collection `api`.

## Walkthrough

`InitializeAsync` resets the DB. `GetReady_WithReachableDatabase_Returns200`: `factory.CreateClient()`, `GET /health/ready`, assert 200. Pairs with the 503 test.

## Concepts Used

### Health checks: liveness vs readiness

#### What it means

*Liveness* answers "is the process alive?"; *readiness* answers "can it serve traffic (dependencies up)?". Orchestrators restart on liveness failure and stop routing on readiness failure, so the two must not be conflated.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#614-options-validation-and-health-checks](../../../../PROJECT_OVERVIEW.md#614-options-validation-and-health-checks).)

#### Where it appears in this file

Readiness success.

#### How it works here

Test.

#### Why it matters here

Completes the readiness contract.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs`](../Fixtures/ApiFactory.cs.md)
- [`tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs`](HealthEndpointTests.cs.md)
