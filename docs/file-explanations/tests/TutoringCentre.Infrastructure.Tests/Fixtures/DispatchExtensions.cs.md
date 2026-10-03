# tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs

## Purpose

Test helpers that dispatch a command or query in a fresh scope as a chosen actor.

## Where It Fits

Infrastructure.Tests/Fixtures. Used by most Infrastructure tests.

## Walkthrough

`SendAsAsync<TCommand,TResponse>(fixture|provider, actor, command)` and `QueryAsAsync<...>`: `await using var scope = provider.CreateAsyncScope()`; `scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor)`; resolve `Dispatcher`; call `SendAsync/QueryAsync` with `CancellationToken.None`. One scope per call mirrors one HTTP request or job. Overloads on `PostgresFixture` forward to the `IServiceProvider` overloads (so tests with custom providers can reuse them).

## Concepts Used

### Service lifetimes (singleton, scoped, transient)

#### What it means

Lifetime says how long a container-built instance lives. *Singleton*: one for the whole application. *Scoped*: one per scope; in ASP.NET one scope = one HTTP request, and code can create its own scope (as the seed command does). *Transient*: a new instance on every resolution. A longer-lived service must not hold a shorter-lived one (a "captive dependency"), which `ValidateScopes` detects.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning](../../../../PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Scope per call.

#### How it works here

`CreateAsyncScope`.

#### Why it matters here

Actor, DbContext and transaction are per dispatch.

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../PROJECT_OVERVIEW.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Setting the actor.

#### How it works here

`Set(actor)`.

#### Why it matters here

How tests simulate SystemActor/Anonymous.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](PostgresFixture.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
- [`src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs`](../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs.md)
