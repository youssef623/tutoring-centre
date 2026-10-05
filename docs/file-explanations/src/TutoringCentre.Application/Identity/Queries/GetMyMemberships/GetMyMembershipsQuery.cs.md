# src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsQuery.cs

## Purpose

Asks for the signed-in staff member's own profile and active memberships.

## Where It Fits

Application/Identity/Queries/GetMyMemberships. Dispatched by `LoginAsync` and `GetMeAsync`.

## Walkthrough

`public sealed record GetMyMembershipsQuery : IQuery<MeDto>;` - no properties: 'my' queries take identity from the actor, so there is no id to tamper with.

## Concepts Used

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Parameterless query.

#### How it works here

Declaration.

#### Why it matters here

Identity from context, not input.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs`](GetMyMembershipsHandler.cs.md)
