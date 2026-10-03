# tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs

## Purpose

`IStartupFilter` that appends the test-only endpoints after the real middleware pipeline.

## Where It Fits

Api.Tests/Fixtures; registered by `ConventionsFactory`.

## Walkthrough

`Configure(Action<IApplicationBuilder> next)` returns a delegate that first runs `next(app)` (the application's own pipeline from `Program.cs`) and then `app.UseEndpoints(ConventionEndpoints.Map)`. Ordering ensures the test routes are subject to the real correlation, logging and exception middleware. CA1812 suppressed because DI instantiates it.

## Concepts Used

### Middleware and the request pipeline

#### What it means

Middleware components form a chain; each receives the request, may act before calling `next`, and may act again after it returns. Order is behaviour: a component only sees what earlier components set up and only wraps what comes after it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#8-routing-and-middleware](../../../../PROJECT_OVERVIEW.md#8-routing-and-middleware).)

#### Where it appears in this file

Startup filter.

#### How it works here

Method.

#### Why it matters here

Extends the pipeline without editing `Program.cs`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs`](ConventionEndpoints.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs`](ConventionsFactory.cs.md)
- [`src/TutoringCentre.Api/Program.cs`](../../../src/TutoringCentre.Api/Program.cs.md)
