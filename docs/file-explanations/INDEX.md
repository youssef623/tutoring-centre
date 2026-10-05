# Repository file index

Every tracked file in the repository (`git ls-files`) except the explanation set itself (`docs/file-explanations/**`), grouped by directory. **282 files**: **273** have an explanation file under this folder (mirroring the repository path plus `.md`) and **9** are intentionally skipped (generated/lockfile/media) with the reason given. The folder `docs/file-explanations/` (this index, the seven area guides and one explanation per file above) is not listed because it describes the other files.

Excluded from the inventory: `.git/`, `node_modules/` (untracked; created by `npm ci`), and build output (`bin/`, `obj/`, `dist/`, `playwright-report/`, `test-results/`) - none is tracked. Categories used: source, test, configuration, documentation, migration, database/schema, script, CI/CD, Docker/infrastructure, lockfile, generated file, binary, image/font/media, vendored dependency, other. Categories with no file in this repository: database/schema, binary, vendored dependency, other (the database schema is defined by the EF configuration classes and the migrations, not by a standalone schema file).

Companion documents: [`../PROJECT_OVERVIEW2.md`](../PROJECT_OVERVIEW2.md) (the current teaching overview, covering the merged authentication work), [`../PROJECT_OVERVIEW.md`](../PROJECT_OVERVIEW.md) (the earlier overview, kept unchanged) and the area guides [01 root/infra](01-root-and-infrastructure.md), [02 domain](02-domain.md), [03 application](03-application.md), [04 infrastructure](04-infrastructure.md), [05 api](05-api.md), [06 backend tests](06-backend-tests.md), [07 frontend](07-frontend.md), which list whole folders.

## Counts by category

| Category | Files |
| --- | --- |
| CI/CD | 4 |
| Docker/infrastructure | 1 |
| configuration | 33 |
| documentation | 20 |
| generated file | 6 |
| image/font/media | 2 |
| lockfile | 1 |
| migration | 2 |
| script | 1 |
| source | 134 |
| test | 78 |
| **total** | **282** |

## `(repository root)`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `.editorconfig` | configuration | Declares formatting and a few diagnostic rules that editors and the compiler apply consistently across the repository. | [Explanation](./.editorconfig.md) |
| `.env.example` | configuration | Template for the git-ignored `.env` file that Docker Compose reads to configure the local PostgreSQL container. | [Explanation](./.env.example.md) |
| `.gitattributes` | configuration | Forces Git to normalise text files to LF so Windows and Linux contributors do not create line-ending diffs, and pins the generated API artefacts to LF. | [Explanation](./.gitattributes.md) |
| `.gitignore` | configuration | Lists files Git must not track: build output, IDE files, secrets (`.env`), dependency folders. | [Explanation](./.gitignore.md) |
| `.semantic-manifest.json` | documentation | Bookkeeping file for a documentation generator called semantic-git: records which generated docs cover which folders and at which commit they were last refreshed. | [Explanation](./.semantic-manifest.json.md) |
| `ARCHITECTURE.semantic.md` | documentation | Generated architecture description focused on the dependency rule, architecture tests and health checks. | [Explanation](./ARCHITECTURE.semantic.md.md) |
| `Directory.Build.props` | configuration | Applies the same compiler settings to every .NET project in the repository without repeating them in each `.csproj`. | [Explanation](./Directory.Build.props.md) |
| `Directory.Packages.props` | configuration | Single source of truth for every NuGet package version (Central Package Management). | [Explanation](./Directory.Packages.props.md) |
| `OVERVIEW.semantic.md` | documentation | Generated overview written when the repo had only the four-project skeleton and health checks. | [Explanation](./OVERVIEW.semantic.md.md) |
| `README.md` | documentation | Human entry point: what the product is meant to be, how to run it locally (database, HTTPS API, seeding, frontend, tests, E2E), the architecture diagram, decisions and the quality gates. | [Explanation](./README.md.md) |
| `TutoringCentre.slnx` | configuration | Solution file listing the nine .NET projects so `dotnet build/test TutoringCentre.slnx` operates on the whole repo. | [Explanation](./TutoringCentre.slnx.md) |
| `api.http` | documentation | A manual walkthrough of the session API for VS Code's REST Client extension: CSRF token, login as two users, centre selection, logout with and without a token. | [Explanation](./api.http.md) |
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
| `ci.yml` | CI/CD | Main continuous-integration workflow: builds and tests the backend and the frontend, then runs the Playwright end-to-end journeys, on every pull request and on pushes to `main`. | [Explanation](./.github/workflows/ci.yml.md) |
| `codeql.yml` | CI/CD | Runs GitHub's CodeQL static security analysis on the C# and TypeScript code. | [Explanation](./.github/workflows/codeql.yml.md) |
| `secret-scan.yml` | CI/CD | Scans the full git history for committed secrets with gitleaks. | [Explanation](./.github/workflows/secret-scan.yml.md) |

## `docs`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `PROJECT_OVERVIEW.md` | documentation | The original teaching overview of the repository (15 sections plus two appendices), written against the commit before the authentication work was merged and kept unchanged as that earlier snapshot. | [Explanation](./docs/PROJECT_OVERVIEW.md.md) |
| `PROJECT_OVERVIEW2.md` | documentation | The current teaching overview of the repository: the original 15 sections and appendices, corrected in place and extended for the merged authentication, session, CSRF, rate-limiting, membership, OpenAPI-client, internationalisation and end-to-end-testing work. | [Explanation](./docs/PROJECT_OVERVIEW2.md.md) |

## `docs/adr`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `0001-clean-architecture.md` | documentation | Architecture Decision Record explaining why the backend is four projects with inward-only dependencies, plus the alternatives that were rejected. | [Explanation](./docs/adr/0001-clean-architecture.md.md) |
| `0002-dotnet-react.md` | documentation | ADR recording the choice of ASP.NET Core + EF Core + PostgreSQL for the backend and a React + TypeScript Vite SPA for the frontend. | [Explanation](./docs/adr/0002-dotnet-react.md.md) |
| `0003-custom-cqrs-dispatcher.md` | documentation | Architecture Decision Record: why the CQRS pipeline is a small hand-written `Dispatcher` instead of MediatR or decorators. | [Explanation](./docs/adr/0003-custom-cqrs-dispatcher.md.md) |
| `0004-unit-of-work-port.md` | documentation | Architecture Decision Record: why Application defines a thin `IUnitOfWork` port that only the dispatcher calls. | [Explanation](./docs/adr/0004-unit-of-work-port.md.md) |
| `0005-cookie-authentication.md` | documentation | Architecture Decision Record: why sessions are an encrypted `HttpOnly` `__Host-` cookie with antiforgery tokens and per-request revalidation, rather than JWT. | [Explanation](./docs/adr/0005-cookie-authentication.md.md) |

## `docs/architecture`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `api-conventions.md` | documentation | Authoritative description of the HTTP conventions: error shape, Result-to-status mapping, exception handling, strict JSON, correlation IDs, log redaction, routing and the first endpoint. | [Explanation](./docs/architecture/api-conventions.md.md) |
| `authentication.md` | documentation | Authoritative description of the authentication flow: login, centre selection, per-request pipeline, anonymous routes, logout, cookie and claims, CSRF, revalidation, lockout and rate limiting, error codes, and what is deferred. | [Explanation](./docs/architecture/authentication.md.md) |
| `overview.md` | documentation | Explains the failure model (Result vs exceptions), the full request pipeline (middleware, authentication, actor, authorization, rate limiting, CSRF filter, dispatcher), the full path of `CreateCentreCommand` from CLI to table, the path of `GET /api/system/info` from HTTP to table, and links the ADRs. | [Explanation](./docs/architecture/overview.md.md) |

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
| `index.html` | source | The single HTML page of the SPA: mount point for React and the module entry script. | [Explanation](./frontend/index.html.md) |
| `orval.config.ts` | configuration | Configuration of Orval, the tool that generates the typed React Query client from the API's OpenAPI document. | [Explanation](./frontend/orval.config.ts.md) |
| `package-lock.json` | lockfile | npm dependency lockfile (v3). | Skipped: skipped because it is generated dependency-resolution data, not project-authored logic. Exact versions are summarised in docs/PROJECT_OVERVIEW2.md section 2. |
| `package.json` | configuration | Declares the frontend's dependencies and npm scripts. | [Explanation](./frontend/package.json.md) |
| `playwright.config.ts` | configuration | Playwright configuration: where the E2E specs are, which browser, and how to start the API and the Vite dev server before the tests. | [Explanation](./frontend/playwright.config.ts.md) |
| `tsconfig.app.json` | configuration | TypeScript settings for the application source under `src/`. | [Explanation](./frontend/tsconfig.app.json.md) |
| `tsconfig.json` | configuration | Solution-style TypeScript config that references the app and node configs. | [Explanation](./frontend/tsconfig.json.md) |
| `tsconfig.node.json` | configuration | TypeScript settings for Node-side config (`vite.config.ts`, `orval.config.ts`, `playwright.config.ts` and the `e2e` folder). | [Explanation](./frontend/tsconfig.node.json.md) |
| `vite.config.ts` | configuration | Vite configuration: plugins, path alias, dev proxy to the HTTPS API, and Vitest settings. | [Explanation](./frontend/vite.config.ts.md) |

## `frontend/e2e`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `auth.spec.ts` | test | Playwright end-to-end tests of the three authentication journeys against the real API, real PostgreSQL and the Vite dev server. | [Explanation](./frontend/e2e/auth.spec.ts.md) |

## `frontend/openapi`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `TutoringCentre.Api.json` | generated file | OpenAPI 3 document generated by the Api build (`Microsoft.Extensions.ApiDescription.Server`, settings in `TutoringCentre.Api.csproj`) and committed so CI can detect staleness. | Skipped: skipped because it is generated output. Its role is explained in `src/TutoringCentre.Api/TutoringCentre.Api.csproj`, `frontend/orval.config.ts` and `.github/workflows/ci.yml` explanations. |

## `frontend/public`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `favicon.svg` | image/font/media | SVG image (the Hessa brand tab icon referenced by `index.html`. | Skipped: replaced by the merge); skipped because it is a media asset. |
| `icons.svg` | image/font/media | SVG sprite of social icons from the Vite template. | Skipped: skipped because it is a media asset (no reference to it was found in `src`). |

## `frontend/scripts`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `check-i18n-keys.mjs` | script | Node script that fails when the English and Arabic translation files do not have exactly the same namespaces and keys. | [Explanation](./frontend/scripts/check-i18n-keys.mjs.md) |

## `frontend/src`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `index.css` | source | Global styles: Tailwind v4 imports, the Hessa brand theme tokens (light and dark), fonts and base layer rules. | [Explanation](./frontend/src/index.css.md) |
| `main.tsx` | source | Frontend entry point: creates the router with the shared query client in its context, wires the session-expiry redirect, imports i18n and mounts the React tree. | [Explanation](./frontend/src/main.tsx.md) |
| `routeTree.gen.ts` | generated file | Generated by the TanStack Router Vite plugin from `src/routes/`. | Skipped: skipped because it is generated code that is overwritten on every dev/build run. How it is produced is explained in the `frontend/src/routes/__root.tsx`, `frontend/src/routes/_authenticated.tsx` and `frontend/vite.config.ts` explanations. |

## `frontend/src/api`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `apiFetch.test.ts` | test | Tests of `apiFetch` and `toApiError`: success and 204 handling, Problem Details mapping, network failure, non-JSON errors, CSRF header attachment and the single retry. | [Explanation](./frontend/src/api/apiFetch.test.ts.md) |
| `apiFetch.ts` | source | The only function that talks to the API: wraps `fetch`, attaches the CSRF header to unsafe methods, retries once on a stale token, and turns every failure into a typed `ApiError`. | [Explanation](./frontend/src/api/apiFetch.ts.md) |
| `csrf.ts` | source | In-memory cache of the antiforgery token, fetched through the generated client and refreshed on demand. | [Explanation](./frontend/src/api/csrf.ts.md) |
| `errorMessages.test.ts` | test | Unit tests of `messageFor` fallback behaviour (English resources). | [Explanation](./frontend/src/api/errorMessages.test.ts.md) |
| `errorMessages.ts` | source | Turns an `ApiError` into user-facing text in the current language via i18next, with a three-level fallback. | [Explanation](./frontend/src/api/errorMessages.ts.md) |
| `errors.ts` | source | TypeScript types for the single error shape the UI uses: `ErrorKind` and `ApiError`. | [Explanation](./frontend/src/api/errors.ts.md) |
| `problemDetails.test.ts` | test | Unit tests for `isProblemDetails`. | [Explanation](./frontend/src/api/problemDetails.test.ts.md) |
| `problemDetails.ts` | source | Type for RFC 9457 Problem Details as the API returns them and a type guard to recognise them. | [Explanation](./frontend/src/api/problemDetails.ts.md) |
| `showApiError.test.ts` | test | Tests of the toast helper and of `applyFieldErrors`. | [Explanation](./frontend/src/api/showApiError.test.ts.md) |
| `showApiError.ts` | source | UI helpers for API failures: a toast with the translated message and correlation reference, and mapping server field errors onto form fields. | [Explanation](./frontend/src/api/showApiError.ts.md) |

## `frontend/src/api/fixtures`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `problem-400.json` | test | Sample Problem Details body for a malformed request (`request.malformed`), matching what `GlobalExceptionHandler` returns. | [Explanation](./frontend/src/api/fixtures/problem-400.json.md) |
| `problem-404.json` | test | Sample Problem Details body for an unknown API route (`route.not_found`). | [Explanation](./frontend/src/api/fixtures/problem-404.json.md) |

## `frontend/src/api/generated`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `tutoring-centre.ts` | generated file | Orval-generated React Query client (types plus hooks such as `useLogin`, `useGetMe`, `useSelectCentre`, `useGetSystemInfo`, `getAntiforgeryToken`). | Skipped: skipped because it is generated code that is overwritten by `npm run generate:api`. How it is produced and consumed is explained in `frontend/orval.config.ts` and `frontend/src/api/apiFetch.ts` explanations. |

## `frontend/src/app`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `queryClient.ts` | source | Creates the shared TanStack Query client with project-wide defaults and global handling of expired sessions. | [Explanation](./frontend/src/app/queryClient.ts.md) |

## `frontend/src/components/brand`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `HessaLogo.tsx` | source | The Hessa brand mark and wordmark as React components (inline SVG). | [Explanation](./frontend/src/components/brand/HessaLogo.tsx.md) |

## `frontend/src/components/ui`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `alert.tsx` | source | shadcn Alert components: `Alert`, `AlertTitle`, `AlertDescription`, `AlertAction`. | [Explanation](./frontend/src/components/ui/alert.tsx.md) |
| `badge-variants.ts` | source | The `cva` variant table (class names) for the Badge component, kept in its own file. | [Explanation](./frontend/src/components/ui/badge-variants.ts.md) |
| `badge.tsx` | source | shadcn Badge component: a small pill used to show roles (owner, teacher, assistant) and status text. | [Explanation](./frontend/src/components/ui/badge.tsx.md) |
| `button-variants.ts` | source | The `cva` definition of Button variants and sizes, kept separate from the component. | [Explanation](./frontend/src/components/ui/button-variants.ts.md) |
| `button.tsx` | source | Button component: Base UI's button primitive with project variants. | [Explanation](./frontend/src/components/ui/button.tsx.md) |
| `card.tsx` | source | shadcn Card components (`Card`, `CardHeader`, `CardTitle`, `CardDescription`, `CardAction`, `CardContent`, `CardFooter`). | [Explanation](./frontend/src/components/ui/card.tsx.md) |
| `skeleton.tsx` | source | Pulsing placeholder used while loading. | [Explanation](./frontend/src/components/ui/skeleton.tsx.md) |
| `sonner.tsx` | source | Toast container wrapper (Sonner) themed from `next-themes` and CSS variables. | [Explanation](./frontend/src/components/ui/sonner.tsx.md) |

## `frontend/src/features/language`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `LanguageSwitcher.test.tsx` | test | Tests that switching language sets the document direction, `lang` attribute and the stored preference. | [Explanation](./frontend/src/features/language/LanguageSwitcher.test.tsx.md) |
| `LanguageSwitcher.tsx` | source | A button that toggles the UI language between Arabic and English. | [Explanation](./frontend/src/features/language/LanguageSwitcher.tsx.md) |

## `frontend/src/features/session`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `meQueryOptions.ts` | source | TanStack Query options for `GET /api/me` that treat 'signed out' as data (`null`), not as an error. | [Explanation](./frontend/src/features/session/meQueryOptions.ts.md) |
| `useSession.test.ts` | test | Tests that `useSession` resolves to `null` on 401, to the profile on success, and reports a real error on a 500. | [Explanation](./frontend/src/features/session/useSession.test.ts.md) |
| `useSession.ts` | source | Hook returning the signed-in session: `me` (profile or `null`), loading and error flags. | [Explanation](./frontend/src/features/session/useSession.ts.md) |
| `useSignOut.ts` | source | Hook returning a function that logs out, refreshes the CSRF token, clears all cached queries and navigates to `/login`. | [Explanation](./frontend/src/features/session/useSignOut.ts.md) |

## `frontend/src/features/shell`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AppShell.tsx` | source | The authenticated page frame: desktop sidebar, mobile slide-in navigation, header with active centre and role, and the user menu (switch centre, language, log out). | [Explanation](./frontend/src/features/shell/AppShell.tsx.md) |

## `frontend/src/features/status`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `StatusCard.test.tsx` | test | Component tests for `StatusCard` against a mocked network. | [Explanation](./frontend/src/features/status/StatusCard.test.tsx.md) |
| `StatusCard.tsx` | source | Renders the API status in four states: loading, cannot reach API, database unavailable, healthy. | [Explanation](./frontend/src/features/status/StatusCard.tsx.md) |
| `SystemInfoCard.test.tsx` | test | Tests the system-information card on success and on a 500 Problem Details response. | [Explanation](./frontend/src/features/status/SystemInfoCard.test.tsx.md) |
| `SystemInfoCard.tsx` | source | Card showing application version, latest migration and whether the schema is up to date, via the generated client's `useGetSystemInfo`. | [Explanation](./frontend/src/features/status/SystemInfoCard.tsx.md) |
| `api.ts` | source | A hand-written network call kept for the readiness check (the rest of the app uses the generated client through `apiFetch`): asks `/health/ready` and maps the status code to a typed result. | [Explanation](./frontend/src/features/status/api.ts.md) |
| `useReadiness.ts` | source | TanStack Query hook providing the API readiness state, cached and polled. | [Explanation](./frontend/src/features/status/useReadiness.ts.md) |

## `frontend/src/i18n`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `index.ts` | source | Initialises i18next: bundled English and Arabic resources, ICU message format, language detection, and synchronisation of `<html lang dir>` and `localStorage`. | [Explanation](./frontend/src/i18n/index.ts.md) |

## `frontend/src/i18n/locales/ar`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `auth.json` | source | Arabic strings for the login page, the centre picker and role names (namespace `auth`). | [Explanation](./frontend/src/i18n/locales/ar/auth.json.md) |
| `common.json` | source | Arabic strings shared across screens (namespace `common`, the default). | [Explanation](./frontend/src/i18n/locales/ar/common.json.md) |
| `errors.json` | source | Arabic error messages keyed by backend error `code` and `kind` (namespace `errors`). | [Explanation](./frontend/src/i18n/locales/ar/errors.json.md) |
| `shell.json` | source | Arabic strings for the authenticated app shell and dashboard (namespace `shell`). | [Explanation](./frontend/src/i18n/locales/ar/shell.json.md) |
| `status.json` | source | Arabic strings for the API status card and the system-information card (namespace `status`). | [Explanation](./frontend/src/i18n/locales/ar/status.json.md) |

## `frontend/src/i18n/locales/en`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `auth.json` | source | English strings for the login page, the centre picker and role names (namespace `auth`). | [Explanation](./frontend/src/i18n/locales/en/auth.json.md) |
| `common.json` | source | English strings shared across screens (namespace `common`, the default). | [Explanation](./frontend/src/i18n/locales/en/common.json.md) |
| `errors.json` | source | English error messages keyed by backend error `code` and `kind` (namespace `errors`). | [Explanation](./frontend/src/i18n/locales/en/errors.json.md) |
| `shell.json` | source | English strings for the authenticated app shell and dashboard (namespace `shell`). | [Explanation](./frontend/src/i18n/locales/en/shell.json.md) |
| `status.json` | source | English strings for the API status card and the system-information card (namespace `status`). | [Explanation](./frontend/src/i18n/locales/en/status.json.md) |

## `frontend/src/lib`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `utils.ts` | source | Re-exports the `cn` class-name helper under the conventional shadcn path. | [Explanation](./frontend/src/lib/utils.ts.md) |

## `frontend/src/routes`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `__root.tsx` | source | Root route: the layout wrapping every page, the typed router context, and the global toast container. | [Explanation](./frontend/src/routes/__root.tsx.md) |
| `_authenticated.test.tsx` | test | Component-level tests of the protected layout guard and of global 401 / logout handling, using the real route tree. | [Explanation](./frontend/src/routes/_authenticated.test.tsx.md) |
| `_authenticated.tsx` | source | The pathless protected layout route: the session guard (`beforeLoad`) and the `AppShell` frame for every page below it. | [Explanation](./frontend/src/routes/_authenticated.tsx.md) |
| `login.test.tsx` | test | Tests of the login page: empty submit, generic 401 message, 429 message, successful redirect and rejection of an external redirect. | [Explanation](./frontend/src/routes/login.test.tsx.md) |
| `login.tsx` | source | The `/login` page: a validated email/password form, error handling for credentials and rate limiting, a safe return-URL redirect, and the Hessa brand panel. | [Explanation](./frontend/src/routes/login.tsx.md) |
| `select-centre.test.tsx` | test | Tests of the centre picker: listing, choosing a centre, and the empty state. | [Explanation](./frontend/src/routes/select-centre.test.tsx.md) |
| `select-centre.tsx` | source | The `/select-centre` page: lists the user's active centres with their role, posts the choice, or shows an empty state with a log-out button. | [Explanation](./frontend/src/routes/select-centre.tsx.md) |
| `status.tsx` | source | The `/status` page: the former status screen (API readiness card and system-information card) moved to its own public route. | [Explanation](./frontend/src/routes/status.tsx.md) |

## `frontend/src/routes/_authenticated`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `index.tsx` | source | The dashboard page at `/` (inside the protected layout): a welcome heading, the active centre name and a placeholder card. | [Explanation](./frontend/src/routes/_authenticated/index.tsx.md) |

## `frontend/src/test`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `render.tsx` | test | Test helpers that render a component or a hook inside a fresh `QueryClientProvider`. | [Explanation](./frontend/src/test/render.tsx.md) |
| `renderRouter.tsx` | test | Test helper that renders the app's real route tree in memory with a fresh query client wired like production, including the session-expiry redirect. | [Explanation](./frontend/src/test/renderRouter.tsx.md) |
| `setup.ts` | test | Vitest setup file: loads i18n, stubs `matchMedia`, starts MSW and cleans the DOM and handlers around tests. | [Explanation](./frontend/src/test/setup.ts.md) |

## `frontend/src/test/msw`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `handlers.ts` | test | Default network behaviour for tests: a healthy API, a signed-out session and a fixed CSRF token. | [Explanation](./frontend/src/test/msw/handlers.ts.md) |
| `server.ts` | test | Creates the MSW Node server from the default handlers. | [Explanation](./frontend/src/test/msw/server.ts.md) |

## `src`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `.semantic.md` | documentation | Generated description of the four `src/` projects as they were at the skeleton stage. | [Explanation](./src/.semantic.md.md) |

## `src/TutoringCentre.Api`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AssemblyMarker.cs` | source | Assembly handle for architecture tests. | [Explanation](./src/TutoringCentre.Api/AssemblyMarker.cs.md) |
| `Program.cs` | source | Application entry point and composition root: configures logging, services (including authentication, antiforgery, rate limiting and OpenAPI), runs the `seed` CLI mode or builds the HTTP pipeline, and starts the web host. | [Explanation](./src/TutoringCentre.Api/Program.cs.md) |
| `TutoringCentre.Api.csproj` | configuration | Project file for the web host: the composition root and HTTP edge, including build-time OpenAPI document generation. | [Explanation](./src/TutoringCentre.Api/TutoringCentre.Api.csproj.md) |
| `appsettings.Development.json` | configuration | Development-only overrides of configuration. | [Explanation](./src/TutoringCentre.Api/appsettings.Development.json.md) |
| `appsettings.json` | configuration | Base application configuration: logging levels, allowed hosts and the (empty) connection-string key. | [Explanation](./src/TutoringCentre.Api/appsettings.json.md) |

## `src/TutoringCentre.Api/Auth`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `ActorMiddleware.cs` | source | The single translation point from HTTP identity (session cookie claims) to Application identity (`StaffActor`). | [Explanation](./src/TutoringCentre.Api/Auth/ActorMiddleware.cs.md) |
| `AntiforgeryEndpointFilter.cs` | source | Endpoint filter applied once to the whole `/api` group that requires a valid antiforgery token on every unsafe request, login included. | [Explanation](./src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs.md) |
| `AntiforgerySetup.cs` | source | Configures the ASP.NET Core antiforgery system: header name and the hardened `__Host-tcm.xsrf` cookie. | [Explanation](./src/TutoringCentre.Api/Auth/AntiforgerySetup.cs.md) |
| `AntiforgeryTokenResponse.cs` | source | Response record for `GET /api/auth/antiforgery`: just the request token string. | [Explanation](./src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs.md) |
| `AuthEndpoints.cs` | source | Maps the five session endpoints (antiforgery, login, logout, me, select centre) as thin transport around Application calls. | [Explanation](./src/TutoringCentre.Api/Auth/AuthEndpoints.cs.md) |
| `AuthenticationSetup.cs` | source | Registers cookie authentication, the fail-closed authorization fallback policy, the session-validation cache/options and Data Protection for the Api. | [Explanation](./src/TutoringCentre.Api/Auth/AuthenticationSetup.cs.md) |
| `LoginRateLimiting.cs` | source | Caps login attempts per client IP with a fixed-window limiter and answers rejections with Problem Details `auth.rate_limited` (429). | [Explanation](./src/TutoringCentre.Api/Auth/LoginRateLimiting.cs.md) |
| `LoginRequest.cs` | source | The login request model and its FluentValidation validator, kept with the endpoint because login is not a CQRS command. | [Explanation](./src/TutoringCentre.Api/Auth/LoginRequest.cs.md) |
| `RequestValidation.cs` | source | Converts a FluentValidation result into the same `validation.failed` Error the dispatcher produces, for requests that bypass the dispatcher. | [Explanation](./src/TutoringCentre.Api/Auth/RequestValidation.cs.md) |
| `SelectCentreRequest.cs` | source | The request body of `POST /api/session/centre`: only the centre id the user asks to switch into. | [Explanation](./src/TutoringCentre.Api/Auth/SelectCentreRequest.cs.md) |
| `SessionClaims.cs` | source | Defines the four session claim names and the factory that builds the principal stored in the encrypted cookie. | [Explanation](./src/TutoringCentre.Api/Auth/SessionClaims.cs.md) |
| `SessionRevalidationHandler.cs` | source | Re-checks a validated cookie's claims against the database so revoked access or a changed security stamp ends the session; valid results are cached briefly, invalid ones never. | [Explanation](./src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs.md) |
| `SessionValidationOptions.cs` | source | Typed options for the revalidation cache duration. | [Explanation](./src/TutoringCentre.Api/Auth/SessionValidationOptions.cs.md) |

## `src/TutoringCentre.Api/Cli`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SeedCommand.cs` | source | The `seed` command: creates two demo centres through the dispatcher as the system actor, then seeds the development staff accounts and memberships. | [Explanation](./src/TutoringCentre.Api/Cli/SeedCommand.cs.md) |

## `src/TutoringCentre.Api/Endpoints`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `PlatformEndpoints.cs` | source | Maps `GET /api/system/info`, the only non-authentication product endpoint, as bind -> dispatch -> map with no logic. | [Explanation](./src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs.md) |

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
| `launchSettings.json` | configuration | Local-development launch profiles for `dotnet run` and IDEs; the `https` profile is the one the frontend, the E2E suite and the README use. | [Explanation](./src/TutoringCentre.Api/Properties/launchSettings.json.md) |

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
| `Actor.cs` | source | Defines who is executing a use case: the abstract `Actor` and its kinds `SystemActor`, `AnonymousActor` and `StaffActor`. | [Explanation](./src/TutoringCentre.Application/Common/Security/Actor.cs.md) |
| `CurrentActorContext.cs` | source | Holds the actor for one scope; starts anonymous, may be set exactly once, and may be replaced only through `Reauthenticate` at login. | [Explanation](./src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs.md) |
| `IAuthenticationService.cs` | source | Application's port for verifying staff credentials, plus the `AuthenticatedUser` result record. | [Explanation](./src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs.md) |
| `ICurrentActor.cs` | source | Read-only view of the actor of the current scope. | [Explanation](./src/TutoringCentre.Application/Common/Security/ICurrentActor.cs.md) |

## `src/TutoringCentre.Application/Identity`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `IMembershipReadService.cs` | source | Read-side port for a staff member's own profile, active memberships and session state, with its DTO records. | [Explanation](./src/TutoringCentre.Application/Identity/IMembershipReadService.cs.md) |

## `src/TutoringCentre.Application/Identity/Queries/GetActiveMembership`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `GetActiveMembershipHandler.cs` | source | The tenant gate: the only way a centre id enters a session. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs.md) |
| `GetActiveMembershipQuery.cs` | source | The tenant-gate question: may the signed-in staff member act in this centre?. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs.md) |

## `src/TutoringCentre.Application/Identity/Queries/GetMyMemberships`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `GetMyMembershipsHandler.cs` | source | Builds the signed-in user's `MeDto` from a freshly read profile, dropping the actor's selected centre if it is no longer an active membership. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs.md) |
| `GetMyMembershipsQuery.cs` | source | Asks for the signed-in staff member's own profile and active memberships. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsQuery.cs.md) |
| `MeDto.cs` | source | The response shape of `GET /api/me` and login: profile, active centre/role and active memberships. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs.md) |

## `src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `ValidateStaffSessionHandler.cs` | source | Decides if a session's security stamp and (when a centre is selected) membership are still current. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs.md) |
| `ValidateStaffSessionQuery.cs` | source | Asks whether an existing session is still valid; internal to the authentication pipeline. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionQuery.cs.md) |

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

## `src/TutoringCentre.Domain/Identity`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `Membership.cs` | source | The entity linking one user to one centre with a role and an active/inactive status. | [Explanation](./src/TutoringCentre.Domain/Identity/Membership.cs.md) |
| `MembershipStatus.cs` | source | Enum: `Active` or `Inactive` - whether a membership currently grants access. | [Explanation](./src/TutoringCentre.Domain/Identity/MembershipStatus.cs.md) |
| `StaffRole.cs` | source | Enum of the three staff roles within a centre: Owner, Teacher, Secretary. | [Explanation](./src/TutoringCentre.Domain/Identity/StaffRole.cs.md) |

## `src/TutoringCentre.Infrastructure`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AssemblyMarker.cs` | source | Assembly handle for architecture tests. | [Explanation](./src/TutoringCentre.Infrastructure/AssemblyMarker.cs.md) |
| `DependencyInjection.cs` | source | Infrastructure's registration entry point: wires every Application port to its PostgreSQL/EF/system implementation, configures the database, health check and options, and registers ASP.NET Core Identity (core only) with the password and lockout policy. | [Explanation](./src/TutoringCentre.Infrastructure/DependencyInjection.cs.md) |
| `TutoringCentre.Infrastructure.csproj` | configuration | Project file for the adapter layer: references Application and Domain and brings in EF Core, the Npgsql provider and the health check. | [Explanation](./src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj.md) |

## `src/TutoringCentre.Infrastructure/Identity`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `ApplicationUser.cs` | source | Infrastructure's persistence model for a staff user, extending ASP.NET Core Identity's `IdentityUser<Guid>`. | [Explanation](./src/TutoringCentre.Infrastructure/Identity/ApplicationUser.cs.md) |
| `DevelopmentIdentitySeeder.cs` | source | Development bootstrapping: creates five staff accounts and their memberships idempotently, using Domain factories. | [Explanation](./src/TutoringCentre.Infrastructure/Identity/DevelopmentIdentitySeeder.cs.md) |
| `IdentityAuthenticationService.cs` | source | Implements `IAuthenticationService` with ASP.NET Core Identity's `UserManager`: password check, lockout, no account enumeration. | [Explanation](./src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AppDbContext.cs` | source | The single EF Core database context for the whole application, extended with ASP.NET Core Identity's user, claim, login and token tables (no role tables). | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs.md) |
| `DatabaseOptions.cs` | source | Typed settings object holding the database connection string, validated at startup. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/DatabaseOptions.cs.md) |
| `MigrationRunner.cs` | source | Extension method that applies pending EF Core migrations from an `IServiceProvider`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs.md) |
| `Schemas.cs` | source | Constants naming the PostgreSQL schemas used per feature module. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Schemas.cs.md) |
| `UnitOfWork.cs` | source | Implements the `IUnitOfWork` port with an explicit database transaction on the scoped `AppDbContext`, including PostgreSQL read-only transactions for queries. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `CentreConfiguration.cs` | source | Maps the `Centre` entity to the `platform.centres` table: columns, limits, key, unique index, locale conversion, CHECK constraint and timestamp shadow properties. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `ApplicationUserConfiguration.cs` | source | Maps `ApplicationUser` to `identity.users`, adds the locale CHECK and a unique normalised-email index. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs.md) |
| `IdentityUserClaimConfiguration.cs` | source | Renames Identity's default `claims` table into `identity.user_claims`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserClaimConfiguration.cs.md) |
| `IdentityUserLoginConfiguration.cs` | source | Renames Identity's default `logins` table into `identity.user_logins`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserLoginConfiguration.cs.md) |
| `IdentityUserTokenConfiguration.cs` | source | Renames Identity's default `tokens` table into `identity.user_tokens`. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/IdentityUserTokenConfiguration.cs.md) |
| `MembershipConfiguration.cs` | source | Maps `Membership` to `identity.memberships` with the first foreign keys, composite unique index, converters and CHECK constraints in the schema. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Interceptors`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `TimestampInterceptor.cs` | source | Automatically fills `CreatedAt` and `UpdatedAt` shadow columns just before EF saves. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs.md) |

## `src/TutoringCentre.Infrastructure/Persistence/Migrations`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `20261002222404_InitialPlatform.Designer.cs` | generated file | EF Core generated target-model snapshot for the migration. | Skipped: skipped because it is tool output. The migration itself is explained in `20261002222404_InitialPlatform.cs.md`. |
| `20261002222404_InitialPlatform.cs` | migration | The only migration: creates the `platform` schema, the `centres` table, its primary key, locale CHECK constraint and unique slug index. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.cs.md) |
| `20261004112858_AddIdentityAndMemberships.Designer.cs` | generated file | EF Core generated target-model snapshot for the identity migration. | Skipped: skipped because it is tool output. The migration itself is explained in `20261004112858_AddIdentityAndMemberships.cs.md`. |
| `20261004112858_AddIdentityAndMemberships.cs` | migration | The second migration: creates the `identity` schema with `users`, `memberships`, `user_claims`, `user_logins` and `user_tokens`, their keys, foreign keys, indexes and CHECK constraints. | [Explanation](./src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.cs.md) |
| `AppDbContextModelSnapshot.cs` | generated file | EF Core generated current-model snapshot (`// <auto-generated />`). | Skipped: skipped because it is tool output. Its role (diff base, checked by CI `has-pending-model-changes`) is explained in the migration, `CentreConfiguration` and `ci.yml` explanations. |

## `src/TutoringCentre.Infrastructure/ReadServices`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `MembershipReadService.cs` | source | Read-only EF projections for a staff member's profile, active memberships and session state. | [Explanation](./src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs.md) |
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

## `tests/TutoringCentre.Api.Tests/Auth`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `LoginFlowTests.cs` | test | HTTP-level tests of the happy paths of authentication: login, automatic and manual centre selection, re-login while signed in, logout and the session after logout. | [Explanation](./tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs.md) |
| `SecurityTests.cs` | test | Proves that the attacks the authentication design is meant to close are actually closed: user enumeration, CSRF, cookie tampering, stale and revoked sessions, tenant escalation, password spraying, lockout and forgotten authorization. | [Explanation](./tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Cli`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `SeedCommandTests.cs` | test | Proves the seed command is idempotent and creates exactly the two demo centres and the development staff set. | [Explanation](./tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Fixtures`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `AntiforgeryTestHelper.cs` | test | Test helper that fetches a real antiforgery token and attaches it (plus a per-call rate-limit partition) to every POST, so tests exercise the CSRF protection instead of bypassing it. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs.md) |
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
| `FakeMembershipReadService.cs` | test | In-memory fake of `IMembershipReadService` that returns whatever the test assigned to three properties. | [Explanation](./tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs.md) |
| `FakeUnitOfWork.cs` | test | In-memory `IUnitOfWork` that records every call in order. | [Explanation](./tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs.md) |

## `tests/TutoringCentre.Application.Tests/Identity`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `GetActiveMembershipHandlerTests.cs` | test | Tests the tenant gate query handler: anonymous actor, no active membership, active membership. | [Explanation](./tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs.md) |
| `GetMyMembershipsHandlerTests.cs` | test | Tests the handler behind `GET /api/me`: anonymous actor, unknown profile, and how the active centre/role are reported or dropped. | [Explanation](./tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs.md) |
| `ValidateStaffSessionHandlerTests.cs` | test | Tests the session-revalidation rule: unknown user, wrong security stamp, inactive membership, and the two valid cases. | [Explanation](./tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs.md) |

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
| `ApiRuleTests.cs` | test | Architecture rules for the Api project: endpoints must not touch Infrastructure, repositories or domain entities, and only `Program` and the CLI may reference Infrastructure. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs.md) |
| `ArchitectureSupport.cs` | test | Shared helpers for the architecture tests: recognising repository interfaces/classes by name and describing a failing result. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs.md) |
| `CqrsRuleTests.cs` | test | Reflection-based architecture rules for CQRS: exactly one handler per request, handlers sealed and non-public, query handlers never depend on repositories. | [Explanation](./tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs.md) |
| `DependencyRuleTests.cs` | test | Type-level architecture tests: fail when compiled code in a layer uses a forbidden namespace. | [Explanation](./tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md) |
| `IdentityRuleTests.cs` | test | Architecture rules that isolate ASP.NET Core Identity: Application and the Api (except Program and the CLI) must not depend on Identity types or on `TutoringCentre.Infrastructure.Identity`. | [Explanation](./tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs.md) |
| `ProjectFiles.cs` | test | Helper that reads the `ProjectReference` items of a `src` project from its `.csproj` XML. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs.md) |
| `ProjectReferenceTests.cs` | test | Declared-reference architecture tests: each `src` project's `ProjectReference` list must equal the allowed set. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md) |
| `RepositoryRuleTests.cs` | test | Architecture rules about repository placement: `I*Repository` interfaces live in Application, `*Repository` classes live in Infrastructure. | [Explanation](./tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs.md) |
| `SourceAssemblies.cs` | test | Single place that names the four production assemblies for architecture tests, obtained from marker types. | [Explanation](./tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs.md) |
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

## `tests/TutoringCentre.Domain.Tests/Identity`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `MembershipTests.cs` | test | Unit tests of the `Membership` entity: creation rules and the Activate/Deactivate state transitions. | [Explanation](./tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs.md) |

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

## `tests/TutoringCentre.Infrastructure.Tests/Identity`

| File | Type | Purpose | Explanation |
| --- | --- | --- | --- |
| `IdentityAuthenticationServiceTests.cs` | test | Proves the credential adapter against real PostgreSQL: correct and wrong passwords, unknown email, lockout after five failures, counter reset, and that the email is never logged. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs.md) |
| `MembershipConstraintTests.cs` | test | Proves that PostgreSQL itself enforces the identity schema: exactly five identity tables, unique (centre, user) membership, foreign key to centres, and no deleting a centre that has members. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs.md) |
| `MembershipReadServiceTests.cs` | test | Proves the membership read service against real PostgreSQL: who sees which centres, that inactive members see none, and that nobody sees another user's memberships. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs.md) |

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
