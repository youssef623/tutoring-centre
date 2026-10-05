# src/TutoringCentre.Application/Common/Security/Actor.cs

## Purpose

Defines who is executing a use case: the abstract `Actor` and its kinds `SystemActor`, `AnonymousActor` and `StaffActor`.

## Where It Fits

Application/Common/Security. Read through `ICurrentActor` by handlers (`CreateCentreHandler`, `GetMyMembershipsHandler`, `GetActiveMembershipHandler`); set by `SeedCommand`, by `ActorMiddleware` (from the session claims) and by `CurrentActorContext.Reauthenticate` at login; test helpers create them directly.

## Walkthrough

`abstract record Actor { abstract Guid? CentreId { get; } }` (9-13). `SystemActor(Guid? CentreId) : Actor` (16-19) with `public override Guid? CentreId { get; } = CentreId;` - the positional parameter initialises the override property. `AnonymousActor : Actor` (22-25) with `CentreId => null`. New: `StaffActor(Guid UserId, Guid? CentreId, StaffRole? Role) : Actor` (31-34), a signed-in staff member; `CentreId` is null right after login (before a centre is chosen) and `Role` is the role in that centre and null whenever `CentreId` is null (doc comment). It imports `TutoringCentre.Domain.Identity` for `StaffRole`. Handlers never receive identity or tenant from request data.

## Concepts Used

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

The actor hierarchy.

#### How it works here

Whole file.

#### Why it matters here

Authorization and tenancy read the same object everywhere (HTTP, CLI, tests).

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Positional records with an overridden property.

#### How it works here

Lines 16-19 and 31-34.

#### Why it matters here

The positional parameter `CentreId` initialises the overriding property so the abstract member is satisfied.

### Identity and membership modelling

#### What it means

*Identity* is the part of a system that stores users, credentials and lockout state. *Membership* models which user may act in which centre in which role. Keeping role on the membership (not on the user) lets one person be an owner in one centre and a teacher in another.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling](../../../../../PROJECT_OVERVIEW2.md#628-identity-and-membership-modelling).)

#### Where it appears in this file

Staff identity in Application.

#### How it works here

`StaffActor`.

#### Why it matters here

Application knows *who* (user id) and *where/as what* (centre, role) without knowing cookies or ASP.NET Core Identity.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

`SystemActor.CentreId` and `StaffActor.CentreId` are read by `GetActiveMembershipHandler`-style checks, but no repository or query is filtered by `Actor.CentreId` yet (tenant filtering is planned for Month 2); roles are carried but not enforced by any permission check.

## Related Files

- [`src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs`](CurrentActorContext.cs.md)
- [`src/TutoringCentre.Application/Common/Security/ICurrentActor.cs`](ICurrentActor.cs.md)
- [`src/TutoringCentre.Domain/Identity/StaffRole.cs`](../../../TutoringCentre.Domain/Identity/StaffRole.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](../../Centres/Commands/CreateCentre/CreateCentreHandler.cs.md)
- [`src/TutoringCentre.Api/Auth/ActorMiddleware.cs`](../../../TutoringCentre.Api/Auth/ActorMiddleware.cs.md)
