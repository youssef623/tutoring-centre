# ARCHITECTURE.semantic.md

## Purpose

Generated architecture description focused on the dependency rule, architecture tests and health checks.

## Where It Fits

Root generated documentation. Complements `docs/adr/0001-clean-architecture.md`.

## Walkthrough

Sections: why this shape; the four projects with a Mermaid diagram; why `Api -> Infrastructure` is dashed; enforcement by fitness functions (declared-reference test and type-level test); liveness vs readiness with a sequence diagram; configuration/secrets; technology choices. **Still valid:** dependency rules, the two test techniques, health endpoint semantics. **Stale:** 'Domain/Application currently empty'; 'If the connection string is missing... Infrastructure registers a check that always reports Unhealthy rather than letting Npgsql throw at startup' - true for registration, but `DatabaseOptions` `ValidateOnStart` now stops the host from starting with an empty string; 'ASP.NET Core minimal hosting (no MVC/controllers yet)' remains true.

## Concepts Used

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../PROJECT_OVERVIEW.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

The layer explanation.

#### How it works here

Mirrors the README diagram.

#### Why it matters here

Good short explanation of why reverse references fail the build (circular reference).

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Stale in the places listed.

## Related Files

- [`OVERVIEW.semantic.md`](OVERVIEW.semantic.md.md)
- [`docs/adr/0001-clean-architecture.md`](docs/adr/0001-clean-architecture.md.md)
- [`tests/.semantic.md`](tests/.semantic.md.md)
- [`src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs`](src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs.md)
