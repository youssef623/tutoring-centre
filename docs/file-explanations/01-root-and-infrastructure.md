# Root, configuration, Docker, CI and documentation files

Part of the [file index](INDEX.md). Concepts referenced here are taught in [`../PROJECT_OVERVIEW.md`](../PROJECT_OVERVIEW.md) §6 and §11.

## Build and tooling configuration

### `global.json`
```json
{ "sdk": { "version": "10.0.401", "rollForward": "latestFeature" } }
```
Pins the .NET SDK. `rollForward: latestFeature` lets a newer feature band of the same major.minor (10.0.4xx+) be used if 10.0.401 is not installed, but not a different major/minor. CI's `actions/setup-dotnet` reads this file (`global-json-file: global.json`) so CI and laptops use the same SDK line. **Before changing:** bump together with `Directory.Packages.props` EF/ASP.NET versions and `.config/dotnet-tools.json`.

### `Directory.Build.props`
MSBuild automatically imports this file into every project under the directory. It sets `TargetFramework=net10.0`, `Nullable=enable`, `ImplicitUsings=enable`, `TreatWarningsAsErrors=true`, `AnalysisLevel=latest-recommended`. Consequence: individual `.csproj` files do not declare a target framework (`src/.semantic.md` notes this as intentional), and **any analyzer warning breaks the build** — hence the justified `[SuppressMessage]` attributes in source (e.g., CA1812 for classes only instantiated by DI).

### `Directory.Packages.props` (Central Package Management)
`ManagePackageVersionsCentrally=true`: `.csproj` files write `<PackageReference Include="X" />` with **no version**; the version is a `<PackageVersion>` here. There is an empty `ItemGroup` with the comment "PackageVersion entries go here as projects add dependencies." (leftover scaffolding) followed by the real list (24 packages — see overview §2). Adding a package: add `PackageVersion` here, then a versionless `PackageReference` in the project. A `Version=` on a `PackageReference` is an error under CPM.

### `TutoringCentre.slnx`
The new XML solution format. Folder `/src/` holds Api, Application, Domain, Infrastructure; folder `/tests/` holds Api.Tests, Application.Tests, Architecture.Tests, Domain.Tests, Infrastructure.Tests. `ProjectFiles.FindRepositoryRoot` (Architecture tests) locates the repo by finding this file name.

### `.config/dotnet-tools.json`
Local tool manifest: `dotnet-ef` `10.0.12`, `rollForward: false`. `dotnet tool restore` (CI step) installs exactly this. Used by `dotnet ef migrations add …` and CI's `has-pending-model-changes`.

### `.editorconfig`
`root = true`; UTF-8, LF, final newline, trim whitespace; 4-space indent default, 2-space for TS/JS/JSON/YAML/CSS/HTML and for csproj/props/slnx/xml; Markdown keeps trailing whitespace (hard breaks). C# style preferences (`file_scoped` namespaces, `var` when apparent, usings outside namespace) are `:suggestion` — IDE hints, never build errors. `[tests/**/*.cs] dotnet_diagnostic.CA1707.severity = none` allows `Method_Condition_Result` underscores in test names while production code keeps the rule.

### `.gitattributes`
`* text=auto eol=lf` — Git normalises line endings to LF in the working tree on every OS (matches `.editorconfig`; avoids CRLF noise).

### `.gitignore`
Mostly the standard `dotnet new gitignore` template. Project-specific tail: `node_modules/`, `dist/`, `.env`, `*.local.json`, `.claude/`. `.env` is ignored so the Postgres password never reaches git; `.claude/` (local Claude Code config) is ignored.

### `.env.example`
```
POSTGRES_DB=tutoring
POSTGRES_USER=tutoring_dev
POSTGRES_PASSWORD=
```
Template copied to `.env`. Comment says keep DB and USER unchanged because the README connection string and Compose healthcheck use them. Password blank on purpose.

## Docker

### `compose.yaml`
See overview §11.1. Line-by-line: `image: postgres:17` (major version pinned, minor floats); `environment` substitutes from `.env` (Compose reads `.env` next to the file automatically); `${POSTGRES_PASSWORD:?…}` fails fast with a message when unset; `ports: "5432:5432"` (host:container) means a local PostgreSQL already on 5432 will conflict; `volumes: pgdata:/var/lib/postgresql/data` named volume survives `docker compose down` but is deleted by `down -v`; `healthcheck` `pg_isready -U … -d …` every 5 s, timeout 5 s, 5 retries. The application is **not** defined here (no Dockerfile in the repo).

## CI/CD (`.github/`)

### `workflows/ci.yml`
Two parallel jobs, `backend` and `frontend` (see overview §11.2 for the step list). Design points visible in the file: `concurrency: ci-${{ github.ref }}` with `cancel-in-progress` so only the newest push per branch/PR runs; least-privilege `permissions: contents: read`; backend builds once (`restore` → `build --no-restore`) and tests with `--no-build`; the `has-pending-model-changes` step provides a dummy `ConnectionStrings__Postgres` (double underscore = `:` in environment-variable configuration keys) because the design-time model comparison needs a syntactically valid string but "never connects"; frontend uses `npm ci` (installs exactly the lockfile) and caches npm by lockfile hash.

### `workflows/codeql.yml`
Matrix over `csharp` (`build-mode: manual` → `dotnet build TutoringCentre.slnx -c Release`) and `javascript-typescript` (`build-mode: none`). Runs on PRs, pushes to `main`, and weekly (`27 3 * * 1`). `security-events: write` uploads results to the Security tab.

### `workflows/secret-scan.yml`
`gitleaks/gitleaks-action@v3`, full-history checkout (`fetch-depth: 0`), `GITHUB_TOKEN` provided; `pull-requests: write` so it can comment on a PR when it finds a leak. No `.gitleaks.toml` exists, so defaults apply.

### `dependabot.yml`
Weekly updates for `nuget` (`/`), `npm` (`/frontend`), `github-actions` (`/`); each ecosystem groups all minor+patch updates into one PR (major updates arrive individually).

## Documentation files

| File | Notes |
| --- | --- |
| `README.md` | Quick start (PowerShell and bash variants), architecture Mermaid diagram, quality-gate claims. Its "Run locally" steps match `launchSettings.json` (port 5080) and `vite.config.ts` (proxy to 5080). |
| `docs/adr/0001-clean-architecture.md` | Decision + alternatives (layered monolith, one project per module, microservices) + consequences. Mentions "Day 2/Day 4" — references to a plan not in the repo. |
| `docs/adr/0002-dotnet-react.md` | Dated 2026-10-02. States the intent to generate an API client from OpenAPI "starting Day 12" — not present in code yet. |
| `docs/architecture/api-conventions.md` | The most reliable description of the HTTP layer; I verified each rule against `Api/Http/*`, `Program.cs` and tests. |
| `docs/architecture/overview.md` | Failure model, dispatcher design diagram, CLI→table request path table. The table's file references were all verified to exist. |
| `docs/notes/pipeline-cases.md` | Cases C1–C9; each corresponds to a `DispatcherTests` method (C1 `SendAsync_ValidCommandHandlerSucceeds…`, C2 `…InvalidCommand…`, C3 `…HandlerReturnsFailure…`, C4 `…HandlerThrows…`, C5 `…SaveChangesThrows…`, C6–C8 query equivalents, C9 `…SeveralFieldFailures…`). |
| `later.md` | Two parked ideas with dates (2026-10-02). |
| `OVERVIEW.semantic.md`, `ARCHITECTURE.semantic.md`, `src/.semantic.md`, `tests/.semantic.md`, `.semantic-manifest.json` | Generated by a tool called semantic-git ("Do not edit manually — regenerate with /semantic-git"). The manifest records generation at commit `42bdfc17…`. They predate the Domain/Application/Infrastructure implementation: they say those layers are empty, count "9 tests", and say the frontend is "not scaffolded yet". **Treat as historical.** Still useful for the explanation of the two architecture-test techniques and the three experiments proving the tests catch what they claim. |
