# tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs

## Purpose

Tests the handler behind `GET /api/me`: anonymous actor, unknown profile, and how the active centre/role are reported or dropped.

## Where It Fits

Application.Tests/Identity. Tests `GetMyMembershipsHandler`.

## Walkthrough

Four `[Fact]`s:
- `HandleAsync_AnonymousActor_ReturnsNotAuthenticated` (12-26).
- `HandleAsync_NoProfileForActor_ReturnsNotAuthenticated` (28-44): staff actor but `Profile = null` (the user row vanished) -> `auth.not_authenticated`.
- `HandleAsync_ActorCentreStillActive_ReportsActiveCentreAndRole` (46-69): actor `StaffActor(userId, centreId, StaffRole.Teacher)`, profile lists a matching `MembershipDto(centreId, "Nile Centre", "nile-centre", Teacher)` -> `ActiveCentreId == centreId`, `ActiveRole == Teacher`, one membership.
- `HandleAsync_ActorCentreNoLongerActive_DropsActiveCentreAndRole` (71-93): actor still claims `staleCentreId` but the freshly read profile has an empty membership list -> success with `ActiveCentreId` and `ActiveRole` both `null` (the handler does not trust the cookie's claim over the database).

## Concepts Used

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Stale actor claims.

#### How it works here

`HandleAsync_ActorCentreNoLongerActive_DropsActiveCentreAndRole`.

#### Why it matters here

The cookie's centre claim can be stale; the handler reconciles it with a fresh read.

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

Success with nulls instead of failure.

#### How it works here

The same test's assertions on `ActiveCentreId` and `ActiveRole`.

#### Why it matters here

Dropping a stale centre is a normal outcome (the user simply must pick a centre again), not an error.

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Canned `StaffProfileDto`.

#### How it works here

`Profile = new StaffProfileDto(...)` in the two `ActorCentre...` tests.

#### Why it matters here

Shows the DTO shapes the handler works on.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs`](../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs.md)
- [`src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs`](../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs.md)
- [`tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs`](../Fakes/FakeMembershipReadService.cs.md)
