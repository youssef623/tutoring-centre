# src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs

## Purpose

Port over the persistence transaction: begin (read-only or read-write), save, commit, rollback.

## Where It Fits

Application/Common/Ports. Called **only** by `Dispatcher`; implemented by `Infrastructure/Persistence/UnitOfWork`; faked by `FakeUnitOfWork` in tests.

## Walkthrough

Four methods. `BeginAsync(bool readOnly, CancellationToken)` (parameter name suppressed for CA1716 with a justification), `SaveChangesAsync`, `CommitAsync`, `RollbackAsync`. Doc: 'one transaction and at most one SaveChanges per command. Handlers never call it.'

## Concepts Used

### Unit of Work and transactions

#### What it means

A *transaction* makes several database operations all-or-nothing and isolated from concurrent work. A *unit of work* groups the changes of one business operation and commits them together. EF Core's `DbContext` already tracks changes and is a unit of work; the repository adds a small port so the dispatcher can control the transaction without referencing EF.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#67-unit-of-work-and-transactions](../../../../../PROJECT_OVERVIEW2.md#67-unit-of-work-and-transactions).)

#### Where it appears in this file

The interface.

#### How it works here

Declaration.

#### Why it matters here

Dispatcher can control transactions while Application stays free of EF.

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Port in Application, adapter in Infrastructure.

#### How it works here

`IUnitOfWork` vs `UnitOfWork`.

#### Why it matters here

Compile-time dependency points inward.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Convention-only: nothing prevents a handler from injecting `IUnitOfWork`; only review and the doc comment do.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs`](../../../TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../Cqrs/Dispatcher.cs.md)
- [`tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs`](../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs.md)
