# frontend/package.json

## Purpose

Declares the frontend's dependencies and npm scripts.

## Where It Fits

Frontend root. Read by npm; `npm ci` also checks it against `package-lock.json`. Scripts are called by `.github/workflows/ci.yml` (`generate:api`, `lint`, `typecheck`, `i18n:check`, `test:ci`, `build`, `test:e2e`) and by developers (`dev`).

## Walkthrough

`name: frontend`, `private: true`, `version: 0.0.0`, `type: module`.
Scripts: `dev` = `vite`; `build` = `tsc -b && vite build`; `lint` = `eslint .`; `format` = `prettier --write .`; `typecheck` = `tsc -b --noEmit`; `preview` = `vite preview`; `test` = `vitest` (watch); `test:ci` = `vitest --run`; new: `generate:api` = `orval --config ./orval.config.ts`, `i18n:check` = `node scripts/check-i18n-keys.mjs`, `test:e2e` = `playwright test`.
Runtime `dependencies`: `react`/`react-dom` ^19.2.8, `@tanstack/react-query` ^5.104.1, `@tanstack/react-router` ^1.170.41, `tailwindcss` + `@tailwindcss/vite`, `@base-ui/react` ^1.8.0, `class-variance-authority`, `cn`, `lucide-react`, `next-themes`, `sonner`, `tw-animate-css`, `@fontsource-variable/geist`, `shadcn`; new since the merge: `i18next` ^26.4.2, `react-i18next` ^17.0.15, `i18next-icu` ^2.4.4, `intl-messageformat` ^11.2.15 (ICU message format), `react-hook-form` ^7.89.0, `@hookform/resolvers` ^5.9.1, `zod` ^4.6.5, `@fontsource/ibm-plex-sans-arabic` ^5.3.0. `devDependencies`: eslint 10 + `typescript-eslint`, `eslint-plugin-react-hooks`/`react-refresh`, `typescript ~6.0.2`, `vite`, `@vitejs/plugin-react`, `@tanstack/router-plugin`, `vitest`, `jsdom`, `msw` ^2.15.0, Testing Library packages, `prettier`; new: `orval` ^8.39.0 (client generator) and `@playwright/test` ^1.63.0.
Observations: `i18next` and the OpenAPI client that the frontend README listed as planned now exist; `shadcn` (a CLI) is listed as a runtime dependency; `^` ranges are made reproducible by the lockfile. After `npm ci` on the merged lockfile, `typecheck` passed, `lint` reported 0 errors and 6 warnings, `test:ci` passed (11 files, 51 tests), `build` succeeded and `i18n:check` passed.

## Concepts Used

### Vite dev server, bundling and proxy

#### What it means

Vite serves source modules during development and bundles for production. Its dev proxy forwards chosen paths (`/api`, `/health`) to another server so the browser sees one origin and no CORS configuration is needed.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Scripts and tools.

#### How it works here

`scripts` block.

#### Why it matters here

Each CI step maps to one npm script.

### Contract-first generated API client

#### What it means

The API publishes a machine-readable contract (OpenAPI). A generator turns it into typed client code, so a changed endpoint shape becomes a compile error in the frontend instead of a runtime bug; CI fails if the committed contract or generated code is stale.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#631-contract-first-api-client-generation](../../PROJECT_OVERVIEW2.md#631-contract-first-api-client-generation).)

#### Where it appears in this file

Generator script.

#### How it works here

`generate:api`.

#### Why it matters here

Regenerates the client from `openapi/TutoringCentre.Api.json`.

### End-to-end testing with Playwright

#### What it means

An end-to-end test drives a real browser against a real running system (here the real API, real PostgreSQL and the Vite dev server) and clicks through a user journey. It is slow but catches wiring problems no unit test can.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#635-end-to-end-testing-with-playwright](../../PROJECT_OVERVIEW2.md#635-end-to-end-testing-with-playwright).)

#### Where it appears in this file

E2E script and dependency.

#### How it works here

`test:e2e`, `@playwright/test`.

#### Why it matters here

Runs the browser tests under `e2e/`.

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Translation libraries.

#### How it works here

`i18next`, `react-i18next`, `i18next-icu`, `intl-messageformat`.

#### Why it matters here

ICU message format is used for placeholders like `{name}`.

### Forms and schema validation (React Hook Form + Zod)

#### What it means

React Hook Form tracks form fields with little re-rendering; Zod describes the valid shape of the data in one schema; a resolver connects them so the form shows field errors before any request is sent. Server-side field errors can be mapped back onto the same fields.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#633-forms-and-validation-with-react-hook-form-and-zod](../../PROJECT_OVERVIEW2.md#633-forms-and-validation-with-react-hook-form-and-zod).)

#### Where it appears in this file

Form libraries.

#### How it works here

`react-hook-form`, `@hookform/resolvers`, `zod`.

#### Why it matters here

Used by the login form.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The `cn` package is used instead of the common `clsx`+`tailwind-merge` pair; the repo does not say why (generated shadcn files import it directly). `shadcn` is a CLI but sits in `dependencies`.

## Related Files

- [`frontend/vite.config.ts`](vite.config.ts.md)
- [`frontend/tsconfig.app.json`](tsconfig.app.json.md)
- [`frontend/eslint.config.js`](eslint.config.js.md)
- [`frontend/orval.config.ts`](orval.config.ts.md)
- [`frontend/playwright.config.ts`](playwright.config.ts.md)
- [`frontend/scripts/check-i18n-keys.mjs`](scripts/check-i18n-keys.mjs.md)
- [`.github/workflows/ci.yml`](../.github/workflows/ci.yml.md)
