# docs/adr/0003-custom-cqrs-dispatcher.md

## Purpose

Architecture Decision Record: why the CQRS pipeline is a small hand-written `Dispatcher` instead of MediatR or decorators.

## Where It Fits

Documentation (docs/adr). Describes `src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`, whose behaviour is covered by `DispatcherTests` and `ReadOnlyQueryTests`. Linked from `docs/architecture/overview.md`.

## Walkthrough

Status Accepted, dated 2026-10-04. **Context:** every use case needs the same steps in a fixed order (validate, open the right transaction, run the handler, commit or roll back, log once); from Month 2 tenant resolution and permission checks join them and must be impossible to forget; handlers are called from several clients (HTTP, the seed CLI, later jobs and an assistant), so the steps cannot live in HTTP middleware. **Decision:** a small `Dispatcher` in Application with two pipelines - commands: validate, begin read-write transaction, handler, roll back on failure result, otherwise save once and commit, an exception rolls back and is rethrown to the global exception handler; queries: validate, begin read-only transaction, handler, commit, never save. Handlers are resolved by closed generic type, validators discovered by assembly scanning, handlers return `Result<T>`; tenant/permission steps will go between validation and `BeginAsync`. **Alternatives:** MediatR with behaviours (implicit pipeline, licensing change), decorators per handler (per-handler registration, order becomes configuration), calling services directly from endpoints (no shared pipeline). **Consequences:** about 100 lines to own; nine pipeline cases covered by unit tests (it names `DispatcherTests.cs`); call sites state generic arguments explicitly; pipeline order readable in one file; queries cannot write because PostgreSQL enforces `SET TRANSACTION READ ONLY` (proven by `ReadOnlyQueryTests`).

## Concepts Used

### Architecture Decision Records and documentation as code

#### What it means

An ADR records a decision, its context, alternatives and consequences, so the *why* survives the people who made it. Docs kept in the repo are versioned with the code.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

ADR format.

#### How it works here

Sections Context, Decision, Alternatives considered, Consequences.

#### Why it matters here

Records *why*, not just what; this is one of the few places the repository states its own reasons.

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

The decision recorded.

#### How it works here

Decision section.

#### Why it matters here

Matches the code in `Dispatcher.cs`; `CqrsRuleTests` (Architecture.Tests) now also enforces one handler per request.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The ADR says the tenant and permission steps 'will be added' - still true: they are comments in `Dispatcher.cs`, not code. 'Nine pipeline cases' is the document's count; I did not re-count the tests for this explanation.

## Related Files

- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
- [`tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs`](../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs`](../../tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs.md)
- [`docs/adr/0004-unit-of-work-port.md`](0004-unit-of-work-port.md.md)
- [`tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs`](../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs.md)
