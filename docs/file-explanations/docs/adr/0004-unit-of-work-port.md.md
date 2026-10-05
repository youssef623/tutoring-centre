# docs/adr/0004-unit-of-work-port.md

## Purpose

Architecture Decision Record: why Application defines a thin `IUnitOfWork` port that only the dispatcher calls.

## Where It Fits

Documentation (docs/adr). Explains `src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs` and its implementation `UnitOfWork` in Infrastructure; companion of ADR 0003.

## Walkthrough

Status Accepted, 2026-10-04. **Context:** Application must not reference EF Core (enforced by architecture tests) yet commands must save atomically and queries must be unable to write. **Decision:** `IUnitOfWork` with four operations - `BeginAsync(readOnly)`, `SaveChangesAsync`, `CommitAsync`, `RollbackAsync` - implemented over `AppDbContext` and a database transaction; only the dispatcher calls it; handlers and repositories never save (repositories track, the dispatcher saves once and commits); a read-only transaction issues `SET TRANSACTION READ ONLY`. **Alternatives:** handlers calling `SaveChanges` themselves; a repository-level unit of work; a generic repository exposing `IQueryable`. **Consequences:** one atomic commit per command (proven by `TransactionBoundaryTests`); queries read-only by construction (`ReadOnlyQueryTests`, SQLSTATE 25006); handlers unit-testable with fakes in `tests/TutoringCentre.Application.Tests/Fakes/`; the port is minimal and nested transactions are refused; future needs extend `BeginAsync`.

## Concepts Used

### Unit of Work and transactions

#### What it means

A *transaction* makes several database operations all-or-nothing and isolated from concurrent work. A *unit of work* groups the changes of one business operation and commits them together. EF Core's `DbContext` already tracks changes and is a unit of work; the repository adds a small port so the dispatcher can control the transaction without referencing EF.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#67-unit-of-work-and-transactions](../../../PROJECT_OVERVIEW2.md#67-unit-of-work-and-transactions).)

#### Where it appears in this file

The decision recorded.

#### How it works here

Decision section.

#### Why it matters here

Describes exactly the contract implemented by `UnitOfWork`.

### Architecture Decision Records and documentation as code

#### What it means

An ADR records a decision, its context, alternatives and consequences, so the *why* survives the people who made it. Docs kept in the repo are versioned with the code.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

ADR format.

#### How it works here

Whole file.

#### Why it matters here

Same structure as the other ADRs.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs`](../../src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs`](../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs`](../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs.md)
- [`docs/adr/0003-custom-cqrs-dispatcher.md`](0003-custom-cqrs-dispatcher.md.md)
