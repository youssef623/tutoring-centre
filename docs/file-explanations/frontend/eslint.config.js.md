# frontend/eslint.config.js

## Purpose

ESLint flat configuration: strict, type-aware TypeScript rules plus React hooks and fast-refresh rules.

## Where It Fits

Frontend config used by `npm run lint` (CI step 'Lint').

## Walkthrough

`defineConfig([...])`: (1) `globalIgnores(["dist","coverage","src/routeTree.gen.ts"])`. (2) For `**/*.{ts,tsx}`: `extends` `js.configs.recommended`, `tseslint.configs.strictTypeChecked`, `reactHooks.configs.flat.recommended`, `reactRefresh.configs.vite`; `languageOptions.globals = globals.browser`; `parserOptions.projectService: true` (type-aware rules find the right tsconfig per file) with `tsconfigRootDir: import.meta.dirname`. (3) For `src/routes/**`: `react-refresh/only-export-components` as `warn` with `allowExportNames: ["Route"]` (TanStack Router files export `Route` beside a component). When I ran lint it reported 2 warnings (not errors) from this rule on `routes/__root.tsx` and `routes/index.tsx`, so the override does not fully silence the pattern.

## Concepts Used

### Strict TypeScript and type-aware linting

#### What it means

TypeScript checks types at build time; `strict` and extra flags such as `noUncheckedIndexedAccess` make unsafe patterns compile errors. Type-aware ESLint rules use the compiler's type information to catch more bugs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Type-aware strict linting.

#### How it works here

`strictTypeChecked`, `projectService`.

#### Why it matters here

Catches unsafe `any`, floating promises, misused promises.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Two lint warnings currently exist (observed by running lint).

## Related Files

- [`frontend/tsconfig.app.json`](tsconfig.app.json.md)
- [`frontend/package.json`](package.json.md)
- [`.github/workflows/ci.yml`](../.github/workflows/ci.yml.md)
