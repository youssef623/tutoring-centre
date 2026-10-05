# tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs

## Purpose

Tests the session-revalidation rule: unknown user, wrong security stamp, inactive membership, and the two valid cases.

## Where It Fits

Application.Tests/Identity. Tests `ValidateStaffSessionHandler`, the Application half of what `SessionRevalidationHandler` calls on each (uncached) request.

## Walkthrough

Static `UserId` and `CentreId` (lines 9-10). Five `[Fact]`s, each building `new ValidateStaffSessionHandler(readService)` and a `ValidateStaffSessionQuery(UserId, stamp, centreId?)`:
- `HandleAsync_UnknownUser_ReturnsFalse` (12-26): `SessionState = null` -> success with value `false` (a *successful* query whose answer is 'not valid').
- `HandleAsync_WrongSecurityStamp_ReturnsFalse` (28-41): stored stamp `stamp-current`, cookie stamp `stamp-stale`.
- `HandleAsync_CentreSetAndMembershipInactive_ReturnsFalse` (43-56): stamp matches but `SessionStateDto("stamp-1", false)` and a centre is set.
- `HandleAsync_MatchingStampAndNoCentre_ReturnsTrue` (58-71): membership flag is irrelevant when no centre is selected.
- `HandleAsync_MatchingStampAndActiveCentreMembership_ReturnsTrue` (73-86).

## Concepts Used

### Session revalidation and revocation

#### What it means

A self-contained cookie cannot be cancelled by itself. Revalidation re-checks the cookie's claims against the database on requests (here via a *security stamp* and the active membership), so a changed password or revoked access ends the session; a short cache bounds the database cost.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation](../../../../PROJECT_OVERVIEW2.md#627-session-revalidation-and-revocation).)

#### Where it appears in this file

The handler rule.

#### How it works here

Each test varies stamp / centre / membership flag.

#### Why it matters here

Documents exactly when a session survives: stamp equal, and (if a centre is active) membership still active.

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

`SessionStateDto` canned values.

#### How it works here

`SessionState = new SessionStateDto(...)` in each test.

#### Why it matters here

No database is needed to test the rule; the SQL lives in `MembershipReadService`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs`](../../../src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs.md)
- [`src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionQuery.cs`](../../../src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionQuery.cs.md)
- [`tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs`](../Fakes/FakeMembershipReadService.cs.md)
- [`src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs`](../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs.md)
