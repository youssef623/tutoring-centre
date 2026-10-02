# ADR 0001: Clean Architecture

## Status

Accepted

## Context

This is a portfolio project aimed at .NET roles, so the architecture itself is part of what's being evaluated, not just the features. The same business logic has to serve three different front doors: a staff web app, a WhatsApp assistant, and background jobs (reminders, billing runs) — none of which should duplicate rules like "a payment can't be recorded against a cancelled enrollment." Those rules need to be testable without spinning up a database, both for fast feedback during development and because the AI assistant path depends on the same use cases behaving identically to the web path. The project is expected to grow over roughly eight months, so the structure needs to hold up as features accumulate, not just look clean on day one.

## Decision

Split the codebase into four projects — `TutoringCentre.Domain`, `TutoringCentre.Application`, `TutoringCentre.Infrastructure`, `TutoringCentre.Api` — with references allowed inward only:

- `Domain` references nothing.
- `Application` references `Domain`.
- `Infrastructure` references `Application` and `Domain` (it implements Application's interfaces — Dependency Inversion Principle).
- `Api` references `Application` and `Infrastructure`, and uses the `Infrastructure` reference only in its composition root (`Program.cs`) to register concrete services; that reference must never leak into endpoint code.

Within each layer, organize by feature rather than by technical type: `Application/Payments/Commands/RecordPayment/` holds the command, handler, and validator for that one use case together, instead of scattering them across `Commands/`, `Handlers/`, `Validators/` folders.

Module and layer boundaries are not just a convention — they're enforced by architecture tests running in CI (see Day 2/Day 4), so a reference that violates the allowed direction fails the build, not just a code review.

## Alternatives Considered

1. **Layered monolith without enforced boundaries.** Simpler to set up — fewer projects, no architecture tests to write. Rejected because the boundaries would depend entirely on developer discipline; nothing stops a controller from reaching straight into a repository, and that drift is exactly what this project needs to demonstrate it avoids.
2. **One project per module** (the original v1 plan). Gives compiler-enforced boundaries between modules (e.g., Payments can't see Enrollments' internals), but still needs layering inside each module project, and multiplies the project count — four layers × N modules. Rejected as more structure than a single-developer portfolio project needs right now; module boundaries are enforced with folders and architecture tests instead of separate assemblies.
3. **Microservices.** Would give independent deployability and failure isolation, but neither is needed for a single-developer, single-deployable system. The cost — distributed transactions, network calls between services, operational overhead of running and monitoring multiple deployments — isn't justified when one process can hold all four layers.

## Consequences

**Positive:**
- Dependency rules are executable, not just documented — a bad reference breaks the build.
- `Domain` and `Application` are testable in complete isolation from the database, HTTP, or any external system.
- The web app, the WhatsApp assistant, and background jobs all call the same Application use cases, so business rules can't drift between entry points.

**Negative:**
- Each feature now spans 4–6 files (command, handler, validator, maybe a response DTO and mapping) instead of living in one controller action.
- More indirection: following a single business operation means jumping across files and sometimes across projects.
- Mapping code between layers (e.g., entity to DTO) adds boilerplate that a single-project CRUD app wouldn't have.
- Estimated ~15% more backend hours than an unstructured approach, per the Architecture v2 "Complexity these patterns add" analysis — a cost accepted deliberately for what the structure demonstrates and for the testability it buys.
