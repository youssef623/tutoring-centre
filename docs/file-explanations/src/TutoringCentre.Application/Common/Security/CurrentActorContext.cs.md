# src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs

## Purpose

Holds the actor for one scope; starts anonymous and may be set exactly once.

## Where It Fits

Application/Common/Security. Registered scoped in `AddApplication`. `Set` is called by `SeedCommand` and test `DispatchExtensions`; **no HTTP code calls it**.

## Walkthrough

`Actor { get; private set; } = new AnonymousActor()` (line 11). `Set(Actor actor)` (15-26): `ArgumentNullException.ThrowIfNull`; if `_isSet` already true throws `InvalidOperationException` ('already been set and cannot be replaced'); else stores and flips `_isSet`. A second `Set` would indicate two pieces of edge code competing to define identity - treated as a bug.

## Concepts Used

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../../PROJECT_OVERVIEW.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

The class.

#### How it works here

`CurrentActorContext`.

#### Why it matters here

Gives each request/job one immutable-after-set identity.

### Service lifetimes (singleton, scoped, transient)

#### What it means

Lifetime says how long a container-built instance lives. *Singleton*: one for the whole application. *Scoped*: one per scope; in ASP.NET one scope = one HTTP request, and code can create its own scope (as the seed command does). *Transient*: a new instance on every resolution. A longer-lived service must not hold a shorter-lived one (a "captive dependency"), which `ValidateScopes` detects.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning](../../../../../PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Scoped holder.

#### How it works here

Registered `AddScoped`.

#### Why it matters here

Actor from one request cannot leak into another.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Over HTTP the actor is always `AnonymousActor` today because nothing sets it.

## Related Files

- [`src/TutoringCentre.Application/Common/Security/Actor.cs`](Actor.cs.md)
- [`src/TutoringCentre.Application/Common/Security/ICurrentActor.cs`](ICurrentActor.cs.md)
- [`tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs`](../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs.md)
- [`src/TutoringCentre.Api/Cli/SeedCommand.cs`](../../../TutoringCentre.Api/Cli/SeedCommand.cs.md)
