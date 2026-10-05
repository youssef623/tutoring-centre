# src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs

## Purpose

The tenant gate: the only way a centre id enters a session.

## Where It Fits

Application/Identity/Queries/GetActiveMembership, `internal`. Depends on `ICurrentActor`, `IMembershipReadService`.

## Walkthrough

`HandleAsync` (17-32): actor not a `StaffActor` -> `Error.Unauthenticated("auth.not_authenticated")`; `readService.GetActiveMembershipAsync(staffActor.UserId, query.CentreId)` null -> `Error.Forbidden("tenant.no_membership", ...)`; else success with the `ActiveMembershipDto`. The same failure covers an unknown centre, a non-member and an inactive member, because `NotFound` would let a caller probe which centre ids exist (doc comment).

## Concepts Used

### Authorization by default, the actor pipeline and the tenant gate

#### What it means

A *fallback authorization policy* makes every endpoint require a signed-in user unless it explicitly opts out (fail closed). A single middleware translates the HTTP identity into the application's own `StaffActor`; the *tenant gate* is the one check that decides which centre a session may act in.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#629-authorization-by-default-the-actor-pipeline-and-the-tenant-gate](../../../../../../PROJECT_OVERVIEW2.md#629-authorization-by-default-the-actor-pipeline-and-the-tenant-gate).)

#### Where it appears in this file

Tenant gate.

#### How it works here

Lines 24-29.

#### Why it matters here

Membership check by the server before a centre is written into the cookie.

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Reads identity from the actor.

#### How it works here

`currentActor.Actor is not StaffActor`.

#### Why it matters here

No user id from request data.

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Query handler.

#### How it works here

Class.

#### Why it matters here

Runs in a read-only transaction.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The `auth.not_authenticated` branch is defensive: the authorization fallback policy already returns 401 before a handler runs for a missing session.

## Related Files

- [`src/TutoringCentre.Application/Identity/IMembershipReadService.cs`](../../IMembershipReadService.cs.md)
- [`src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs`](GetActiveMembershipQuery.cs.md)
- [`tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs`](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs.md)
- [`src/TutoringCentre.Application/Common/Security/Actor.cs`](../../../Common/Security/Actor.cs.md)
