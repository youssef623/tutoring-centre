# Frontend — Tutoring Centre Manager

Scaffolded Day 3 with Vite + React + TypeScript. This file records the decisions that shaped the scaffold, the real structure as it stands at the end of Month 2, and the API contract the status page depends on.

## Stack

- **React + TypeScript**, `strict` mode on.
- **Vite** for the dev server and build.
- **TanStack Router** for routing, **TanStack Query** for server state / data fetching.
- **Tailwind CSS** + **shadcn/ui** for styling and base components.
- **Generated API client** from the backend's OpenAPI spec — no hand-written fetch wrappers for endpoints.
- **i18next** with Arabic and English locales.
- **Logical CSS properties** (`margin-inline-start`, `padding-inline-end`, etc.) instead of physical ones (`margin-left`, `padding-right`), so layout flips correctly for Arabic RTL instead of needing a separate RTL stylesheet.

## Folder structure (end of Month 2)

```
frontend/
  src/
    app/            # app shell, providers
    routes/         # TanStack Router file-based routes (__root.tsx, login.tsx, _authenticated/*)
    api/            # generated API client (src/api/generated/), apiFetch, csrf
    features/       # one folder per feature (feature-based, not type-based):
                     #   auth, session (Can/useCan/useSession — see below), language, shell,
                     #   status (system info, demo banner), subjects, staff, settings, audit
    components/
      ui/           # shadcn/ui components and shared primitives
      brand/        # logo/wordmark
    i18n/           # i18next config and locale files (ar, en), one namespace per feature
    lib/            # small framework-free helpers (e.g. initials)
    test/           # MSW handlers, render helpers
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

### The permission hook (`features/session`)

`useCan(permission: Permission): boolean` reads the signed-in session's active-role permission list
(`me.permissions`, from `GET /api/me`) and answers whether it holds the given one. `<Can permission={...}>`
wraps it for conditionally rendering children. **This is advice for the UI only** — it decides what to show,
never what to allow. The actual decision is the dispatcher's permission step
(`docs/architecture/authorization.md`), enforced server-side on every request regardless of what the client
rendered; a `useCan` check that is wrong, stale, or bypassed by editing the bundle changes nothing about what
the API will accept. Permission names (`Permission`, `features/session/permissions.ts`) are a frontend-side
mirror of the backend's own code-defined permission set (ADR 0007) — kept in sync by hand, not generated,
since they are a small, stable, enum-like list.

### CSP constraints on new dependencies

The backend's Content-Security-Policy (`docs/architecture/deployment.md`) is `script-src 'self'` with no
relaxation, ever. Before adding a frontend dependency:

- **No CDN `<script src>` tags, ever** — everything ships bundled through Vite, served from this origin.
- **No inline event handlers** (`onclick="..."` in HTML) and no `new Function`/`eval`-based libraries —
  `script-src` has no `'unsafe-eval'` and never will.
- **Inline styles need scrutiny.** `style-src` carries `'unsafe-inline'` today, and only because Sonner's
  `Toaster` sets runtime-computed inline styles with no alternative (Task 34.8, documented in
  `later.md`). A new dependency that also needs inline styles does not get a free pass — it reuses the
  already-open door, but a library that can be configured to use CSS classes instead should be.
- **Fonts and images:** `font-src 'self'`, `img-src 'self' data:` — self-host fonts (`vite.config.ts`'s
  `assetsInlineLimit: 0` keeps even small ones as real same-origin files, Task 34.1), no third-party font or
  image CDNs.
- If a dependency cannot work within this policy, that is a reason to pick a different dependency, not to
  relax the policy — raise it instead of quietly adding `'unsafe-inline'`/`'unsafe-eval'` to make something work.

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
| `applicationVersion` | string | Informational version; carries the deployed commit SHA as a `+<sha>` suffix once built with `-p:SourceRevisionId` (Task 34.2) |
| `latestMigration` | string or null | Latest applied database migration |
| `databaseUpToDate` | boolean | True when no migrations are pending |
| `demoMode` | boolean | Drives the non-dismissible demo banner (Task 35.7) shown on every route when true |

Errors are RFC 9457 Problem Details with a stable `code` and `correlationId`. The MSW handler in `src/test/msw/handlers.ts` mirrors this shape; from Day 12 the generated client in `src/api/generated/` is the source of truth.
