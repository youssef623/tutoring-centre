# docs/adr/0002-dotnet-react.md

## Purpose

ADR recording the choice of ASP.NET Core + EF Core + PostgreSQL for the backend and a React + TypeScript Vite SPA for the frontend.

## Where It Fits

Documentation. Dated 2026-10-02. Explains why `frontend/` exists and what is planned for it.

## Walkthrough

Context: career target is .NET backend roles; large domain (billing, attendance, scheduling, AI assistant); money/access correctness favours a typed language and mature ORM; staff UI is an interactive SPA; one developer. Decision: ASP.NET Core (.NET 10) + EF Core + PostgreSQL, Clean Architecture, hand-written CQRS pipeline; React + TS + Vite SPA served from the same origin as the API in production and proxied in development. Alternatives: Node/TypeScript end to end; Angular. Consequences: two toolchains, both built in CI; a generated API client from OpenAPI 'starting Day 12'. **Code status:** the proxy exists (`frontend/vite.config.ts`); same-origin production serving and the generated client do not exist yet.

## Concepts Used

### Vite dev server, bundling and proxy

#### What it means

Vite serves source modules during development and bundles for production. Its dev proxy forwards chosen paths (`/api`, `/health`) to another server so the browser sees one origin and no CORS configuration is needed.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

The same-origin/proxy decision.

#### How it works here

Implemented only for development in `frontend/vite.config.ts`.

#### Why it matters here

Explains why no CORS is configured.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Plans described as decisions are not all implemented (see walkthrough).

## Related Files

- [`docs/adr/0001-clean-architecture.md`](0001-clean-architecture.md.md)
- [`frontend/vite.config.ts`](../../frontend/vite.config.ts.md)
- [`frontend/README.md`](../../frontend/README.md.md)
