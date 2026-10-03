# src/TutoringCentre.Application/Common/Security/Actor.cs

## Purpose

Defines who is executing a use case: the abstract `Actor`, the `SystemActor` and the `AnonymousActor`.

## Where It Fits

Application/Common/Security. Read through `ICurrentActor` by handlers (`CreateCentreHandler`); set by `SeedCommand` and test helpers.

## Walkthrough

`abstract record Actor { abstract Guid? CentreId { get; } }`. `SystemActor(Guid? CentreId) : Actor` with `public override Guid? CentreId { get; } = CentreId;` - the positional parameter initialises the override property. `AnonymousActor : Actor` with `CentreId => null`. Doc: handlers never receive identity or tenant from request data. There is no user/staff/parent actor yet.

## Concepts Used

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../../PROJECT_OVERVIEW.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

All three types.

#### How it works here

Declarations.

#### Why it matters here

`is SystemActor` is the only authorization rule today.

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Record hierarchy.

#### How it works here

`abstract record` + sealed derived records.

#### Why it matters here

Value equality and `is` pattern matching.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

`CentreId` is never read by any code (planned for tenant scoping).

## Related Files

- [`src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs`](CurrentActorContext.cs.md)
- [`src/TutoringCentre.Application/Common/Security/ICurrentActor.cs`](ICurrentActor.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](../../Centres/Commands/CreateCentre/CreateCentreHandler.cs.md)
- [`src/TutoringCentre.Api/Cli/SeedCommand.cs`](../../../TutoringCentre.Api/Cli/SeedCommand.cs.md)
