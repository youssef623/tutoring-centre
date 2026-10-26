# Frontend — Tutoring Centre Manager

Scaffolded Day 3 with Vite + React + TypeScript. This file records the decisions that shaped the scaffold, the real structure as it stands at the end of Month 3, and the API contract the status page depends on.

## Stack

- **React + TypeScript**, `strict` mode on.
- **Vite** for the dev server and build.
- **TanStack Router** for routing, **TanStack Query** for server state / data fetching.
- **Tailwind CSS** + **shadcn/ui** for styling and base components.
- **Generated API client** from the backend's OpenAPI spec — no hand-written fetch wrappers for endpoints.
- **i18next** with Arabic and English locales.
- **Logical CSS properties** (`margin-inline-start`, `padding-inline-end`, etc.) instead of physical ones (`margin-left`, `padding-right`), so layout flips correctly for Arabic RTL instead of needing a separate RTL stylesheet.

## Folder structure (end of Month 3)

```
frontend/
  src/
    app/            # app shell, providers
    routes/         # TanStack Router file-based routes (__root.tsx, login.tsx, _authenticated/*)
    api/            # generated API client (src/api/generated/), apiFetch, csrf
    features/       # one folder per feature (feature-based, not type-based):
                     #   auth, session (Can/useCan/useSession — see below), language, shell,
                     #   status (system info, demo banner), dashboard, subjects, staff, settings, audit,
                     #   students, parents, teachers, groups, enrollments, schedule (Month 3)
    components/
      ui/           # shadcn/ui components and thin wrappers (Base UI select, tooltip, radio group, tabs)
      shared/       # app components used by more than one feature (see "Shared components" below)
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

### Shared components (`components/shared`, Month 3)

Used by more than one feature, so they live outside any one feature folder:

- `PageHeader`, `FilterBar`, `StatTile`, `EmptyState`, `StatusBadge` and `DataTable`: the page frame, the filter
  row, the summary tiles, the empty states, the written status badges and the table every list page uses.
- `SearchInput` (debounced; the server does the Arabic normalization, so "احمد" finds "أحمد") and
  `LoadMoreButton` (the "load more" action over a cursor API).
- `PhoneInput` and `PhoneText`: a phone is typed and shown left to right, inside Arabic text too.
- `ParentLookupField` and `useParentLookup`: the parent-phone lookup on student registration and "add parent",
  which offers the existing parent for a known number (the sibling case) instead of creating a duplicate.

`SeatMeter` (`features/enrollments`) is the one seats display ("enrolled / capacity", with "Full" written out),
used by the groups list, the group page and the roster.

### Times: `centreTime` (`features/schedule/centreTime.ts`, Day 53)

The one place a time becomes text. Every function takes the centre's IANA zone (`timeZoneId`, sent by the API with
the timetable and on the session's membership) as an argument and never reads the browser's zone, so a secretary
abroad or a laptop set to another zone still sees Cairo's wall clock: `formatTime`, `formatTimeRange` (instants),
`formatLocalTime`, `formatLocalTimeRange` (a weekly rule's wall-clock time, which has no instant), `todayIn` (the
centre's date, which can differ from UTC's near midnight), and `isLocalTime`, `minutesOf`, `addMinutes`. No date
library: `Intl` applies each instant's own offset, so 16:00Z in winter and 15:00Z in summer both read 18:00 in
Cairo. Times read left to right in both languages (`dir="ltr"` around them). Never format a time with
`toLocaleTimeString()` or `new Date().getHours()`: both use the browser's zone.

### No role checks

The frontend never branches on a role name (`role === "teacher"`): it branches only on permissions, through `Can`
and `useCan` below, and otherwise renders whatever the API returned. A teacher's group list, roster and timetable
are the same components as an owner's, given fewer rows by the server's teacher scope; a page whose data a
role cannot read is wrapped in `RouteGuard`. The one place a role value appears in feature code is as data, not
as a decision: `eligibleStaff.ts` filters the staff list for members whose role is Teacher, because that is the
rule for which accounts may be linked to a teacher record. This mirrors the backend rule that no handler compares
roles (`docs/architecture/authorization.md`).

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
