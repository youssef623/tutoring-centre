# tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs

## Purpose

Integration tests of the create-centre use case through the real dispatcher and PostgreSQL.

## Where It Fits

Infrastructure.Tests/Centres. Exercises validator, handler, domain, repository, unit of work, EF configuration and interceptor together.

## Walkthrough

Helper `ValidCommand(slug)`. Tests:
- `SendAsync_SystemActorValidCommand_PersistsCentreWithCreatedAtAndNoUpdatedAt`: Act - send as `SystemActor(null)`; Assert - success, slug returned, count 1, and raw SQL confirms `created_at is not null and updated_at is null` (proves the `TimestampInterceptor`).
- `..._SlugAlreadyExists_ReturnsConflictAndKeepsOneRow`: second send -> `centre.slug_taken`/Conflict, still 1 row.
- `..._AnonymousActor_ReturnsForbiddenAndWritesNothing`: `centre.create_forbidden`, 0 rows.
- `..._InvalidTimeZone_ReturnsDomainErrorAndWritesNothing`: `Mars/Base` -> `centre.time_zone_invalid`, 0 rows (handler returned failure -> rollback).
Not tested: validator failure path end to end, locale round trip, reading a centre back.

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Real dependencies instead of fakes.

#### How it works here

Entire class.

#### Why it matters here

Proves wiring and SQL, not just logic.

### Unit of Work and transactions

#### What it means

A *transaction* makes several database operations all-or-nothing and isolated from concurrent work. A *unit of work* groups the changes of one business operation and commits them together. EF Core's `DbContext` already tracks changes and is a unit of work; the repository adds a small port so the dispatcher can control the transaction without referencing EF.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#67-unit-of-work-and-transactions](../../../../PROJECT_OVERVIEW.md#67-unit-of-work-and-transactions).)

#### Where it appears in this file

Rollback on failure.

#### How it works here

Tests 3-4 assert 0 rows.

#### Why it matters here

Failure results leave no trace.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs`](../Fixtures/PostgresTestBase.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs`](../Fixtures/DispatchExtensions.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs`](../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs.md)
