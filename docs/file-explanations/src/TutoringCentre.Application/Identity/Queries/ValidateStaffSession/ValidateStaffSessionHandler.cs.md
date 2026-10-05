# src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs

## Purpose

Decides if a session's security stamp and (when a centre is selected) membership are still current.

## Where It Fits

Application/Identity/Queries/ValidateStaffSession, `internal`. Depends on `IMembershipReadService` only (no actor).

## Walkthrough

`HandleAsync` (12-23): `state = await readService.GetSessionStateAsync(userId, centreId)`; `isValid = state is not null && state.SecurityStamp == query.SecurityStamp && state.MembershipActive`; returns `Result<bool>.Success(isValid)`. False for an unknown user, a changed stamp, or a centre whose membership is inactive; `GetSessionStateAsync` reports `MembershipActive` true when no centre is given (comment lines 16-17).

## Concepts Used

### Session revalidation and revocation

#### What it means

A self-contained cookie cannot be cancelled by itself. Revalidation re-checks the cookie's claims against the database on requests (here via a *security stamp* and the active membership), so a changed password or revoked access ends the session; a short cache bounds the database cost.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation](../../../../../../PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation).)

#### Where it appears in this file

The decision.

#### How it works here

Lines 18-20.

#### Why it matters here

Security stamp mismatch or inactive membership ends the session.

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Query returning `bool`.

#### How it works here

`IQueryHandler<..., bool>`.

#### Why it matters here

Fits the standard read pipeline.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

String comparison with `==` of stamps (not constant-time); the stamp is a server-side value compared to a value from the server's own encrypted cookie, not attacker-supplied input.

## Related Files

- [`src/TutoringCentre.Application/Identity/IMembershipReadService.cs`](../../IMembershipReadService.cs.md)
- [`tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs`](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs.md)
