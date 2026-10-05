# docs/adr/0001-clean-architecture.md

## Purpose

Architecture Decision Record explaining why the backend is four projects with inward-only dependencies, plus the alternatives that were rejected.

## Where It Fits

Documentation. Referenced by README, `OVERVIEW.semantic.md`, and code comments (`Dispatcher`). The decision it records is enforced by `tests/TutoringCentre.Architecture.Tests`.

## Walkthrough

Status Accepted. **Context:** portfolio project aimed at .NET roles; same business logic must serve a staff web app, a WhatsApp assistant and background jobs; rules must be testable without a database; ~8 months growth. **Decision:** `Domain` references nothing; `Application` -> `Domain`; `Infrastructure` -> `Application`+`Domain` (implements Application's interfaces); `Api` -> `Application`+`Infrastructure` (Infrastructure only in the composition root); feature folders inside layers (`Application/Payments/Commands/RecordPayment/`); boundaries enforced by architecture tests in CI. **Alternatives:** layered monolith without enforced boundaries; one project per module; microservices. **Consequences:** executable rules, isolated testability, shared use cases; but 4-6 files per feature, indirection, mapping boilerplate, ~15% more hours (from a document not in the repo). This is one of the few places the repo *states* its reasons.

## Concepts Used

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Whole ADR.

#### How it works here

Explains the rationale for ports/adapters and project boundaries.

#### Why it matters here

Provides documented (not inferred) reasons.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Mentions 'Day 2/Day 4' and an 'Architecture v2' analysis that are not in the repository.

## Related Files

- [`docs/adr/0002-dotnet-react.md`](0002-dotnet-react.md.md)
- [`README.md`](../../README.md.md)
- [`tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
- [`docs/architecture/overview.md`](../architecture/overview.md.md)
