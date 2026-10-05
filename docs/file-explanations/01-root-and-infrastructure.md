# Root, configuration, Docker, CI, documentation and docs folders

Folder map generated from the per-file explanations. Part of the [file index](INDEX.md); the teaching overview is [`../PROJECT_OVERVIEW2.md`](../PROJECT_OVERVIEW2.md). Each row links to the full explanation of that file (purpose, where it fits, walkthrough, concepts, flow, configuration, gotchas, related files). **31 files.**

## `(repository root)`

| File | Purpose | Explanation |
| --- | --- | --- |
| `.editorconfig` | Declares formatting and a few diagnostic rules that editors and the compiler apply consistently across the repository. | [Explanation](./.editorconfig.md) |
| `.env.example` | Template for the git-ignored `.env` file that Docker Compose reads to configure the local PostgreSQL container. | [Explanation](./.env.example.md) |
| `.gitattributes` | Forces Git to normalise text files to LF so Windows and Linux contributors do not create line-ending diffs, and pins the generated API artefacts to LF. | [Explanation](./.gitattributes.md) |
| `.gitignore` | Lists files Git must not track: build output, IDE files, secrets (`.env`), dependency folders. | [Explanation](./.gitignore.md) |
| `.semantic-manifest.json` | Bookkeeping file for a documentation generator called semantic-git: records which generated docs cover which folders and at which commit they were last refreshed. | [Explanation](./.semantic-manifest.json.md) |
| `ARCHITECTURE.semantic.md` | Generated architecture description focused on the dependency rule, architecture tests and health checks. | [Explanation](./ARCHITECTURE.semantic.md.md) |
| `Directory.Build.props` | Applies the same compiler settings to every .NET project in the repository without repeating them in each `.csproj`. | [Explanation](./Directory.Build.props.md) |
| `Directory.Packages.props` | Single source of truth for every NuGet package version (Central Package Management). | [Explanation](./Directory.Packages.props.md) |
| `OVERVIEW.semantic.md` | Generated overview written when the repo had only the four-project skeleton and health checks. | [Explanation](./OVERVIEW.semantic.md.md) |
| `README.md` | Human entry point: what the product is meant to be, how to run it locally (database, HTTPS API, seeding, frontend, tests, E2E), the architecture diagram, decisions and the quality gates. | [Explanation](./README.md.md) |
| `TutoringCentre.slnx` | Solution file listing the nine .NET projects so `dotnet build/test TutoringCentre.slnx` operates on the whole repo. | [Explanation](./TutoringCentre.slnx.md) |
| `api.http` | A manual walkthrough of the session API for VS Code's REST Client extension: CSRF token, login as two users, centre selection, logout with and without a token. | [Explanation](./api.http.md) |
| `compose.yaml` | Defines the only container this project uses: a local PostgreSQL 17 database for development. | [Explanation](./compose.yaml.md) |
| `global.json` | Pins which .NET SDK builds the solution so every laptop and CI runner uses the same compiler and tooling line. | [Explanation](./global.json.md) |
| `later.md` | Parking lot for ideas deliberately postponed, with the reason and the earliest sensible time. | [Explanation](./later.md.md) |

## `.config`

| File | Purpose | Explanation |
| --- | --- | --- |
| `dotnet-tools.json` | Local .NET tool manifest that pins the `dotnet-ef` command-line tool to the same version as the EF Core packages. | [Explanation](./.config/dotnet-tools.json.md) |

## `.github`

| File | Purpose | Explanation |
| --- | --- | --- |
| `dependabot.yml` | Tells GitHub Dependabot to open weekly pull requests updating NuGet, npm and GitHub Actions dependencies. | [Explanation](./.github/dependabot.yml.md) |

## `.github/workflows`

| File | Purpose | Explanation |
| --- | --- | --- |
| `ci.yml` | Main continuous-integration workflow: builds and tests the backend and the frontend, then runs the Playwright end-to-end journeys, on every pull request and on pushes to `main`. | [Explanation](./.github/workflows/ci.yml.md) |
| `codeql.yml` | Runs GitHub's CodeQL static security analysis on the C# and TypeScript code. | [Explanation](./.github/workflows/codeql.yml.md) |
| `secret-scan.yml` | Scans the full git history for committed secrets with gitleaks. | [Explanation](./.github/workflows/secret-scan.yml.md) |

## `docs`

| File | Purpose | Explanation |
| --- | --- | --- |
| `PROJECT_OVERVIEW.md` | The original teaching overview of the repository (15 sections plus two appendices), written against the commit before the authentication work was merged and kept unchanged as that earlier snapshot. | [Explanation](./docs/PROJECT_OVERVIEW.md.md) |
| `PROJECT_OVERVIEW2.md` | The current teaching overview of the repository: the original 15 sections and appendices, corrected in place and extended for the merged authentication, session, CSRF, rate-limiting, membership, OpenAPI-client, internationalisation and end-to-end-testing work. | [Explanation](./docs/PROJECT_OVERVIEW2.md.md) |

## `docs/adr`

| File | Purpose | Explanation |
| --- | --- | --- |
| `0001-clean-architecture.md` | Architecture Decision Record explaining why the backend is four projects with inward-only dependencies, plus the alternatives that were rejected. | [Explanation](./docs/adr/0001-clean-architecture.md.md) |
| `0002-dotnet-react.md` | ADR recording the choice of ASP.NET Core + EF Core + PostgreSQL for the backend and a React + TypeScript Vite SPA for the frontend. | [Explanation](./docs/adr/0002-dotnet-react.md.md) |
| `0003-custom-cqrs-dispatcher.md` | Architecture Decision Record: why the CQRS pipeline is a small hand-written `Dispatcher` instead of MediatR or decorators. | [Explanation](./docs/adr/0003-custom-cqrs-dispatcher.md.md) |
| `0004-unit-of-work-port.md` | Architecture Decision Record: why Application defines a thin `IUnitOfWork` port that only the dispatcher calls. | [Explanation](./docs/adr/0004-unit-of-work-port.md.md) |
| `0005-cookie-authentication.md` | Architecture Decision Record: why sessions are an encrypted `HttpOnly` `__Host-` cookie with antiforgery tokens and per-request revalidation, rather than JWT. | [Explanation](./docs/adr/0005-cookie-authentication.md.md) |

## `docs/architecture`

| File | Purpose | Explanation |
| --- | --- | --- |
| `api-conventions.md` | Authoritative description of the HTTP conventions: error shape, Result-to-status mapping, exception handling, strict JSON, correlation IDs, log redaction, routing and the first endpoint. | [Explanation](./docs/architecture/api-conventions.md.md) |
| `authentication.md` | Authoritative description of the authentication flow: login, centre selection, per-request pipeline, anonymous routes, logout, cookie and claims, CSRF, revalidation, lockout and rate limiting, error codes, and what is deferred. | [Explanation](./docs/architecture/authentication.md.md) |
| `overview.md` | Explains the failure model (Result vs exceptions), the full request pipeline (middleware, authentication, actor, authorization, rate limiting, CSRF filter, dispatcher), the full path of `CreateCentreCommand` from CLI to table, the path of `GET /api/system/info` from HTTP to table, and links the ADRs. | [Explanation](./docs/architecture/overview.md.md) |

## `docs/notes`

| File | Purpose | Explanation |
| --- | --- | --- |
| `pipeline-cases.md` | Specification table (cases C1-C9) that `DispatcherTests` implements. | [Explanation](./docs/notes/pipeline-cases.md.md) |
