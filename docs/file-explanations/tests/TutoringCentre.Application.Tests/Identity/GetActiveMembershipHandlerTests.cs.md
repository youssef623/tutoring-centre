# tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs

## Purpose

Tests the tenant gate query handler: anonymous actor, no active membership, active membership.

## Where It Fits

Application.Tests/Identity. Tests `GetActiveMembershipHandler` with a real `CurrentActorContext` and `FakeMembershipReadService`.

## Walkthrough

Three `[Fact]`s, each constructing `new GetActiveMembershipHandler(actorContext, readService)`:
- `HandleAsync_AnonymousActor_ReturnsNotAuthenticated` (12-26): default context (anonymous) -> failure `auth.not_authenticated`, `ErrorKind.Unauthenticated`.
- `HandleAsync_NoActiveMembershipInCentre_ReturnsTenantNoMembership` (28-44): actor is `new StaffActor(userId, null, null)`, fake returns `null` -> failure `tenant.no_membership`, `ErrorKind.Forbidden`. Comment: one `null` covers unknown centre, non-member and inactive member alike, which is deliberate (no information about which case).
- `HandleAsync_ActiveMembershipInCentre_Succeeds` (46-66): fake returns `new ActiveMembershipDto(centreId, "Nile Centre", StaffRole.Owner)` -> success with `CentreId` and `Role` copied through.

## Concepts Used

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Actor kinds in tests.

#### How it works here

The tests build the anonymous actor (a fresh `CurrentActorContext` left unset) and staff actors (`actorContext.Set(new StaffActor(...))`) by hand.

#### Why it matters here

The handler's first check is 'is the actor a `StaffActor`'; tests show both branches.

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

Asserting on `Result`.

#### How it works here

`result.IsFailure`, `result.Error!.Code`, `result.Error.Kind`.

#### Why it matters here

Failures are values with a stable code, so tests assert the code, not an exception.

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

`FakeMembershipReadService`.

#### How it works here

`new FakeMembershipReadService()` or `{ ActiveMembership = ... }` in each test.

#### Why it matters here

Handler logic is tested in isolation from PostgreSQL.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs`](../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs.md)
- [`tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs`](../Fakes/FakeMembershipReadService.cs.md)
