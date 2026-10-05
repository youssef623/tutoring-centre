# tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs

## Purpose

Proves the membership read service against real PostgreSQL: who sees which centres, that inactive members see none, and that nobody sees another user's memberships.

## Where It Fits

Infrastructure.Tests/Identity; tests `IMembershipReadService` as resolved from DI (`MembershipReadService`). Seeds the same staff shape as `SeedCommand`.

## Walkthrough

`SeedStaffAsync` (66-95): creates centres `nile-centre` and `maadi-hub` (via `CreateCentreCommand` as `SystemActor`), four users (owner@nile.test, teacher@both.test, secretary@nile.test, inactive@nile.test) and memberships: owner->Nile, teacher->Nile and Maadi, secretary->Nile, inactive secretary->Nile then `Deactivate()`d. Returns the private record `StaffIds`.
- `GetProfileAsync_TwoCentreTeacher_SeesBothCentres` (17-28): 2 memberships.
- `GetProfileAsync_Secretary_SeesOneCentre` (30-41).
- `GetProfileAsync_InactiveMember_SeesNoCentres` (43-54): profile exists, memberships empty.
- `GetActiveMembershipAsync_OwnerAskingForAForeignCentre_ReturnsNull` (56-66).
- `GetProfileAsync_NeverIncludesAnotherUsersMemberships` (68-80): owner sees exactly one membership, the Nile one.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services](../../../../PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

Read service tested with real SQL.

#### How it works here

Whole file.

#### Why it matters here

The fake in Application tests cannot test the `where user_id = ... and status = 'active'` filtering; this file does.

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Tenant-scoped reads.

#### How it works here

`GetActiveMembershipAsync_OwnerAskingForAForeignCentre_ReturnsNull` and `GetProfileAsync_NeverIncludesAnotherUsersMemberships`.

#### Why it matters here

The user id is the only access scope here; the tests show it is applied.

### Testcontainers and Respawn

#### What it means

Testcontainers starts a disposable Docker container (here PostgreSQL 17) for the test run. Respawn deletes rows between tests, which is much faster than recreating the database.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Seeding through the real model.

#### How it works here

`SeedStaffAsync`.

#### Why it matters here

Data is created via the domain factory `Membership.Create` and EF, so the test also exercises the mapping.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Needs Docker.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs`](../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs.md)
- [`src/TutoringCentre.Application/Identity/IMembershipReadService.cs`](../../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs.md)
- [`src/TutoringCentre.Domain/Identity/Membership.cs`](../../../src/TutoringCentre.Domain/Identity/Membership.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](../Fixtures/PostgresFixture.cs.md)
