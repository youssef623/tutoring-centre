# README.md

## Purpose

Human entry point: what the product is meant to be, how to run it locally, the architecture diagram and the quality gates.

## Where It Fits

Root documentation. Links to `docs/adr/0001-clean-architecture.md`, `docs/adr/0002-dotnet-react.md` and `frontend/README.md`. Describes the commands that match `launchSettings.json` (port 5080) and `vite.config.ts` (proxy to 5080).

## Walkthrough

Sections: title + CI badge; one-paragraph product description (multi-tenant platform for Egyptian tutoring centres: students/parents, groups/schedules, attendance, fees, WhatsApp assistant) - this is the **intended** scope, not what exists; **Quick start** (prerequisites .NET 10 SDK, Node 22.12+, Docker, Git; six numbered steps: clone, `.env`/`docker compose up -d`, user-secrets connection string, `dotnet run ... --launch-profile http`, `npm ci && npm run dev`, tests); `docker compose down -v` to reset; **Architecture** with a Mermaid flowchart of the four projects and links to ADRs; **Quality gates** (backend build+tests incl. architecture tests, frontend lint/typecheck/tests/build, CodeQL, gitleaks, Dependabot, 'main is protected').
Verified against code: ports, project names, connection-string keys, health URLs. Not verifiable from the repo: branch protection.

## Concepts Used

### Architecture Decision Records and documentation as code

#### What it means

An ADR records a decision, its context, alternatives and consequences, so the *why* survives the people who made it. Docs kept in the repo are versioned with the code.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../PROJECT_OVERVIEW.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

The architecture section.

#### How it works here

README mirrors the diagram that `ProjectReferenceTests` enforce.

#### Why it matters here

Entry point for new contributors.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Documents `POSTGRES_PASSWORD` (.env) and the user-secrets key `ConnectionStrings:Postgres`; ports 5080 (API), 5173 (Vite), 5432 (Postgres).

## Gotchas and Issues

The product description lists features that do not exist yet (students, attendance, payments, WhatsApp). Step 4 says the health URLs return `Healthy`; with a missing connection string the app would not start (see `DatabaseOptions`).

## Related Files

- [`docs/adr/0001-clean-architecture.md`](docs/adr/0001-clean-architecture.md.md)
- [`docs/adr/0002-dotnet-react.md`](docs/adr/0002-dotnet-react.md.md)
- [`frontend/README.md`](frontend/README.md.md)
- [`compose.yaml`](compose.yaml.md)
- [`.env.example`](.env.example.md)
