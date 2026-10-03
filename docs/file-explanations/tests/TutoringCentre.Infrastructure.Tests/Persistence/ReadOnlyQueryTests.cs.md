# tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs

## Purpose

Proves the database itself refuses writes made inside a query handler.

## Where It Fits

Infrastructure.Tests/Persistence. Contains the test-only `SneakyWriteQuery` and `SneakyWriteQueryHandler`.

## Walkthrough

Arrange: `Fixture.CreateServiceProvider(services => services.AddScoped<IQueryHandler<SneakyWriteQuery,string>, SneakyWriteQueryHandler>())`. The handler injects `AppDbContext` and runs raw SQL `insert into platform.centres (...) values (gen_random_uuid(), 'Sneaky', 'sneaky', ...)`. Act: `QueryAsAsync` and capture the exception (`Record.ExceptionAsync`). Assert: walking `InnerException`s finds a `PostgresException` with `SqlState == "25006"` (read_only_sql_transaction) and the table has 0 rows. This proves `UnitOfWork.BeginAsync(readOnly: true)` really issues `SET TRANSACTION READ ONLY`.

## Concepts Used

### Unit of Work and transactions

#### What it means

A *transaction* makes several database operations all-or-nothing and isolated from concurrent work. A *unit of work* groups the changes of one business operation and commits them together. EF Core's `DbContext` already tracks changes and is a unit of work; the repository adds a small port so the dispatcher can control the transaction without referencing EF.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#67-unit-of-work-and-transactions](../../../../PROJECT_OVERVIEW.md#67-unit-of-work-and-transactions).)

#### Where it appears in this file

Read-only transaction.

#### How it works here

Whole test.

#### Why it matters here

Database-enforced query purity.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The raw `ExecuteSqlRawAsync` call string has no user input, so no injection concern.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs`](../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
