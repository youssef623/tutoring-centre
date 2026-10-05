# src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs

## Purpose

Holds the actor for one scope; starts anonymous, may be set exactly once, and may be replaced only through `Reauthenticate` at login.

## Where It Fits

Application/Common/Security. Registered scoped in `AddApplication`. `Set` is called by `ActorMiddleware`, `SeedCommand` and test `DispatchExtensions`; `Reauthenticate` by `AuthEndpoints.LoginAsync`.

## Walkthrough

`Actor { get; private set; } = new AnonymousActor()` (line 11). `Set(Actor actor)` (15-26): `ArgumentNullException.ThrowIfNull`; if `_isSet` already true throws `InvalidOperationException` ('already been set and cannot be replaced'); else stores and flips `_isSet`. A second `Set` would indicate two pieces of edge code competing to define identity - treated as a bug. New `Reauthenticate(StaffActor actor)` (34-40): null-checks, then assigns `Actor` and sets `_isSet = true` *without* the already-set check. Its doc comment explains why: a request can arrive with an existing session cookie (so `ActorMiddleware` has already called `Set`), then log in as a different user; the new identity must supersede the old one. After `Reauthenticate`, a later `Set` still throws (tested).

## Concepts Used

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Set-once with one controlled exception.

#### How it works here

`Set` and `Reauthenticate`.

#### Why it matters here

`Set` keeps the guard against accidental double assignment; `Reauthenticate` is the narrow, typed (`StaffActor` only) way to replace the actor.

### Service lifetimes (singleton, scoped, transient)

#### What it means

Lifetime says how long a container-built instance lives. *Singleton*: one for the whole application. *Scoped*: one per scope; in ASP.NET one scope = one HTTP request, and code can create its own scope (as the seed command does). *Transient*: a new instance on every resolution. A longer-lived service must not hold a shorter-lived one (a "captive dependency"), which `ValidateScopes` detects.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning](../../../../../PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Scoped holder.

#### How it works here

Registered scoped.

#### Why it matters here

One instance per request, shared by middleware and handlers through `ICurrentActor`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Only login uses `Reauthenticate`; the type system limits it to `StaffActor`, so it cannot be used to impersonate a system actor.

## Related Files

- [`src/TutoringCentre.Application/Common/Security/Actor.cs`](Actor.cs.md)
- [`src/TutoringCentre.Application/Common/Security/ICurrentActor.cs`](ICurrentActor.cs.md)
- [`tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs`](../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs.md)
- [`src/TutoringCentre.Api/Auth/ActorMiddleware.cs`](../../../TutoringCentre.Api/Auth/ActorMiddleware.cs.md)
- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](../../../TutoringCentre.Api/Auth/AuthEndpoints.cs.md)
