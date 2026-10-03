# frontend/package.json

## Purpose

Declares the frontend's dependencies and npm scripts.

## Where It Fits

Frontend root. Read by npm; `npm ci` also checks it against `package-lock.json`. Scripts are called by `.github/workflows/ci.yml` (`lint`, `typecheck`, `test:ci`, `build`) and by developers (`dev`).

## Walkthrough

`name: frontend`, `private: true`, `version: 0.0.0`, `type: module` (ES modules; why `eslint.config.js` and `vite.config.ts` use `import`).
Scripts: `dev` = `vite`; `build` = `tsc -b && vite build` (type-check, then bundle); `lint` = `eslint .`; `format` = `prettier --write .`; `typecheck` = `tsc -b --noEmit`; `preview` = `vite preview`; `test` = `vitest` (watch); `test:ci` = `vitest --run` (single run).
Runtime `dependencies`: `react`/`react-dom` ^19.2.8, `@tanstack/react-query`, `@tanstack/react-router`, `tailwindcss` + `@tailwindcss/vite`, `@base-ui/react`, `class-variance-authority`, `cn`, `lucide-react`, `next-themes`, `sonner`, `tw-animate-css`, `@fontsource-variable/geist`, `shadcn`. `devDependencies`: eslint 10 + `typescript-eslint`, `eslint-plugin-react-hooks`/`react-refresh`, `typescript ~6.0.2`, `vite ^8.3.0`, `@vitejs/plugin-react`, `@tanstack/router-plugin`, `vitest ^5.0.3`, `jsdom`, `msw`, Testing Library packages, `prettier`.
Observations: no `i18next` and no OpenAPI client although the README plans them; `shadcn` (a CLI) is listed as a runtime dependency; `^` ranges are made reproducible by the lockfile. Installed versions I observed after `npm ci`: React 19.3.0, Vite 8.3.2, Vitest 5.0.3, TypeScript 6.0.3.

## Concepts Used

### Vite dev server, bundling and proxy

#### What it means

Vite serves source modules during development and bundles for production. Its dev proxy forwards chosen paths (`/api`, `/health`) to another server so the browser sees one origin and no CORS configuration is needed.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Scripts and Vite tooling.

#### How it works here

`scripts` block.

#### Why it matters here

One-word entry points for CI and developers.

### Continuous Integration

#### What it means

CI runs automated build, test and analysis on every change in a clean machine so that regressions are caught before merge. Workflows are YAML files describing jobs (parallel units) made of steps.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#11-configuration--environment--deployment](../../PROJECT_OVERVIEW.md#11-configuration--environment--deployment).)

#### Where it appears in this file

Scripts used by CI.

#### How it works here

`lint`, `typecheck`, `test:ci`, `build`.

#### Why it matters here

CI and local runs are identical.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The `cn` package is used instead of the common `clsx`+`tailwind-merge` pair; the repo does not say why (the generated shadcn files import it directly).

## Related Files

- [`frontend/vite.config.ts`](vite.config.ts.md)
- [`frontend/tsconfig.app.json`](tsconfig.app.json.md)
- [`frontend/eslint.config.js`](eslint.config.js.md)
- [`.github/workflows/ci.yml`](../.github/workflows/ci.yml.md)
