# ARCHITECTURE.semantic.md

## Purpose
A generated (by semantic-git, line 1) architecture description of the backend **as of commit 42bdfc1 (Day 2)**. It covers why the system is layered, the four projects, why the Api→Infrastructure edge is "composition root only", the two-technique fitness functions, liveness vs readiness, configuration, and technology choices.

## Where it fits
Documentation. It complements `docs/adr/0001-clean-architecture.md` and is indexed by `.semantic-manifest.json`.

## Walkthrough
- **Lines 7–9:** the "three front doors" rationale.
- **Lines 11–32:** the projects and a diagram.
- **Lines 34–36:** why Api may reference Infrastructure only to call `AddInfrastructure`.
- **Lines 38–47:** enforcement. `ProjectReferenceTests` sees declared but unused references. `DependencyRuleTests` (NetArchTest) sees real IL usage. MSBuild rejects cycles (MSB4006).
- **Lines 49–76:** health checks. `/health` registers zero checks (`Predicate = _ => false`). `/health/ready` runs the `"ready"`-tagged Postgres check. A missing connection string registers an always-Unhealthy check instead of crashing. Includes a sequence diagram.
- **Lines 78–82:** configuration and secrets (empty key in `appsettings.json`, user-secrets, `.env`, Central Package Management).
- **Lines 84–91:** technology list.

## Concepts used
Clean Architecture, the Dependency Inversion Principle, fitness functions, and liveness/readiness probes. These are explained in `docs/PROJECT_OVERVIEW.md` §4.

## Data and control flow
Not applicable.

## Configuration and environment
Describes `ConnectionStrings:Postgres`.

## Gotchas and issues
- **Outdated.** Line 27 says Domain is "Currently empty" and line 28 says Application is "Currently empty". Line 29 says the readiness check is Infrastructure's "one real piece of logic". All three are false now: Domain has `Centre`/`Result`/`Error`; Application has `Dispatcher`; Infrastructure has EF Core, `UnitOfWork` and `SystemClock`.
- Line 87 says "no MVC/controllers yet". Still true.
- The file says "Do not edit manually". To fix it, regenerate it or remove it.

## Related files
- [.semantic-manifest.json](.semantic-manifest.json.md)
- [OVERVIEW.semantic.md](OVERVIEW.semantic.md.md)
- [ADR 0001](docs/adr/0001-clean-architecture.md.md)
