# src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs

## Purpose

The tenant-gate question: may the signed-in staff member act in this centre?

## Where It Fits

Application/Identity/Queries/GetActiveMembership. Dispatched by `AuthEndpoints.SelectCentreAsync`; handled by `GetActiveMembershipHandler`.

## Walkthrough

`public sealed record GetActiveMembershipQuery(Guid CentreId) : IQuery<ActiveMembershipDto>;` - the user id is *not* in the query; the handler takes it from the actor.

## Concepts Used

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Query message.

#### How it works here

Declaration.

#### Why it matters here

Authorization question asked through the read pipeline.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs`](GetActiveMembershipHandler.cs.md)
- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](../../../../TutoringCentre.Api/Auth/AuthEndpoints.cs.md)
