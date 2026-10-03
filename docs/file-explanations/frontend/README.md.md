# frontend/README.md

## Purpose
The frontend's decision record and conventions guide:
- the chosen stack;
- a planned folder structure;
- the HTTP contract the status page relies on;
- coding conventions that must hold before a PR.

It also still contains the stock Vite template notes.

## Where it fits
Frontend documentation, linked from the root `README.md:94`. It describes `src/features/status/*`, `src/test/render.tsx` and the `package.json` scripts.

## Walkthrough
- **Lines 5–13, stack:**
  - React + TS strict, Vite, TanStack Router and Query, Tailwind + shadcn/ui;
  - a generated OpenAPI client;
  - i18next (ar/en);
  - logical CSS properties for RTL.
  - **The OpenAPI client and i18next are not installed yet.**
- **Lines 15–24, planned structure:** `app/`, `api/`, `features/`, `components/ui/`, `i18n/`. The real code lives under `src/`; `i18n/` doesn't exist yet.
- **Lines 26–32, status-page contract:**
  - `GET /health/ready` → 200 `Healthy`, or 503 `Unhealthy`.
  - No response means the API is unreachable.
- **Lines 34–43, conventions:**
  1. One folder per feature: `api.ts` (network), `use<Thing>.ts` (Query hooks), components, co-located tests.
  2. No `fetch` in components; relative URLs only.
  3. Every query renders loading, empty, error and success, with retry.
  4. Errors are modelled: an API answer is data; "no trustworthy answer" throws.
  5. Logical CSS only (`ms-`, `me-`, `ps-`, `pe-`, `start-`, `end-`, `text-start`).
  6. MSW for network mocking, and a fresh `QueryClient` per test.
  7. No `any`, no `!`, no `eslint-disable` in feature code.
  8. Must pass: lint, typecheck, `npm test -- --run`, build.
- **Lines 45–117:** the unmodified Vite template text (React Compiler note, how to expand ESLint).

## Concepts used
- **Feature-sliced folders.**
- **RTL via logical properties.**
- **Modelled errors.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Logical-CSS rule broken.** The shadcn components (`alert.tsx`, `button-variants.ts`) use physical classes (`text-left`, `right-2`, `pr-*`, `pl-*`).
- **Stale template text.** Lines 45–117 recommend switching to type-aware lint, which `eslint.config.js` already does with `strictTypeChecked`.
- **Prettier isn't listed as a gate.** It isn't in CI either.

## Related files
- [package.json](package.json.md)
- [features/status/api.ts](src/features/status/api.ts.md)
- [eslint.config.js](eslint.config.js.md)
