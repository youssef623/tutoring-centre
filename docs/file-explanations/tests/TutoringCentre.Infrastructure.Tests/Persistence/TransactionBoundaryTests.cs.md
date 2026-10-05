# tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs

## Purpose

Proves rollback on exception and on failure results, with a success control.

## Where It Fits

Infrastructure.Tests/Persistence. Contains `WriteThenFailCommand`, `WriteThenFailMode`, `WriteThenFailHandler`.

## Walkthrough

The handler creates `doomed-centre`, `centres.Add`, then calls `db.SaveChangesAsync` itself (inside the dispatcher's open transaction, so the INSERT truly happens) and then, by mode, throws `InvalidOperationException`, returns `Error.Rule("test.rule_broken")`, or succeeds. Tests: Throw -> exception propagates and 0 rows; ReturnFailure -> failure with that code and 0 rows; Succeed -> 1 row (**control**: proves the other two could have observed a surviving row). Each builds a provider with `Register` adding the handler.

## Concepts Used

### Unit of Work and transactions

#### What it means

A *transaction* makes several database operations all-or-nothing and isolated from concurrent work. A *unit of work* groups the changes of one business operation and commits them together. EF Core's `DbContext` already tracks changes and is a unit of work; the repository adds a small port so the dispatcher can control the transaction without referencing EF.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#67-unit-of-work-and-transactions](../../../../PROJECT_OVERVIEW2.md#67-unit-of-work-and-transactions).)

#### Where it appears in this file

Transaction boundary.

#### How it works here

All three tests.

#### Why it matters here

Atomicity of handler work.

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Test-only handler registered via `configure`.

#### How it works here

`Register`.

#### Why it matters here

Production registrations untouched.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs`](../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](../Fixtures/PostgresFixture.cs.md)
