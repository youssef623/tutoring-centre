# README.md

## Purpose
The repository's front page. It says what the product is (a multi-tenant platform for Egyptian tutoring centres, line 5), gives a step-by-step local quick start (lines 7–72), shows the four-project dependency diagram (lines 74–94), and lists the quality gates every PR must pass (lines 96–98).

## Where it fits
Top-level documentation. It links to `docs/adr/0001-clean-architecture.md`, `docs/adr/0002-dotnet-react.md` and `frontend/README.md`. Its commands depend on `.env.example`, `compose.yaml`, `src/TutoringCentre.Api` (user-secrets, launch profile `http`) and `frontend/package.json` scripts.

## Walkthrough
- **Line 3:** CI badge for `.github/workflows/ci.yml`.
- **Line 5:** product scope: students and parents, groups and schedules, manual and QR attendance, fees and payments, and a WhatsApp assistant in Egyptian Arabic and English. Stack: ASP.NET Core (.NET 10), Clean Architecture, a hand-written CQRS pipeline, PostgreSQL, React + TypeScript. Most of this scope is **not built yet** (see `docs/PROJECT_OVERVIEW.md` §1).
- **Lines 9–14, prerequisites:** .NET 10 SDK (pinned in `global.json`), Node LTS ≥ 22.12, Docker Desktop, Git.
- **Lines 16–72, run locally:**
  1. Clone.
  2. Copy `.env.example` to `.env` and set `POSTGRES_PASSWORD` ("no `;` or spaces", because the same password goes into a semicolon-delimited connection string). Then `docker compose up -d` and wait for `(healthy)`.
  3. `dotnet user-secrets set "ConnectionStrings:Postgres" ...`, keeping the secret outside the repo.
  4. `dotnet run --launch-profile http` serves the API on port 5080. Check `/health` and `/health/ready`.
  5. `npm ci && npm run dev` in `frontend/` serves port 5173.
  6. `dotnet test` and `npm run test:ci`.
  - Reset the DB with `docker compose down -v`.
- **Lines 74–94:** Mermaid diagram of the project references. The Api → Infrastructure edge is dashed with the label "composition root only".
- **Lines 96–98, quality gates:** backend build and tests, frontend lint, typecheck, tests and build, CodeQL, gitleaks, weekly Dependabot. It says `main` is protected.

## Concepts used
- **User-secrets:** a per-machine secret store outside the repo, keyed by the `UserSecretsId` in `TutoringCentre.Api.csproj`.
- **Liveness vs readiness:** `/health` vs `/health/ready`.

## Data and control flow
Not applicable (documentation).

## Configuration and environment
Mentions `POSTGRES_PASSWORD`, `ConnectionStrings:Postgres`, ports 5080 and 5173.

## Gotchas and issues
- "`main` is protected" is a GitHub repository setting that cannot be verified from the files.
- The scope in line 5 describes the end goal, not the current state.

## Related files
- [compose.yaml](compose.yaml.md)
- [.env.example](.env.example.md)
- [Program.cs](src/TutoringCentre.Api/Program.cs.md)
- [ADR 0001](docs/adr/0001-clean-architecture.md.md)
- [frontend README](frontend/README.md.md)
