# src/TutoringCentre.Api/Auth/SelectCentreRequest.cs

## Purpose

The request body of `POST /api/session/centre`: only the centre id the user asks to switch into.

## Where It Fits

Api/Auth. Bound by `AuthEndpoints.SelectCentreAsync`.

## Walkthrough

`public sealed record SelectCentreRequest(Guid CentreId);` Doc: the id is only a request - nothing is written into the session until `GetActiveMembershipQuery` confirms the current user belongs to that centre.

## Concepts Used

### Authorization by default, the actor pipeline and the tenant gate

#### What it means

A *fallback authorization policy* makes every endpoint require a signed-in user unless it explicitly opts out (fail closed). A single middleware translates the HTTP identity into the application's own `StaffActor`; the *tenant gate* is the one check that decides which centre a session may act in.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#629-authorization-by-default-the-actor-pipeline-and-the-tenant-gate](../../../../PROJECT_OVERVIEW2.md#629-authorization-by-default-the-actor-pipeline-and-the-tenant-gate).)

#### Where it appears in this file

Request vs authority.

#### How it works here

Doc comment.

#### Why it matters here

The client asserts which centre it *wants*, the server decides.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](AuthEndpoints.cs.md)
