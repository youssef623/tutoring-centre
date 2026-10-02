# Frontend — Tutoring Centre Manager

Scaffolded Day 3 with Vite + React + TypeScript. This file records the decisions that shaped the scaffold, the planned structure, and the API contract the status page depends on.

## Stack

- **React + TypeScript**, `strict` mode on.
- **Vite** for the dev server and build.
- **TanStack Router** for routing, **TanStack Query** for server state / data fetching.
- **Tailwind CSS** + **shadcn/ui** for styling and base components.
- **Generated API client** from the backend's OpenAPI spec — no hand-written fetch wrappers for endpoints.
- **i18next** with Arabic and English locales.
- **Logical CSS properties** (`margin-inline-start`, `padding-inline-end`, etc.) instead of physical ones (`margin-left`, `padding-right`), so layout flips correctly for Arabic RTL instead of needing a separate RTL stylesheet.

## Planned folder structure

```
frontend/
  app/            # app shell, routing setup, providers
  api/            # generated API client + query hooks
  features/       # one folder per feature (feature-based, not type-based)
  components/ui/  # shadcn/ui components and shared primitives
  i18n/           # i18next config and locale files (ar, en)
```

## API contract used by the status page

| Request                       | Response                               | Meaning                           |
| ----------------------------- | -------------------------------------- | --------------------------------- |
| `GET /health/ready`           | `200` with plain-text body `Healthy`   | API and database reachable        |
| `GET /health/ready`           | `503` with plain-text body `Unhealthy` | API running, database unavailable |
| (no response / network error) | —                                      | API unreachable                   |

## Conventions

- **One folder per feature** under `src/features/<name>/`: `api.ts` (HTTP calls and response mapping), `use<Thing>.ts` (TanStack Query hooks), components (`<Thing>.tsx`), tests next to what they test (`<Thing>.test.tsx`). Example: `features/status/`.
- **No `fetch` in components.** Components call hooks; hooks call `api.ts`; only `api.ts` touches the network. Relative URLs only (`/api/...`, `/health/...`) — never an absolute API URL.
- **Every query renders every state:** loading, empty (when a list can be empty), error, success. Retry is offered where it makes sense.
- **Errors are modelled:** an answer the API gives (e.g. 503 = dependency down) is data; "no trustworthy answer" is a thrown error.
- **Logical CSS only:** `ms-`, `me-`, `ps-`, `pe-`, `start-`, `end-`, `text-start`; never `ml-`, `mr-`, `pl-`, `pr-`, `left-`, `right-`, `text-left`.
- **Tests mock the network with MSW,** not hooks or modules; each test renders with a fresh `QueryClient` (`src/test/render.tsx`).
- **No `any`, no `!` non-null assertions, no `eslint-disable` comments** in feature code.
- **Scripts that must pass before a PR:** `npm run lint`, `npm run typecheck`, `npm test -- --run`, `npm run build`.

## Vite template notes

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

### React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

### Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type-aware lint rules:

```js
export default defineConfig([
  globalIgnores(["dist"]),
  {
    files: ["**/*.{ts,tsx}"],
    extends: [
      // Other configs...

      // Remove tseslint.configs.recommended and replace with this
      tseslint.configs.recommendedTypeChecked,
      // Alternatively, use this for stricter rules
      tseslint.configs.strictTypeChecked,
      // Optionally, add this for stylistic rules
      tseslint.configs.stylisticTypeChecked,

      // Other configs...
    ],
    languageOptions: {
      parserOptions: {
        project: ["./tsconfig.node.json", "./tsconfig.app.json"],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
]);
```

You can also install [eslint-plugin-react-x](https://npmx.dev/package/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://npmx.dev/package/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from "eslint-plugin-react-x";
import reactDom from "eslint-plugin-react-dom";

export default defineConfig([
  globalIgnores(["dist"]),
  {
    files: ["**/*.{ts,tsx}"],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs["recommended-typescript"],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ["./tsconfig.node.json", "./tsconfig.app.json"],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
]);
```

## API contract

`GET /api/system/info` (anonymous) returns:

| Field | Type | Meaning |
| --- | --- | --- |
| `applicationVersion` | string | Informational version without build metadata |
| `latestMigration` | string or null | Latest applied database migration |
| `databaseUpToDate` | boolean | True when no migrations are pending |

Errors are RFC 9457 Problem Details with a stable `code` and `correlationId`. The MSW handler in `src/test/msw/handlers.ts` mirrors this shape; from Day 12 the generated client in `src/api/generated/` is the source of truth.
