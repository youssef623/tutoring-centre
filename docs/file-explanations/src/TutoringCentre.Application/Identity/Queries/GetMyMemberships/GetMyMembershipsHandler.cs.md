# src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs

## Purpose

Builds the signed-in user's `MeDto` from a freshly read profile, dropping the actor's selected centre if it is no longer an active membership.

## Where It Fits

Application/Identity/Queries/GetMyMemberships, `internal`. Depends on `ICurrentActor`, `IMembershipReadService`.

## Walkthrough

`HandleAsync` (16-44): not a `StaffActor` -> `Unauthenticated(auth.not_authenticated)`; `GetProfileAsync(staffActor.UserId)` null -> same failure; `activeMembership = staffActor.CentreId is {} centreId ? profile.Memberships.FirstOrDefault(m => m.CentreId == centreId) : null` (30-32); returns `MeDto` with `ActiveCentreId/ActiveRole` from that membership (null if stale). Doc: takes no user id from the caller.

## Concepts Used

### Authorization by default, the actor pipeline and the tenant gate

#### What it means

A *fallback authorization policy* makes every endpoint require a signed-in user unless it explicitly opts out (fail closed). A single middleware translates the HTTP identity into the application's own `StaffActor`; the *tenant gate* is the one check that decides which centre a session may act in.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#629-authorization-by-default-the-actor-pipeline-and-the-tenant-gate](../../../../../../PROJECT_OVERVIEW2.md#629-authorization-by-default-the-actor-pipeline-and-the-tenant-gate).)

#### Where it appears in this file

Fresh check of the selected centre.

#### How it works here

Lines 29-32.

#### Why it matters here

A stale cookie claim cannot report an active centre that the database no longer supports.

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Identity from the actor.

#### How it works here

Line 18.

#### Why it matters here

No tampering surface.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Identity/IMembershipReadService.cs`](../../IMembershipReadService.cs.md)
- [`src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs`](MeDto.cs.md)
- [`tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs`](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs.md)
