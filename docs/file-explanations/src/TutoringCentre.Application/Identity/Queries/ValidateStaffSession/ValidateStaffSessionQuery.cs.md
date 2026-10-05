# src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionQuery.cs

## Purpose

Asks whether an existing session is still valid; internal to the authentication pipeline.

## Where It Fits

Application/Identity/Queries/ValidateStaffSession. Dispatched only by `SessionRevalidationHandler`.

## Walkthrough

`public sealed record ValidateStaffSessionQuery(Guid UserId, string SecurityStamp, Guid? CentreId) : IQuery<bool>;` Doc: it takes the user id explicitly because it runs before an actor exists; no endpoint may expose it, because it would let a caller probe security stamps and membership state.

## Concepts Used

### Session revalidation and revocation

#### What it means

A self-contained cookie cannot be cancelled by itself. Revalidation re-checks the cookie's claims against the database on requests (here via a *security stamp* and the active membership), so a changed password or revoked access ends the session; a short cache bounds the database cost.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation](../../../../../../PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation).)

#### Where it appears in this file

The question asked per request.

#### How it works here

Declaration.

#### Why it matters here

Input = what the cookie claims.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The 'no endpoint may expose it' rule is a convention in a doc comment, not an architecture test.

## Related Files

- [`src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs`](ValidateStaffSessionHandler.cs.md)
- [`src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs`](../../../../TutoringCentre.Api/Auth/SessionRevalidationHandler.cs.md)
