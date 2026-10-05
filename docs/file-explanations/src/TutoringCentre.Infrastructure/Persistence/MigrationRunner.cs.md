# src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs

## Purpose

Extension method that applies pending EF Core migrations from an `IServiceProvider`.

## Where It Fits

Infrastructure/Persistence. Called by `Program.cs` (Development startup and `seed` mode), `ApiFactory`, `PostgresFixture`.

## Walkthrough

`ApplyMigrationsAsync(this IServiceProvider services, CancellationToken ct = default)`: `await using var scope = services.CreateAsyncScope()`; resolves `AppDbContext`; `await db.Database.MigrateAsync(ct)`. A *new scope* is required because the context is scoped; the root provider cannot resolve it safely. `MigrateAsync` creates the database if absent, creates `__ef_migrations_history`, and runs unapplied migrations in order.

## Concepts Used

### Database migrations

#### What it means

A migration is versioned code describing one schema change with an `Up` (apply) and `Down` (revert). EF records applied migrations in a history table and keeps a *model snapshot* of the last known model; the next `migrations add` diffs the current model against it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

Applying migrations.

#### How it works here

`MigrateAsync`.

#### Why it matters here

Brings an empty database to the current schema.

### Service lifetimes (singleton, scoped, transient)

#### What it means

Lifetime says how long a container-built instance lives. *Singleton*: one for the whole application. *Scoped*: one per scope; in ASP.NET one scope = one HTTP request, and code can create its own scope (as the seed command does). *Transient*: a new instance on every resolution. A longer-lived service must not hold a shorter-lived one (a "captive dependency"), which `ValidateScopes` detects.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning](../../../../PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Own scope.

#### How it works here

`CreateAsyncScope`.

#### Why it matters here

Scoped services need a scope.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Doc says 'Development and seed only' - test fixtures also call it. Running it automatically in production is explicitly discouraged in `Program.cs` comments.

## Related Files

- [`src/TutoringCentre.Api/Program.cs`](../../TutoringCentre.Api/Program.cs.md)
- [`src/TutoringCentre.Api/Cli/SeedCommand.cs`](../../TutoringCentre.Api/Cli/SeedCommand.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs.md)
