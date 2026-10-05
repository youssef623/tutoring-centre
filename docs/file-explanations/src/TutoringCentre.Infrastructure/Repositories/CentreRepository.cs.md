# src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs

## Purpose

EF Core implementation of `ICentreRepository`.

## Where It Fits

Infrastructure/Repositories, `internal`, scoped. Depends on `AppDbContext`; consumed by `CreateCentreHandler` through the Application port.

## Walkthrough

Primary-constructor `CentreRepository(AppDbContext db)`. `ExistsBySlugAsync` => `db.Set<Centre>().AnyAsync(centre => centre.Slug == slug, ct)` (an `EXISTS` query on `platform.centres`, executed when awaited, inside the dispatcher's transaction). `Add` => null-guard then `db.Set<Centre>().Add(centre)` (marks `Added`; no SQL until `SaveChanges`). Same `AppDbContext` instance as the `UnitOfWork` because both are scoped in one scope.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services](../../../../PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

The class.

#### How it works here

Whole file.

#### Why it matters here

Hides EF behind the port.

### ORM and EF Core

#### What it means

An ORM maps database rows to objects. EF Core keeps a *change tracker* that records the state of each entity (`Added`, `Unchanged`, `Modified`, `Deleted`); `SaveChanges` turns those states into `INSERT/UPDATE/DELETE`. LINQ expressions are translated to SQL by the provider and executed when awaited or enumerated (deferred execution). Mapping is declared in *model configuration*.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

`Set<T>()`, `AnyAsync`, `Add`.

#### How it works here

Both methods.

#### Why it matters here

Query translation and change tracking in action.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No `SaveChanges` here by design.

## Related Files

- [`src/TutoringCentre.Application/Centres/ICentreRepository.cs`](../../TutoringCentre.Application/Centres/ICentreRepository.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs`](../Persistence/AppDbContext.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](../../TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs.md)
