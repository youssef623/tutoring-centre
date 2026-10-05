# README.md

## Purpose

Human entry point: what the product is meant to be, how to run it locally (database, HTTPS API, seeding, frontend, tests, E2E), the architecture diagram, decisions and the quality gates.

## Where It Fits

Root documentation. Links to `docs/adr/0001`, `0002`, `0005`, `docs/architecture/authentication.md` and `frontend/README.md`. Describes the commands that match `launchSettings.json` (the `https` profile, `https://localhost:7197`), `vite.config.ts` (proxy to that URL), `SeedCommand` and `playwright.config.ts`.

## Walkthrough

Sections: title + CI badge; one-paragraph product description (multi-tenant platform for Egyptian tutoring centres: students/parents, groups/schedules, attendance, fees, WhatsApp assistant) - the **intended** scope, not what exists; **Quick start** with seven numbered steps: (1) clone, (2) `.env` and `docker compose up -d`, (3) user-secrets connection string, (4) **run the API on https://localhost:7197** - `dotnet dev-certs https --trust` once, then `--launch-profile https`, with the reason (the session and antiforgery cookies are `Secure` and `__Host-`, which a browser only stores from an HTTPS origin) and health URLs on https; (5) **seed the development database** - set `Seed:Password` with user-secrets (never written in the repo), run `-- seed`; a table of the five seeded accounts (`owner@nile.test` Owner of Nile; `owner@maadi.test` Owner of Maadi; `teacher@both.test` Teacher in both; `secretary@nile.test` Secretary; `inactive@nile.test` Secretary with an inactive membership); (6) run the frontend (`npm ci`, `npm run dev`, open http://localhost:5173; signed out it redirects to `/login`); (7) run the tests, then **end-to-end journeys** with `SEED_PASSWORD` set (PowerShell and bash variants) - Playwright reuses already-running servers locally. `docker compose down -v` resets the database. **Architecture** (Mermaid flowchart of the four projects) and a decisions list (ADR 0001, 0002, 0005, the authentication document, the frontend README). **Quality gates**: backend build and all tests, frontend lint, type-check, tests, i18n key-parity check and production build, three Playwright end-to-end journeys, CodeQL, gitleaks, Dependabot, protected `main`.
Verified against code: ports, project names, seeded accounts, command names, health URLs. Not verifiable from the repo: branch protection.

## Concepts Used

### Architecture Decision Records and documentation as code

#### What it means

An ADR records a decision, its context, alternatives and consequences, so the *why* survives the people who made it. Docs kept in the repo are versioned with the code.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

README as the entry point.

#### How it works here

All sections.

#### Why it matters here

The first file a new developer reads; it now covers HTTPS, seeding and E2E because sign-in cannot work without them.

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#11-configuration--environment--deployment](../PROJECT_OVERVIEW2.md#11-configuration--environment--deployment).)

#### Where it appears in this file

Passwords supplied by the developer.

#### How it works here

Steps 3 and 5 and the E2E commands.

#### Why it matters here

Nothing secret is committed; the seed password goes through user-secrets and an environment variable.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Documents `POSTGRES_PASSWORD` (.env), the user-secrets keys `ConnectionStrings:Postgres` and `Seed:Password`, the environment variable `SEED_PASSWORD` for E2E, and the ports 7197 (API https), 5173 (Vite), 5432 (Postgres).

## Gotchas and Issues

The product description lists features that do not exist yet (students, attendance, payments, WhatsApp). The README says the E2E run starts its own copies of the API and frontend; Playwright reuses running servers locally only because `reuseExistingServer` is `!process.env.CI`.

## Related Files

- [`docs/adr/0001-clean-architecture.md`](docs/adr/0001-clean-architecture.md.md)
- [`docs/adr/0002-dotnet-react.md`](docs/adr/0002-dotnet-react.md.md)
- [`docs/adr/0005-cookie-authentication.md`](docs/adr/0005-cookie-authentication.md.md)
- [`docs/architecture/authentication.md`](docs/architecture/authentication.md.md)
- [`frontend/README.md`](frontend/README.md.md)
- [`compose.yaml`](compose.yaml.md)
- [`.env.example`](.env.example.md)
- [`frontend/playwright.config.ts`](frontend/playwright.config.ts.md)
