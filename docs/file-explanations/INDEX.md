# Repository file index

Every tracked file in the repository (`git ls-files`), grouped by directory. **180 files**: **174** have an explanation file under this folder (mirroring the repository path plus `.md`) and **6** are intentionally skipped (generated/lockfile/media) with the reason given.

Excluded from the inventory: `.git/`, `node_modules/` (untracked; created by `npm ci`), and build output (`bin/`, `obj/`, `dist/`) - none is tracked. Categories used: source, test, configuration, documentation, migration, database/schema, script, CI/CD, Docker/infrastructure, lockfile, generated file, binary, image/font/media, vendored dependency, other. This repository has no `script`, `binary`, `vendored dependency` or `other` files, and no standalone `database/schema` file (the schema is defined by EF configuration and the migration).

Companion documents: [`../PROJECT_OVERVIEW.md`](../PROJECT_OVERVIEW.md) (teaching overview) and the earlier area guides [01 root/infra](01-root-and-infrastructure.md), [02 domain](02-domain.md), [03 application](03-application.md), [04 infrastructure](04-infrastructure.md), [05 api](05-api.md), [06 backend tests](06-backend-tests.md), [07 frontend](07-frontend.md) which summarise whole folders.

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

## `(repository root)`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `.editorconfig` | configuration | Declares formatting and a few diagnostic rules that editors and the compiler apply consistently across the repository. | [Explanation](./.editorconfig.md) |
| `.env.example` | configuration | Template for the git-ignored `.env` file that Docker Compose reads to configure the local PostgreSQL container. | [Explanation](./.env.example.md) |
| `.gitattributes` | configuration | Forces Git to normalise text files to LF so Windows and Linux contributors do not create line-ending diffs. | [Explanation](./.gitattributes.md) |
| `.gitignore` | configuration | Lists files Git must not track: build output, IDE files, secrets (`.env`), dependency folders. | [Explanation](./.gitignore.md) |
| `.semantic-manifest.json` | documentation | Bookkeeping file for a documentation generator called semantic-git: records which generated docs cover which folders and at which commit they were last refreshed. | [Explanation](./.semantic-manifest.json.md) |
| `ARCHITECTURE.semantic.md` | documentation | Generated architecture description focused on the dependency rule, architecture tests and health checks. | [Explanation](./ARCHITECTURE.semantic.md.md) |
| `Directory.Build.props` | configuration | Applies the same compiler settings to every .NET project in the repository without repeating them in each `.csproj`. | [Explanation](./Directory.Build.props.md) |
| `Directory.Packages.props` | configuration | Single source of truth for every NuGet package version (Central Package Management). | [Explanation](./Directory.Packages.props.md) |
| `OVERVIEW.semantic.md` | documentation | Generated overview written when the repo had only the four-project skeleton and health checks. | [Explanation](./OVERVIEW.semantic.md.md) |
| `README.md` | documentation | Human entry point: what the product is meant to be, how to run it locally, the architecture diagram and the quality gates. | [Explanation](./README.md.md) |
| `TutoringCentre.slnx` | configuration | Solution file listing the nine .NET projects so `dotnet build/test TutoringCentre.slnx` operates on the whole repo. | [Explanation](./TutoringCentre.slnx.md) |
| `compose.yaml` | Docker/infrastructure | Defines the only container this project uses: a local PostgreSQL 17 database for development. | [Explanation](./compose.yaml.md) |
| `global.json` | configuration | Pins which .NET SDK builds the solution so every laptop and CI runner uses the same compiler and tooling line. | [Explanation](./global.json.md) |
| `later.md` | documentation | Parking lot for ideas deliberately postponed, with the reason and the earliest sensible time. | [Explanation](./later.md.md) |

## `.config`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `dotnet-tools.json` | configuration | Local .NET tool manifest that pins the `dotnet-ef` command-line tool to the same version as the EF Core packages. | [Explanation](./.config/dotnet-tools.json.md) |

## `.github`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `dependabot.yml` | CI/CD | Tells GitHub Dependabot to open weekly pull requests updating NuGet, npm and GitHub Actions dependencies. | [Explanation](./.github/dependabot.yml.md) |

## `.github/workflows`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `ci.yml` | CI/CD | Main continuous-integration workflow: builds and tests the backend and the frontend on every pull request and on pushes to `main`. | [Explanation](./.github/workflows/ci.yml.md) |
| `codeql.yml` | CI/CD | Runs GitHub's CodeQL static security analysis on the C# and TypeScript code. | [Explanation](./.github/workflows/codeql.yml.md) |
| `secret-scan.yml` | CI/CD | Scans the full git history for committed secrets with gitleaks. | [Explanation](./.github/workflows/secret-scan.yml.md) |

## `docs/adr`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `0001-clean-architecture.md` | documentation | Architecture Decision Record explaining why the backend is four projects with inward-only dependencies, plus the alternatives that were rejected. | [Explanation](./docs/adr/0001-clean-architecture.md.md) |
| `0002-dotnet-react.md` | documentation | ADR recording the choice of ASP.NET Core + EF Core + PostgreSQL for the backend and a React + TypeScript Vite SPA for the frontend. | [Explanation](./docs/adr/0002-dotnet-react.md.md) |

## `docs/architecture`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `api-conventions.md` | documentation | Authoritative description of the HTTP conventions: error shape, Result-to-status mapping, exception handling, strict JSON, correlation IDs, log redaction, routing and the first endpoint. | [Explanation](./docs/architecture/api-conventions.md.md) |
| `overview.md` | documentation | Explains the failure model (Result vs exceptions), the dispatcher pipeline design and the full path of `CreateCentreCommand` from CLI to table. | [Explanation](./docs/architecture/overview.md.md) |

## `docs/notes`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `pipeline-cases.md` | documentation | Specification table (cases C1-C9) that `DispatcherTests` implements. | [Explanation](./docs/notes/pipeline-cases.md.md) |

## `frontend`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `.gitignore` | configuration | Ignore rules for the frontend folder (Vite template). | [Explanation](./frontend/.gitignore.md) |
| `.prettierignore` | configuration | Tells Prettier which files not to format. | [Explanation](./frontend/.prettierignore.md) |
| `.prettierrc` | configuration | Prettier formatting options. | [Explanation](./frontend/.prettierrc.md) |
| `README.md` | documentation | Frontend decisions, conventions and API contract, mixed with unedited Vite template notes. | [Explanation](./frontend/README.md.md) |
| `components.json` | configuration | Configuration for the shadcn/ui CLI that generated the files in `src/components/ui`. | [Explanation](./frontend/components.json.md) |
| `eslint.config.js` | configuration | ESLint flat configuration: strict, type-aware TypeScript rules plus React hooks and fast-refresh rules. | [Explanation](./frontend/eslint.config.js.md) |
| `index.html` | source | The single HTML page of the SPA: mounts point for React and the module entry script. | [Explanation](./frontend/index.html.md) |
| `package-lock.json` | lockfile | npm dependency lockfile (v3). | Skipped: skipped because it is generated dependency-resolution data, not project-authored logic. Exact versions are summarised in docs/PROJECT_OVERVIEW.md section 2. |
| `package.json` | configuration | Declares the frontend's dependencies and npm scripts. | [Explanation](./frontend/package.json.md) |
| `tsconfig.app.json` | configuration | TypeScript settings for the application source under `src/`. | [Explanation](./frontend/tsconfig.app.json.md) |
| `tsconfig.json` | configuration | Solution-style TypeScript config that references the app and node configs. | [Explanation](./frontend/tsconfig.json.md) |
| `tsconfig.node.json` | configuration | TypeScript settings for Node-side config (only `vite.config.ts`). | [Explanation](./frontend/tsconfig.node.json.md) |
| `vite.config.ts` | configuration | Vite configuration: plugins, path alias, dev proxy to the API, and Vitest settings. | [Explanation](./frontend/vite.config.ts.md) |

## `frontend/public`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `favicon.svg` | image/font/media | SVG image (browser tab icon referenced by `index.html`). | Skipped: skipped because it is a media asset. |
| `icons.svg` | image/font/media | SVG sprite of social icons from the Vite template. | Skipped: skipped because it is a media asset (no reference to it was found in `src`). |

## `frontend/src`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `index.css` | source | Global styles: Tailwind v4 imports, shadcn theme tokens (light and dark) and base layer rules. | [Explanation](./frontend/src/index.css.md) |
| `main.tsx` | source | Frontend entry point: creates the router and mounts the React tree. | [Explanation](./frontend/src/main.tsx.md) |
| `routeTree.gen.ts` | generated file | Generated by the TanStack Router Vite plugin from `src/routes/`. | Skipped: skipped because it is generated code that is overwritten on every dev/build run. How it is produced is explained in `frontend/src/routes/__root.tsx`, `routes/index.tsx` and `frontend/vite.config.ts` explanations. |

## `frontend/src/api`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `errorMessages.test.ts` | test | Unit tests of `messageFor` fallback behaviour. | [Explanation](./frontend/src/api/errorMessages.test.ts.md) |
| `errorMessages.ts` | source | Turns an `ApiError` into user-facing text in English or Arabic using a three-level fallback. | [Explanation](./frontend/src/api/errorMessages.ts.md) |
| `errors.ts` | source | TypeScript types for the single error shape the UI will use: `ErrorKind` and `ApiError`. | [Explanation](./frontend/src/api/errors.ts.md) |
| `problemDetails.test.ts` | test | Unit tests for `isProblemDetails`. | [Explanation](./frontend/src/api/problemDetails.test.ts.md) |
| `problemDetails.ts` | source | Type for RFC 9457 Problem Details as the API returns them and a type guard to recognise them. | [Explanation](./frontend/src/api/problemDetails.ts.md) |

## `frontend/src/api/fixtures`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `problem-400.json` | test | Sample Problem Details body for a malformed request (`request.malformed`), matching what `GlobalExceptionHandler` returns. | [Explanation](./frontend/src/api/fixtures/problem-400.json.md) |
| `problem-404.json` | test | Sample Problem Details body for an unknown API route (`route.not_found`). | [Explanation](./frontend/src/api/fixtures/problem-404.json.md) |

## `frontend/src/app`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `queryClient.ts` | source | Creates the single shared TanStack Query client with project-wide defaults. | [Explanation](./frontend/src/app/queryClient.ts.md) |

## `frontend/src/components/ui`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `alert.tsx` | source | shadcn Alert components: `Alert`, `AlertTitle`, `AlertDescription`, `AlertAction`. | [Explanation](./frontend/src/components/ui/alert.tsx.md) |
| `button-variants.ts` | source | The `cva` definition of Button variants and sizes, kept separate from the component. | [Explanation](./frontend/src/components/ui/button-variants.ts.md) |
| `button.tsx` | source | Button component: Base UI's button primitive with project variants. | [Explanation](./frontend/src/components/ui/button.tsx.md) |
| `card.tsx` | source | shadcn Card components (`Card`, `CardHeader`, `CardTitle`, `CardDescription`, `CardAction`, `CardContent`, `CardFooter`). | [Explanation](./frontend/src/components/ui/card.tsx.md) |
| `skeleton.tsx` | source | Pulsing placeholder used while loading. | [Explanation](./frontend/src/components/ui/skeleton.tsx.md) |
| `sonner.tsx` | source | Toast container wrapper (Sonner) themed from `next-themes` and CSS variables. | [Explanation](./frontend/src/components/ui/sonner.tsx.md) |

## `frontend/src/features/status`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `StatusCard.test.tsx` | test | Component tests for `StatusCard` against a mocked network. | [Explanation](./frontend/src/features/status/StatusCard.test.tsx.md) |
| `StatusCard.tsx` | source | Renders the API status in four states: loading, cannot reach API, database unavailable, healthy. | [Explanation](./frontend/src/features/status/StatusCard.tsx.md) |
| `api.ts` | source | The only network call in the frontend: asks `/health/ready` and maps the status code to a typed result. | [Explanation](./frontend/src/features/status/api.ts.md) |
| `useReadiness.ts` | source | TanStack Query hook providing the API readiness state, cached and polled. | [Explanation](./frontend/src/features/status/useReadiness.ts.md) |

## `frontend/src/lib`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `utils.ts` | source | Re-exports the `cn` class-name helper under the conventional shadcn path. | [Explanation](./frontend/src/lib/utils.ts.md) |

## `frontend/src/routes`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `__root.tsx` | source | Root route: the layout wrapping every page, plus the global toast container. | [Explanation](./frontend/src/routes/__root.tsx.md) |
| `index.tsx` | source | The `/` route: the 'System status' page. | [Explanation](./frontend/src/routes/index.tsx.md) |

## `frontend/src/test`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `render.tsx` | test | Test helper that renders a component inside a fresh `QueryClientProvider`. | [Explanation](./frontend/src/test/render.tsx.md) |
| `setup.ts` | test | Vitest setup file: starts MSW, cleans the DOM and resets handlers around tests. | [Explanation](./frontend/src/test/setup.ts.md) |

## `frontend/src/test/msw`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `handlers.ts` | test | Default network behaviour for tests: a healthy API. | [Explanation](./frontend/src/test/msw/handlers.ts.md) |
| `server.ts` | test | Creates the MSW Node server from the default handlers. | [Explanation](./frontend/src/test/msw/server.ts.md) |

## `src`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `.semantic.md` | documentation | Generated description of the four `src/` projects as they were at the skeleton stage. | [Explanation](./src/.semantic.md.md) |

## `src/TutoringCentre.Api`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AssemblyMarker.cs` | source | Assembly handle for architecture tests. | [Explanation](./src/TutoringCentre.Api/AssemblyMarker.cs.md) |
| `Program.cs` | source | Application entry point and composition root: configures logging and services, runs the `seed` CLI mode or builds the HTTP pipeline, and starts the web host. | [Explanation](./src/TutoringCentre.Api/Program.cs.md) |
| `TutoringCentre.Api.csproj` | configuration | Project file for the web host: the composition root and HTTP edge. | [Explanation](./src/TutoringCentre.Api/TutoringCentre.Api.csproj.md) |
| `appsettings.Development.json` | configuration | Development-only overrides of configuration. | [Explanation](./src/TutoringCentre.Api/appsettings.Development.json.md) |
| `appsettings.json` | configuration | Base application configuration: logging levels, allowed hosts and the (empty) connection-string key. | [Explanation](./src/TutoringCentre.Api/appsettings.json.md) |

## `src/TutoringCentre.Api/Cli`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SeedCommand.cs` | source | The `seed` command: creates two demo centres by sending `CreateCentreCommand` through the dispatcher as the system actor. | [Explanation](./src/TutoringCentre.Api/Cli/SeedCommand.cs.md) |

## `src/TutoringCentre.Api/Endpoints`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `PlatformEndpoints.cs` | source | Maps `GET /api/system/info`, the only product endpoint, as bind -> dispatch -> map with no logic. | [Explanation](./src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs.md) |

## `src/TutoringCentre.Api/Http`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CorrelationIdMiddleware.cs` | source | Gives every request a validated correlation ID, exposes it in the response header and in every log line. | [Explanation](./src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs.md) |
| `GlobalExceptionHandler.cs` | source | The single place unexpected exceptions are logged and converted into Problem Details responses without leaking internals. | [Explanation](./src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs.md) |
| `HttpContextKeys.cs` | source | Holds the string key under which the correlation ID is stored in `HttpContext.Items`. | [Explanation](./src/TutoringCentre.Api/Http/HttpContextKeys.cs.md) |
| `ProblemDetailsSetup.cs` | source | Registers Problem Details generation for framework-produced errors and wires the global exception handler. | [Explanation](./src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs.md) |
| `ProblemResult.cs` | source | The single writer of error bodies: serialises a `ProblemDetails` as `application/problem+json` with `traceId` and `correlationId`. | [Explanation](./src/TutoringCentre.Api/Http/ProblemResult.cs.md) |
| `ResultHttpExtensions.cs` | source | The only place domain `Result`/`Error` values become HTTP responses. | [Explanation](./src/TutoringCentre.Api/Http/ResultHttpExtensions.cs.md) |

## `src/TutoringCentre.Api/Logging`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SensitiveDataDestructuringPolicy.cs` | source | Serilog safety net that masks properties named like Password/Token/Secret/ConnectionString/Phone* when an object is logged with `{@Obj}`. | [Explanation](./src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs.md) |

## `src/TutoringCentre.Api/Properties`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `launchSettings.json` | configuration | Local-development launch profiles for `dotnet run` and IDEs. | [Explanation](./src/TutoringCentre.Api/Properties/launchSettings.json.md) |

## `src/TutoringCentre.Application`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AssemblyMarker.cs` | source | Assembly handle used by handler scanning and by architecture tests. | [Explanation](./src/TutoringCentre.Application/AssemblyMarker.cs.md) |
| `DependencyInjection.cs` | source | The Application layer's registration entry point: `AddApplication`, called once from `Program.cs`. | [Explanation](./src/TutoringCentre.Application/DependencyInjection.cs.md) |
| `TutoringCentre.Application.csproj` | configuration | Project file for the use-case layer: references Domain only, plus validation and DI/logging *abstractions*. | [Explanation](./src/TutoringCentre.Application/TutoringCentre.Application.csproj.md) |

## `src/TutoringCentre.Application/Centres`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `ICentreRepository.cs` | source | Write-side persistence port for the Centre aggregate. | [Explanation](./src/TutoringCentre.Application/Centres/ICentreRepository.cs.md) |

## `src/TutoringCentre.Application/Centres/Commands/CreateCentre`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CreateCentreCommand.cs` | source | The message representing the intent to create a centre. | [Explanation](./src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs.md) |
| `CreateCentreHandler.cs` | source | The use case 'create a centre': authorise, check uniqueness, let the domain validate and build the entity, then track it. | [Explanation](./src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs.md) |
| `CreateCentreResult.cs` | source | The success payload of `CreateCentreCommand`: the new centre's id and slug. | [Explanation](./src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreResult.cs.md) |
| `CreateCentreValidator.cs` | source | Shape validation of `CreateCentreCommand` (required fields, maximum lengths, defined enum). | [Explanation](./src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs.md) |

## `src/TutoringCentre.Application/Common/Cqrs`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `Dispatcher.cs` | source | The single entry point for every use case: validates, opens the right kind of transaction, runs the handler, saves once, commits or rolls back, and logs one outcome line. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md) |
| `HandlerRegistration.cs` | source | Reflection scan that registers every command/query handler and validator in the Application assembly. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs.md) |
| `ICommand.cs` | source | Marker interface for commands, carrying the response type as a generic parameter. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/ICommand.cs.md) |
| `ICommandHandler.cs` | source | Contract for a class that executes one command and returns a `Result`. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/ICommandHandler.cs.md) |
| `IQuery.cs` | source | Marker interface for read-only requests. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/IQuery.cs.md) |
| `IQueryHandler.cs` | source | Contract for a class that answers one query inside a read-only transaction. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs.md) |
| `Unit.cs` | source | Placeholder 'no data' response type for commands that only succeed or fail. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/Unit.cs.md) |

## `src/TutoringCentre.Application/Common/Ports`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `IClock.cs` | source | Port for the current time and for converting between UTC instants and local wall-clock time in an IANA time zone. | [Explanation](./src/TutoringCentre.Application/Common/Ports/IClock.cs.md) |
| `IUnitOfWork.cs` | source | Port over the persistence transaction: begin (read-only or read-write), save, commit, rollback. | [Explanation](./src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs.md) |

## `src/TutoringCentre.Application/Common/Security`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `Actor.cs` | source | Defines who is executing a use case: the abstract `Actor`, the `SystemActor` and the `AnonymousActor`. | [Explanation](./src/TutoringCentre.Application/Common/Security/Actor.cs.md) |
| `CurrentActorContext.cs` | source | Holds the actor for one scope; starts anonymous and may be set exactly once. | [Explanation](./src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs.md) |
| `ICurrentActor.cs` | source | Read-only view of the actor of the current scope. | [Explanation](./src/TutoringCentre.Application/Common/Security/ICurrentActor.cs.md) |

## `src/TutoringCentre.Application/Platform`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `ISystemInfoReadService.cs` | source | Read-side port that returns database schema status as plain data, plus the `SchemaStatus` record. | [Explanation](./src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs.md) |
| `SystemInfoDto.cs` | source | The public response shape of `GET /api/system/info`: application version, latest migration, up-to-date flag. | [Explanation](./src/TutoringCentre.Application/Platform/SystemInfoDto.cs.md) |

## `src/TutoringCentre.Application/Platform/Queries/GetSystemInfo`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `GetSystemInfoHandler.cs` | source | Builds `SystemInfoDto` from the read service and from the assembly's informational version. | [Explanation](./src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs.md) |
| `GetSystemInfoQuery.cs` | source | The parameterless query asking for application version and migration status. | [Explanation](./src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs.md) |

## `src/TutoringCentre.Domain`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AssemblyMarker.cs` | source | An empty internal class used only so tests can obtain this assembly with `typeof(AssemblyMarker).Assembly`. | [Explanation](./src/TutoringCentre.Domain/AssemblyMarker.cs.md) |
| `TutoringCentre.Domain.csproj` | configuration | Project file for the innermost layer. | [Explanation](./src/TutoringCentre.Domain/TutoringCentre.Domain.csproj.md) |

## `src/TutoringCentre.Domain/Centres`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `Centre.cs` | source | The only entity in the system: a tutoring centre (the tenant). | [Explanation](./src/TutoringCentre.Domain/Centres/Centre.cs.md) |
| `SupportedLocale.cs` | source | Enum of languages a centre can use by default: `Ar` and `En`. | [Explanation](./src/TutoringCentre.Domain/Centres/SupportedLocale.cs.md) |

## `src/TutoringCentre.Domain/Common`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `Entity.cs` | source | Base class for domain entities: gives each one a UUIDv7 identity at construction. | [Explanation](./src/TutoringCentre.Domain/Common/Entity.cs.md) |
| `Error.cs` | source | Immutable description of an expected business failure: a stable code, a developer message, a kind and optional per-field messages. | [Explanation](./src/TutoringCentre.Domain/Common/Error.cs.md) |
| `ErrorKind.cs` | source | Closed enum of failure categories; each maps to exactly one HTTP status. | [Explanation](./src/TutoringCentre.Domain/Common/ErrorKind.cs.md) |
| `ITenantOwned.cs` | source | Marker interface for future entities that belong to one centre (tenant). | [Explanation](./src/TutoringCentre.Domain/Common/ITenantOwned.cs.md) |
| `Result.cs` | source | Defines `Result` and `Result<T>`, the return types for operations that can fail for expected business reasons. | [Explanation](./src/TutoringCentre.Domain/Common/Result.cs.md) |

## `src/TutoringCentre.Infrastructure`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AssemblyMarker.cs` | source | Assembly handle for architecture tests. | [Explanation](./src/TutoringCentre.Infrastructure/AssemblyMarker.cs.md) |
| `DependencyInjection.cs` | source | Infrastructure's registration entry point: wires every Application port to its PostgreSQL/EF/system implementation and configures the database, health check and options. | [Explanation](./src/TutoringCentre.Infrastructure/DependencyInjection.cs.md) |
| `TutoringCentre.Infrastructure.csproj` | configuration | Project file for the adapter layer: references Application and Domain and brings in EF Core, the Npgsql provider and the health check. | [Explanation](./src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj.md) |

## `src/TutoringCentre.Infrastructure/Persistence`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AppDbContext.cs` | source | The single EF Core database context for the whole application. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs.md) |
| `DatabaseOptions.cs` | source | Typed settings object holding the database connection string, validated at startup. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs.md) |
| `MigrationRunner.cs` | source | Extension method that applies pending EF Core migrations from an `IServiceProvider`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs.md) |
| `Schemas.cs` | source | Constants naming the PostgreSQL schemas used per feature module. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Schemas.cs.md) |
| `UnitOfWork.cs` | source | Implements the `IUnitOfWork` port with an explicit database transaction on the scoped `AppDbContext`, including PostgreSQL read-only transactions for queries. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CentreConfiguration.cs` | source | Maps the `Centre` entity to the `platform.centres` table: columns, limits, key, unique index, locale conversion, CHECK constraint and timestamp shadow properties. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Interceptors`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `TimestampInterceptor.cs` | source | Automatically fills `CreatedAt` and `UpdatedAt` shadow columns just before EF saves. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Migrations`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `20261002222404_InitialPlatform.Designer.cs` | generated file | EF Core generated target-model snapshot for the migration. | Skipped: skipped because it is tool output. The migration itself is explained in `20261002222404_InitialPlatform.cs.md`. |
| `20261002222404_InitialPlatform.cs` | migration | The only migration: creates the `platform` schema, the `centres` table, its primary key, locale CHECK constraint and unique slug index. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.cs.md) |
| `AppDbContextModelSnapshot.cs` | generated file | EF Core generated current-model snapshot (`// <auto-generated />`). | Skipped: skipped because it is tool output. Its role (diff base, checked by CI `has-pending-model-changes`) is explained in the migration, `CentreConfiguration` and `ci.yml` explanations. |

## `src/TutoringCentre.Infrastructure/ReadServices`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SystemInfoReadService.cs` | source | Read-side implementation that reports applied and pending EF migrations as plain data. | [Explanation](./src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs.md) |

## `src/TutoringCentre.Infrastructure/Repositories`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CentreRepository.cs` | source | EF Core implementation of `ICentreRepository`. | [Explanation](./src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs.md) |

## `src/TutoringCentre.Infrastructure/Time`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SystemClock.cs` | source | Real implementation of `IClock` based on `TimeProvider` and the OS time-zone database. | [Explanation](./src/TutoringCentre.Infrastructure/Time/SystemClock.cs.md) |

## `tests`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `.semantic.md` | documentation | Generated description of the five test projects at the skeleton stage. | [Explanation](./tests/.semantic.md.md) |

## `tests/TutoringCentre.Api.Tests`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `TutoringCentre.Api.Tests.csproj` | configuration | Project file for HTTP-level tests that host the real Api in-process. | [Explanation](./tests/TutoringCentre.Api.Tests/TutoringCentre.Api.Tests.csproj.md) |

## `tests/TutoringCentre.Api.Tests/Cli`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SeedCommandTests.cs` | test | Proves the seed command is idempotent and creates exactly the two demo centres. | [Explanation](./tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Fixtures`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `ApiCollection.cs` | test | xUnit collection for tests sharing `ApiFactory`. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs.md) |
| `ApiFactory.cs` | test | Hosts the real Api in-process against a throw-away PostgreSQL 17 container, for database-backed HTTP/CLI tests. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs.md) |
| `ConventionEndpoints.cs` | test | Defines the test-only `/api/test/*` endpoints used to trigger each error kind, strict-JSON failures and sensitive-data logging. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs.md) |
| `ConventionEndpointsStartupFilter.cs` | test | `IStartupFilter` that appends the test-only endpoints after the real middleware pipeline. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs.md) |
| `ConventionTestTypes.cs` | test | Test-only commands, handlers and validator that produce every error kind on demand. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs.md) |
| `ConventionsCollection.cs` | test | xUnit collection for tests sharing `ConventionsFactory`. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs.md) |
| `ConventionsFactory.cs` | test | The real Api plus test-only endpoints, handlers and an in-memory log sink, used to test HTTP conventions. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs.md) |
| `InMemoryLogSink.cs` | test | Serilog sink that stores log events in memory so tests can assert on logging. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs.md) |

## `tests/TutoringCentre.Api.Tests/Health`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `HealthEndpointTests.cs` | test | Tests liveness and readiness endpoints without a database. | [Explanation](./tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs.md) |
| `ReadinessWithDatabaseTests.cs` | test | Tests that readiness returns 200 when a real database is reachable. | [Explanation](./tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Http`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CorrelationIdTests.cs` | test | Tests the correlation-ID middleware's generation, validation and propagation. | [Explanation](./tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs.md) |
| `LoggingConventionTests.cs` | test | Tests log levels for successful and failing requests. | [Explanation](./tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs.md) |
| `ProblemDetailsTests.cs` | test | Tests the HTTP error conventions end to end through the real pipeline. | [Explanation](./tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs.md) |
| `ResultHttpExtensionsTests.cs` | test | Unit tests of the `Result` -> HTTP mapping without starting a host. | [Explanation](./tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Logging`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SensitiveDataDestructuringPolicyTests.cs` | test | Tests that the policy masks sensitive properties and leaves others alone. | [Explanation](./tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs.md) |

## `tests/TutoringCentre.Application.Tests`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `TutoringCentre.Application.Tests.csproj` | configuration | Project file for Application unit tests; references Application and the DI package used to build a service provider in `DispatcherTests`. | [Explanation](./tests/TutoringCentre.Application.Tests/TutoringCentre.Application.Tests.csproj.md) |

## `tests/TutoringCentre.Application.Tests/Centres`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CreateCentreHandlerTests.cs` | test | Unit tests for `CreateCentreHandler` branches using fakes. | [Explanation](./tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs.md) |
| `CreateCentreValidatorTests.cs` | test | Unit tests for `CreateCentreValidator`. | [Explanation](./tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs.md) |

## `tests/TutoringCentre.Application.Tests/Cqrs`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `DispatcherTests.cs` | test | Tests every branch of the dispatcher pipeline using a call-recording fake unit of work. | [Explanation](./tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs.md) |
| `TestRequests.cs` | test | Test-only commands, queries, handlers and validators used to drive `Dispatcher` in isolation. | [Explanation](./tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs.md) |

## `tests/TutoringCentre.Application.Tests/Fakes`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `FakeCentreRepository.cs` | test | In-memory `ICentreRepository` with pre-seeded rows and a record of additions. | [Explanation](./tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs.md) |
| `FakeUnitOfWork.cs` | test | In-memory `IUnitOfWork` that records every call in order. | [Explanation](./tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs.md) |

## `tests/TutoringCentre.Application.Tests/Platform`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `GetSystemInfoHandlerTests.cs` | test | Unit tests for `GetSystemInfoHandler` with a private fake read service. | [Explanation](./tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs.md) |

## `tests/TutoringCentre.Application.Tests/Security`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CurrentActorContextTests.cs` | test | Tests the actor holder's default and set-once behaviour. | [Explanation](./tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs.md) |

## `tests/TutoringCentre.Architecture.Tests`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `DependencyRuleTests.cs` | test | Type-level architecture tests: fail when compiled code in a layer uses a forbidden namespace. | [Explanation](./tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md) |
| `ProjectFiles.cs` | test | Helper that reads the `ProjectReference` items of a `src` project from its `.csproj` XML. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs.md) |
| `ProjectReferenceTests.cs` | test | Declared-reference architecture tests: each `src` project's `ProjectReference` list must equal the allowed set. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md) |
| `TutoringCentre.Architecture.Tests.csproj` | configuration | Project file for the architecture tests; references all four `src` projects and the NetArchTest library. | [Explanation](./tests/TutoringCentre.Architecture.Tests/TutoringCentre.Architecture.Tests.csproj.md) |

## `tests/TutoringCentre.Domain.Tests`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `TutoringCentre.Domain.Tests.csproj` | configuration | Project file for the Domain unit tests; references only the Domain project. | [Explanation](./tests/TutoringCentre.Domain.Tests/TutoringCentre.Domain.Tests.csproj.md) |

## `tests/TutoringCentre.Domain.Tests/Centres`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CentreTests.cs` | test | Unit tests for the `Centre.Create` business rules. | [Explanation](./tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs.md) |

## `tests/TutoringCentre.Domain.Tests/Common`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `EntityTests.cs` | test | Tests the `Entity` base class's identity generation. | [Explanation](./tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs.md) |
| `ErrorTests.cs` | test | Tests the two `Error.Validation` overloads. | [Explanation](./tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs.md) |
| `ResultTests.cs` | test | Tests `Result`/`Result<T>` invariants. | [Explanation](./tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `TutoringCentre.Infrastructure.Tests.csproj` | configuration | Project file for database-backed Infrastructure tests. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/TutoringCentre.Infrastructure.Tests.csproj.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Centres`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CentreSlugRaceTests.cs` | test | Proves that two simultaneous creates with the same slug leave exactly one row, because the unique index is the real guarantee. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs.md) |
| `CreateCentreTests.cs` | test | Integration tests of the create-centre use case through the real dispatcher and PostgreSQL. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Fixtures`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `DispatchExtensions.cs` | test | Test helpers that dispatch a command or query in a fresh scope as a chosen actor. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs.md) |
| `PostgresCollection.cs` | test | xUnit collection definition that makes all database tests share one `PostgresFixture`. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs.md) |
| `PostgresFixture.cs` | test | Shared test fixture: starts one PostgreSQL 17 container per test run, applies the real migrations, builds a production-identical DI container and resets data between tests with Respawn. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs.md) |
| `PostgresTestBase.cs` | test | Base class for database tests: puts the class into the postgres collection and resets all rows before each test. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Persistence`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `MigrationTests.cs` | test | Proves a brand-new database built only from the migrations has the expected objects. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs.md) |
| `ReadOnlyQueryTests.cs` | test | Proves the database itself refuses writes made inside a query handler. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs.md) |
| `TransactionBoundaryTests.cs` | test | Proves rollback on exception and on failure results, with a success control. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Platform`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SystemInfoQueryTests.cs` | test | End-to-end test of `GetSystemInfoQuery` against a migrated database. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Time`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SystemClockTests.cs` | test | Unit tests for `SystemClock` using `FakeTimeProvider`. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs.md) |
