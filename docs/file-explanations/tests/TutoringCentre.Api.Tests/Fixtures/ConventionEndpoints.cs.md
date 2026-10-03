# tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs

## Purpose

Defines the test-only `/api/test/*` endpoints used to trigger each error kind, strict-JSON failures and sensitive-data logging.

## Where It Fits

Api.Tests/Fixtures. Never part of the product or its OpenAPI document.

## Walkthrough

`Map(IEndpointRouteBuilder)`: group `/api/test`; `GET /{kind}` dispatches `ConventionCommand(kind)` and returns `.ToHttpResult(Results.Ok)`; `POST /name` binds `TestNameBody` and dispatches `TestNameCommand`; `GET /log-sensitive` logs `"Login attempt {@Request}"` with a `SensitiveProbe("sara@example.test","hunter2","+201001234567")` to prove masking (the endpoint exists; the tests that call it are not present in the suite I read - no test hits `/api/test/log-sensitive`). Uses the same `ToHttpResult` path as real endpoints, so conventions are tested for real.

## Concepts Used

### Minimal APIs, routing and route groups

#### What it means

Minimal APIs map a URL pattern and HTTP verb straight to a delegate (`MapGet("/x", handler)`). The framework binds delegate parameters from DI (services), the route, query, body or `CancellationToken`. A *route group* shares a prefix and metadata across endpoints.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Test routes.

#### How it works here

`MapGroup`, `MapGet`, `MapPost`.

#### Why it matters here

Same mechanics as product endpoints.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

`/api/test/log-sensitive` appears unused by any test (searched for the path string).

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs`](ConventionTestTypes.cs.md)
- [`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`](../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs.md)
- [`tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs`](../Http/ProblemDetailsTests.cs.md)
