# src/TutoringCentre.Application/Centres/ICentreRepository.cs

## Purpose

Write-side persistence port for the Centre aggregate.

## Where It Fits

Application/Centres. Used by `CreateCentreHandler`; implemented by `Infrastructure/Repositories/CentreRepository`; faked by `FakeCentreRepository`.

## Walkthrough

`Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct)` and `void Add(Centre centre)`. Doc: methods are named by use; nothing saves (dispatcher does). `Add` is synchronous because it only starts tracking.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services](../../../../PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

The interface.

#### How it works here

Declaration.

#### Why it matters here

Handler logic is testable with an in-memory fake.

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Port in Application.

#### How it works here

Namespace `TutoringCentre.Application.Centres`.

#### Why it matters here

EF stays in Infrastructure.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No `GetBySlug`/update methods yet.

## Related Files

- [`src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs`](../../TutoringCentre.Infrastructure/Repositories/CentreRepository.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](Commands/CreateCentre/CreateCentreHandler.cs.md)
- [`tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs`](../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs.md)
