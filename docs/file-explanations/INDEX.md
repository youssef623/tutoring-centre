# Repository file index

Complete inventory of every tracked file (`git ls-files`, 180 files at commit `371be5c` on top of which this documentation was added). Excluded: `.git/`, `node_modules/` (untracked, installed by `npm ci`), build output (`bin/`, `obj/`, `dist/`). Nothing project-owned is omitted.

Categories: source, test, configuration, documentation, migration, database/schema, script, CI/CD, Docker/infrastructure, lockfile, generated file, binary, image/font/media, vendored dependency, other. No `script`, `database/schema` (apart from the migration), `binary`, `vendored dependency` or `other` files exist in the repository. The DB schema lives only in EF configuration + the migration.

Detail documents: [01-root-and-infrastructure.md](01-root-and-infrastructure.md), [02-domain.md](02-domain.md), [03-application.md](03-application.md), [04-infrastructure.md](04-infrastructure.md), [05-api.md](05-api.md), [06-backend-tests.md](06-backend-tests.md), [07-frontend.md](07-frontend.md)

## Counts by category

| Category | Files |
| --- | --- |
| CI/CD | 4 |
| Docker/infrastructure | 1 |
| configuration | 31 |
| documentation | 13 |
| generated file | 3 |
| image/font/media | 2 |
| lockfile | 1 |
| migration | 1 |
| source | 73 |
| test | 51 |
| **total** | **180** |

## Files

| Path | Category | What it is | Detail |
| --- | --- | --- | --- |
| `.config/dotnet-tools.json` | configuration | Local tool manifest pinning `dotnet-ef` 10.0.12 (`rollForward: false`) so every machine and CI runs the same EF CLI. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.editorconfig` | configuration | Editor/style rules: LF, UTF-8, 4-space C#, 2-space TS/JSON/YAML/csproj; style hints are `suggestion` only; disables CA1707 (underscores in names) for `tests/**`. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.env.example` | configuration | Template for the git-ignored `.env` consumed by `compose.yaml`; `POSTGRES_PASSWORD` intentionally blank. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.gitattributes` | configuration | `* text=auto eol=lf` — normalises line endings to LF. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.github/dependabot.yml` | CI/CD | Weekly Dependabot updates for NuGet (`/`), npm (`/frontend`) and GitHub Actions, grouped into minor+patch PRs. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.github/workflows/ci.yml` | CI/CD | CI: `backend` job (restore, build Release, `dotnet ef migrations has-pending-model-changes`, test) and `frontend` job (`npm ci`, lint, typecheck, test:ci, build). | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.github/workflows/codeql.yml` | CI/CD | CodeQL static analysis for C# (manual build) and JavaScript/TypeScript; PRs, pushes to `main`, and weekly cron. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.github/workflows/secret-scan.yml` | CI/CD | gitleaks secret scan over full git history on PRs and `main`. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.gitignore` | configuration | Ignore rules (the `dotnet new gitignore` template plus `.env`, `node_modules/`, `dist/`, `*.local.json`, `.claude/`). | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `.semantic-manifest.json` | documentation | Bookkeeping for the semantic-git doc generator (which docs cover which paths; last generated commit). | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `ARCHITECTURE.semantic.md` | documentation | Auto-generated architecture note. Layering explanation still valid; its description of Domain/Application as empty and the tolerant missing-connection-string behaviour is stale. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `Directory.Build.props` | configuration | MSBuild file applied to every project: `net10.0`, nullable, implicit usings, `TreatWarningsAsErrors`, `AnalysisLevel=latest-recommended`. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `Directory.Packages.props` | configuration | Central Package Management: the only place NuGet package versions are written. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `OVERVIEW.semantic.md` | documentation | Auto-generated (semantic-git) overview. **Stale**: says no business features and an unscaffolded frontend; code now contains both. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `README.md` | documentation | Human entry point: what the product is meant to be, quick start, architecture diagram, quality gates. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `TutoringCentre.slnx` | configuration | Solution file (new XML `.slnx` format) listing 4 `src` and 5 `tests` projects. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `compose.yaml` | Docker/infrastructure | Docker Compose: one service, `postgres:17`, port 5432, named volume `pgdata`, `pg_isready` healthcheck; password required via `${POSTGRES_PASSWORD:?...}`. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `docs/adr/0001-clean-architecture.md` | documentation | ADR: four-project Clean Architecture, feature folders, architecture tests; lists rejected alternatives. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `docs/adr/0002-dotnet-react.md` | documentation | ADR: .NET + React/Vite SPA, same-origin deployment goal, generated API client plan. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `docs/architecture/api-conventions.md` | documentation | Authoritative description of Problem Details, Result→HTTP mapping, strict JSON, correlation IDs, log redaction, `/api` routing, `GET /api/system/info`. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `docs/architecture/overview.md` | documentation | Failure model (Result vs exceptions), dispatcher pipeline design, and the `CreateCentreCommand` path from CLI to table. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `docs/notes/pipeline-cases.md` | documentation | Case table C1–C9 that specifies `DispatcherTests`. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `frontend/.gitignore` | configuration | Vite template ignore rules (node_modules, dist, logs, editor files). | [07-frontend.md](07-frontend.md) |
| `frontend/.prettierignore` | configuration | Prettier skips `dist`, `coverage`, lockfile, generated route tree. | [07-frontend.md](07-frontend.md) |
| `frontend/.prettierrc` | configuration | Prettier: width 100, double quotes, semicolons, trailing commas. | [07-frontend.md](07-frontend.md) |
| `frontend/README.md` | documentation | Frontend decisions/conventions; contains planned items (i18next, generated client) not yet in the code, plus Vite template boilerplate. | [07-frontend.md](07-frontend.md) |
| `frontend/components.json` | configuration | shadcn/ui CLI config: style `base-nova`, Tailwind CSS file, aliases, lucide icons. | [07-frontend.md](07-frontend.md) |
| `frontend/eslint.config.js` | configuration | ESLint flat config: strict type-checked TS rules, React hooks, React refresh; ignores generated tree. | [07-frontend.md](07-frontend.md) |
| `frontend/index.html` | source | SPA shell: `<div id="root">` and `/src/main.tsx` module script; title is still the template's `frontend`. | [07-frontend.md](07-frontend.md) |
| `frontend/package-lock.json` | lockfile | npm lockfile v3 (exact dependency tree; 639 `node_modules/` entries) used by `npm ci`. | [07-frontend.md](07-frontend.md) |
| `frontend/package.json` | configuration | Dependencies and npm scripts (dev, build, lint, typecheck, test, test:ci). | [07-frontend.md](07-frontend.md) |
| `frontend/public/favicon.svg` | image/font/media | Browser tab icon referenced by `index.html`. | [07-frontend.md](07-frontend.md) |
| `frontend/public/icons.svg` | image/font/media | SVG sprite of social icons from the Vite template; no reference to it found in `src`. | [07-frontend.md](07-frontend.md) |
| `frontend/src/api/errorMessages.test.ts` | test | Tests code/kind/generic fallback of `messageFor`. | [07-frontend.md](07-frontend.md) |
| `frontend/src/api/errorMessages.ts` | source | `messageFor(error, lang)`: code → kind → generic message lookup in en/ar. | [07-frontend.md](07-frontend.md) |
| `frontend/src/api/errors.ts` | source | `ErrorKind` and `ApiError` types mirroring the backend. | [07-frontend.md](07-frontend.md) |
| `frontend/src/api/fixtures/problem-400.json` | test | Sample 400 Problem Details body (`request.malformed`). Not imported by any file I found. | [07-frontend.md](07-frontend.md) |
| `frontend/src/api/fixtures/problem-404.json` | test | Sample 404 Problem Details body (`route.not_found`). Not imported by any file I found. | [07-frontend.md](07-frontend.md) |
| `frontend/src/api/problemDetails.test.ts` | test | Tests `isProblemDetails`. | [07-frontend.md](07-frontend.md) |
| `frontend/src/api/problemDetails.ts` | source | `ProblemDetails` type and `isProblemDetails` type guard. | [07-frontend.md](07-frontend.md) |
| `frontend/src/app/queryClient.ts` | source | Shared `QueryClient` defaults: retry 1, staleTime 30 s. | [07-frontend.md](07-frontend.md) |
| `frontend/src/components/ui/alert.tsx` | source | shadcn Alert components. | [07-frontend.md](07-frontend.md) |
| `frontend/src/components/ui/button-variants.ts` | source | `cva` variant definitions for Button. | [07-frontend.md](07-frontend.md) |
| `frontend/src/components/ui/button.tsx` | source | shadcn Button over Base UI primitive. | [07-frontend.md](07-frontend.md) |
| `frontend/src/components/ui/card.tsx` | source | shadcn Card components. | [07-frontend.md](07-frontend.md) |
| `frontend/src/components/ui/skeleton.tsx` | source | Pulse placeholder. | [07-frontend.md](07-frontend.md) |
| `frontend/src/components/ui/sonner.tsx` | source | Toaster wrapper using `next-themes` theme. | [07-frontend.md](07-frontend.md) |
| `frontend/src/features/status/StatusCard.test.tsx` | test | Component tests with MSW: healthy, 503 + Retry, refetch after Retry. | [07-frontend.md](07-frontend.md) |
| `frontend/src/features/status/StatusCard.tsx` | source | Renders loading / error / unavailable / healthy states. | [07-frontend.md](07-frontend.md) |
| `frontend/src/features/status/api.ts` | source | `fetchReadiness`: the only network call; maps 200/503, throws otherwise. | [07-frontend.md](07-frontend.md) |
| `frontend/src/features/status/useReadiness.ts` | source | TanStack Query hook, key `["health","ready"]`, polls every 15 s. | [07-frontend.md](07-frontend.md) |
| `frontend/src/index.css` | source | Tailwind v4 imports, shadcn theme tokens (oklch light/dark), base layer. | [07-frontend.md](07-frontend.md) |
| `frontend/src/lib/utils.ts` | source | Re-exports `cn` from the `cn` npm package. | [07-frontend.md](07-frontend.md) |
| `frontend/src/main.tsx` | source | Entry: creates router, mounts React with `StrictMode` + `QueryClientProvider` + `RouterProvider`. | [07-frontend.md](07-frontend.md) |
| `frontend/src/routeTree.gen.ts` | generated file | Generated by the TanStack Router Vite plugin from `src/routes`; do not edit. | [07-frontend.md](07-frontend.md) |
| `frontend/src/routes/__root.tsx` | source | Root layout with `<Outlet />` and the Sonner `<Toaster>`. | [07-frontend.md](07-frontend.md) |
| `frontend/src/routes/index.tsx` | source | Route `/`: heading + `StatusCard`. | [07-frontend.md](07-frontend.md) |
| `frontend/src/test/msw/handlers.ts` | test | Default MSW handlers: `/health/ready`, `/api/system/info`. | [07-frontend.md](07-frontend.md) |
| `frontend/src/test/msw/server.ts` | test | MSW node server. | [07-frontend.md](07-frontend.md) |
| `frontend/src/test/render.tsx` | test | `renderWithQueryClient` helper with a fresh QueryClient (retry off). | [07-frontend.md](07-frontend.md) |
| `frontend/src/test/setup.ts` | test | Vitest setup: jest-dom, MSW lifecycle (`onUnhandledRequest: error`), cleanup. | [07-frontend.md](07-frontend.md) |
| `frontend/tsconfig.app.json` | configuration | Strict TS config for `src` (bundler resolution, `noUncheckedIndexedAccess`, `erasableSyntaxOnly`). | [07-frontend.md](07-frontend.md) |
| `frontend/tsconfig.json` | configuration | Solution tsconfig referencing app + node configs; `@/*` path alias. | [07-frontend.md](07-frontend.md) |
| `frontend/tsconfig.node.json` | configuration | TS config for `vite.config.ts`. | [07-frontend.md](07-frontend.md) |
| `frontend/vite.config.ts` | configuration | Vite: TanStack Router, React, Tailwind plugins; `@` alias; dev proxy `/api`,`/health` → `localhost:5080`; Vitest jsdom config. | [07-frontend.md](07-frontend.md) |
| `global.json` | configuration | Pins .NET SDK `10.0.401` with `rollForward: latestFeature`. | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `later.md` | documentation | Parking lot of out-of-scope ideas (pgAdmin in compose; idempotency keys). | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `src/.semantic.md` | documentation | Auto-generated `src/` note. **Stale** (describes Domain/Application as empty). | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `src/TutoringCentre.Api/AssemblyMarker.cs` | source | Assembly handle for tests. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Cli/SeedCommand.cs` | source | `seed` CLI: dispatches `CreateCentreCommand` for two demo centres as `SystemActor`; idempotent. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs` | source | Maps `GET /api/system/info` → dispatcher query → `ToHttpResult`. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs` | source | Accepts/creates `X-Correlation-Id`, stores it, echoes it, pushes it to Serilog `LogContext`. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs` | source | `IExceptionHandler`: 400 `request.malformed` or 500 `server.unexpected`; logs; never leaks exception text. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Http/HttpContextKeys.cs` | source | Constant key for `HttpContext.Items["CorrelationId"]`. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs` | source | `AddApiProblemDetails`: enriches framework problem details and registers the exception handler. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Http/ProblemResult.cs` | source | `IResult` that writes RFC 9457 `application/problem+json` with `traceId`/`correlationId` — the single error writer. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Http/ResultHttpExtensions.cs` | source | `Result`/`Error` → HTTP status/Problem Details mapping. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs` | source | Serilog policy masking Password/Token/Secret/ConnectionString/Phone* properties as `***`. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Program.cs` | source | Entry point and composition root: Serilog, DI, JSON options, `seed` CLI mode, middleware, health checks, `/api` group, fallback. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/Properties/launchSettings.json` | configuration | `http` (5080) and `https` (7197/5245) dev profiles, `ASPNETCORE_ENVIRONMENT=Development`. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/TutoringCentre.Api.csproj` | configuration | Web SDK project; references Application + Infrastructure; `UserSecretsId`; EF Design (private) and Serilog.AspNetCore. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/appsettings.Development.json` | configuration | Development logging levels (same values as the base file). | [05-api.md](05-api.md) |
| `src/TutoringCentre.Api/appsettings.json` | configuration | Logging/Serilog levels, `AllowedHosts: *`, empty `ConnectionStrings:Postgres`. | [05-api.md](05-api.md) |
| `src/TutoringCentre.Application/AssemblyMarker.cs` | source | Assembly handle used by handler scanning and tests. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs` | source | Command record (Name, Slug, TimeZoneId, DefaultLocale) → `CreateCentreResult`. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs` | source | Use case: authorize (SystemActor) → slug uniqueness → `Centre.Create` → `Add`. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreResult.cs` | source | Response record (CentreId, Slug). | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs` | source | FluentValidation shape checks (not-empty, max lengths, enum defined). | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Centres/ICentreRepository.cs` | source | Write-side port: `ExistsBySlugAsync`, `Add`. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs` | source | Single entry point: `SendAsync` (command pipeline) and `QueryAsync` (read-only pipeline); validation, transaction control, outcome logging. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs` | source | Reflection scan registering `ICommandHandler<,>`/`IQueryHandler<,>` implementations as scoped, plus validators. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Cqrs/ICommand.cs` | source | Marker interface `ICommand<TResponse>`. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Cqrs/ICommandHandler.cs` | source | Handler contract (contravariant in the command). | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Cqrs/IQuery.cs` | source | Marker interface `IQuery<TResponse>`. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs` | source | Query handler contract. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Cqrs/Unit.cs` | source | `Unit` — the "no data" response type. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Ports/IClock.cs` | source | Port for "now" and IANA-time-zone conversion. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs` | source | Port: Begin(readOnly)/SaveChanges/Commit/Rollback; only the dispatcher calls it. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Security/Actor.cs` | source | `Actor` base record, `SystemActor(CentreId?)`, `AnonymousActor`. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs` | source | Per-scope holder; starts anonymous; `Set` allowed exactly once. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Common/Security/ICurrentActor.cs` | source | Read-only view of the current actor for handlers. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/DependencyInjection.cs` | source | `AddApplication`: registers `CurrentActorContext`, `ICurrentActor`, all handlers/validators (scan), and `Dispatcher` — all scoped. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs` | source | Read-side port + `SchemaStatus` record. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs` | source | Builds `SystemInfoDto` from the read service and assembly informational version (build metadata after `+` stripped). | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs` | source | Parameterless query record. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/Platform/SystemInfoDto.cs` | source | Public DTO: ApplicationVersion, LatestMigration, DatabaseUpToDate. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Application/TutoringCentre.Application.csproj` | configuration | References Domain; FluentValidation(+DI extensions), DI and Logging abstractions; `InternalsVisibleTo` for Architecture and Application tests. | [03-application.md](03-application.md) |
| `src/TutoringCentre.Domain/AssemblyMarker.cs` | source | Empty internal class so tests can load the assembly via `typeof(AssemblyMarker).Assembly`. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Domain/Centres/Centre.cs` | source | The `Centre` (tenant) entity; private constructors; `Create` factory enforcing name/slug/time-zone rules and returning `Result<Centre>`. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Domain/Centres/SupportedLocale.cs` | source | Enum `Ar`, `En`. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Domain/Common/Entity.cs` | source | Base entity: `Id` is a UUIDv7 created in the constructor. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Domain/Common/Error.cs` | source | Immutable record `Error(Code, Message, Kind, Fields)` with factory methods per kind. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Domain/Common/ErrorKind.cs` | source | Closed enum: Validation, NotFound, Conflict, Rule, Forbidden. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Domain/Common/ITenantOwned.cs` | source | Marker interface (`Guid CentreId`) for future tenant-scoped entities; **no implementer yet**. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Domain/Common/Result.cs` | source | `Result` / `Result<T>`: success-or-error value type used instead of exceptions for expected failures. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Domain/TutoringCentre.Domain.csproj` | configuration | Domain project: no package or project references; grants `InternalsVisibleTo` to the architecture tests. | [02-domain.md](02-domain.md) |
| `src/TutoringCentre.Infrastructure/AssemblyMarker.cs` | source | Assembly handle for tests. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/DependencyInjection.cs` | source | `AddInfrastructure`: clock, health check, validated `DatabaseOptions`, `AppDbContext`, interceptor, unit of work, repository, read service. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs` | source | The single EF Core context; no `DbSet`s; applies all `IEntityTypeConfiguration` from the assembly. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs` | source | Maps `Centre` → `platform.centres`: keys, lengths, unique slug index, locale converter + CHECK, shadow timestamp columns. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs` | source | Options class with `[Required]` connection string. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs` | source | `SaveChangesInterceptor` stamping `CreatedAt`/`UpdatedAt` shadow properties. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs` | source | `ApplyMigrationsAsync(IServiceProvider)` extension. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.Designer.cs` | generated file | EF-generated target-model snapshot for the migration (`[Migration]` id attribute). | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.cs` | migration | The only migration: creates schema `platform`, table `centres`, PK, CHECK, unique index. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs` | generated file | EF-generated current-model snapshot used to diff the next migration; CI checks it is up to date. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/Schemas.cs` | source | Constants for PostgreSQL schema names (`platform`, `identity`). | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs` | source | `IUnitOfWork` over `AppDbContext`: explicit transaction, `SET TRANSACTION READ ONLY` for queries. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs` | source | Reads applied/pending EF migrations → `SchemaStatus`. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs` | source | `ICentreRepository` over `db.Set<Centre>()`. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/Time/SystemClock.cs` | source | `IClock` over `TimeProvider` and `TimeZoneInfo`. | [04-infrastructure.md](04-infrastructure.md) |
| `src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj` | configuration | References Application + Domain; EF Core, Npgsql provider, snake_case naming, Npgsql health check, Options.DataAnnotations. | [04-infrastructure.md](04-infrastructure.md) |
| `tests/.semantic.md` | documentation | Auto-generated `tests/` note. **Stale** (says 9 tests; empty test projects). | [01-root-and-infrastructure.md](01-root-and-infrastructure.md) |
| `tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs` | test | Running seed twice yields exactly two centres. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs` | test | Collection for DB-backed API tests. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs` | test | `WebApplicationFactory<Program>` + Postgres container, env `Testing`, strict DI validation. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs` | test | Test-only `/api/test/*` endpoints. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs` | test | Adds the test endpoints after the real pipeline. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs` | test | Test command/handler/validator producing every error kind. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs` | test | Collection for convention tests. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs` | test | `ApiFactory` plus test handlers and an in-memory log sink. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs` | test | Serilog sink capturing events for assertions. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs` | test | Liveness 200 and readiness 503 with an unreachable DB. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs` | test | Readiness 200 with a real DB. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs` | test | Header generation/validation/echo; ID in body and logs. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs` | test | Info line for success; Error line for exception. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs` | test | Status/code/shape for every error kind, strict JSON, fallback 404. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs` | test | Unit tests of the mapping without a host. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs` | test | Masking behaviour. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Api.Tests/TutoringCentre.Api.Tests.csproj` | configuration | Web-SDK xUnit project hosting the Api in-process. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs` | test | Handler branches with fakes: forbidden, conflict, domain error, success. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs` | test | Validator shape rules. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs` | test | Pipeline call-order tests (cases C1–C9) using `FakeUnitOfWork`. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs` | test | Test-only commands, queries, handlers, validators, `HandlerBehaviour`. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs` | test | In-memory repository. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs` | test | Records Begin/Save/Commit/Rollback calls. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs` | test | DTO mapping from `SchemaStatus`; version has no `+`. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs` | test | Default anonymous; set once. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Application.Tests/TutoringCentre.Application.Tests.csproj` | configuration | xUnit project referencing Application + DI package. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs` | test | IL-level forbidden-dependency checks per layer. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs` | test | Helper: reads `ProjectReference`s from `.csproj` XML. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs` | test | Exact allowed `ProjectReference` sets per layer. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Architecture.Tests/TutoringCentre.Architecture.Tests.csproj` | configuration | xUnit project referencing all four `src` projects + NetArchTest. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs` | test | `Centre.Create` rules: trimming, name/slug/time-zone failures, boundaries. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs` | test | Unique ids; id is UUID version 7. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs` | test | `Error.Validation` with/without fields. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs` | test | Result success/failure semantics and guards. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Domain.Tests/TutoringCentre.Domain.Tests.csproj` | configuration | xUnit project referencing Domain only. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs` | test | Two parallel creates with one slug → exactly one row (unique index). | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs` | test | Real-DB command tests: persist, conflict, forbidden, bad time zone. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs` | test | Helpers dispatching as an actor in a fresh scope. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs` | test | xUnit collection sharing one DB fixture. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs` | test | Starts Postgres 17 container, migrates, Respawn reset, SQL helpers, real DI container. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs` | test | Base class resetting the DB before each test. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs` | test | Table, unique index, CHECK, history table exist after migrating. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs` | test | A write inside a query is rejected by Postgres (SQLSTATE 25006). | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs` | test | Rollback on throw and on failure result; commit on success. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs` | test | `GetSystemInfoQuery` against a migrated DB. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs` | test | UtcNow and Cairo winter/summer conversions. | [06-backend-tests.md](06-backend-tests.md) |
| `tests/TutoringCentre.Infrastructure.Tests/TutoringCentre.Infrastructure.Tests.csproj` | configuration | xUnit project with Testcontainers, Respawn, FakeTimeProvider. | [06-backend-tests.md](06-backend-tests.md) |
