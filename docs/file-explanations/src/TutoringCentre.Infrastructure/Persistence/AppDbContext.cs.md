# src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs

## Purpose

The single EF Core database context for the whole application.

## Where It Fits

Infrastructure/Persistence. Registered by `AddDbContext` in `DependencyInjection.cs`; injected into `UnitOfWork`, `CentreRepository`, `SystemInfoReadService`, `MigrationRunner` and some tests.

## Walkthrough

`public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)` with one override: `OnModelCreating` -> `ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly)` - finds every `IEntityTypeConfiguration<T>` in this assembly (today `CentreConfiguration`). Doc: one transaction boundary across all modules; mapping in configuration classes so Domain has no persistence code; **no `DbSet` properties** - repositories use `Set<T>()`.

## Concepts Used

### ORM and EF Core

#### What it means

An ORM maps database rows to objects. EF Core keeps a *change tracker* that records the state of each entity (`Added`, `Unchanged`, `Modified`, `Deleted`); `SaveChanges` turns those states into `INSERT/UPDATE/DELETE`. LINQ expressions are translated to SQL by the provider and executed when awaited or enumerated (deferred execution). Mapping is declared in *model configuration*.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#69-ef-core-how-the-orm-actually-works-here](../../../../PROJECT_OVERVIEW.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

The DbContext.

#### How it works here

Class.

#### Why it matters here

Unit of change tracking per scope.

### Unit of Work and transactions

#### What it means

A *transaction* makes several database operations all-or-nothing and isolated from concurrent work. A *unit of work* groups the changes of one business operation and commits them together. EF Core's `DbContext` already tracks changes and is a unit of work; the repository adds a small port so the dispatcher can control the transaction without referencing EF.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#67-unit-of-work-and-transactions](../../../../PROJECT_OVERVIEW.md#67-unit-of-work-and-transactions).)

#### Where it appears in this file

One context = one transaction boundary.

#### How it works here

Doc comment.

#### Why it matters here

All modules in one database transaction.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Options come from `AddDbContext` (connection string, provider).

## Gotchas and Issues

Public only because other assemblies (tests, migrations) reference it. Entities appear in the model only if a configuration class exists for them or they are reachable from `Set<T>()` calls - a new entity needs a configuration class.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs`](Configurations/Centres/CentreConfiguration.cs.md)
- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](../DependencyInjection.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs`](UnitOfWork.cs.md)
- [`src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs`](../Repositories/CentreRepository.cs.md)
