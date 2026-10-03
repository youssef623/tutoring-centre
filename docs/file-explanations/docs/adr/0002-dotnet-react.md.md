# docs/adr/0002-dotnet-react.md

## Purpose
ADR 0002 (Accepted, 2026-10-02). It chooses an **ASP.NET Core (.NET 10) + EF Core + PostgreSQL** backend with Clean Architecture and a hand-written CQRS pipeline, and a **React + TypeScript (Vite) SPA** served from the same origin as the API.

## Where it fits
Design documentation, linked from `README.md:93`. Its decisions show in `global.json`, `Directory.Packages.props`, `frontend/package.json` and `frontend/vite.config.ts` (the dev proxy).

## Walkthrough
- **Context (line 8):**
  - Career target: .NET backend roles.
  - A large domain (billing, attendance, scheduling, an AI assistant) needs static typing and a mature ORM.
  - Staff need a rich SPA.
  - One developer.
- **Decision (line 12):**
  - The backend stack above.
  - The SPA is served from the same origin in production and proxied in development ("see Day 3"), giving one deployable unit and no CORS.
- **Alternatives (lines 16–17):**
  - Node/TS end to end: rejected for career fit and ORM maturity.
  - Angular: rejected in favour of React, because TanStack Router/Query and shadcn/ui were already chosen, and for job-market fit.
- **Consequences (line 21):**
  - Two toolchains, so CI must build both.
  - From Day 12, an OpenAPI-generated client turns contract drift into a frontend type error.

## Concepts used
- **Same-origin deployment:** avoids CORS.
- **Generated API clients from OpenAPI.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Same-origin production serving is **not implemented**: `Program.cs` has no static-file middleware, and there is no Dockerfile.
- The OpenAPI client is not generated yet (Day 12).

## Related files
- [ADR 0001](0001-clean-architecture.md.md)
- [vite.config.ts](../../frontend/vite.config.ts.md)
- [frontend README](../../frontend/README.md.md)
