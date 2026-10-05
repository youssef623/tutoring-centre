# frontend/README.md

## Purpose

Frontend decisions, conventions and API contract, mixed with unedited Vite template notes.

## Where It Fits

Frontend documentation. Linked from the root README.

## Walkthrough

Sections: Stack (React+TS strict, Vite, TanStack Router/Query, Tailwind + shadcn/ui, 'Generated API client' - planned, i18next with ar/en - planned, logical CSS properties); Planned folder structure (`app/ api/ features/ components/ui/ i18n/`); API contract used by the status page (table for `GET /health/ready`: 200 `Healthy`, 503 `Unhealthy`, network error); Conventions (one folder per feature with `api.ts`, `use*.ts`, components, tests; no `fetch` in components; every query renders loading/empty/error/success; errors are modelled - an API answer is data, no trustworthy answer is a thrown error; logical CSS only; MSW for tests with a fresh `QueryClient`; no `any`, no `!`, no `eslint-disable` in feature code; scripts to pass before a PR); Vite template notes; a second API contract table for `GET /api/system/info`. I verified: the status page honours the 200/503 contract and the no-fetch-in-components rule. At the time the README was written several items were planned; after the merge they exist: i18next (`src/i18n/`), the generated client (`src/api/generated/tutoring-centre.ts`, produced by Orval from the API's OpenAPI document), and `app/` (`queryClient.ts`). The README text itself was not changed by the merge, so its 'Generated API client' and 'i18next' bullets still read as plans.

## Concepts Used

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Conventions section.

#### How it works here

Whole document.

#### Why it matters here

Describes how components/hooks/api layers are meant to be arranged.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The README's rule 'only `api.ts` touches the network' is no longer literally true: `src/api/apiFetch.ts` and the generated client are the network layer. The 'logical CSS only' rule is stated for feature code; generated shadcn files in `components/ui/` contained physical utilities such as `text-left`, `pr-18`, `right-2`; `alert.tsx` was converted after the merge, other generated files (for example `components/ui/badge-variants.ts` uses `pr-1.5`/`pl-1.5` in a `has-data-[icon=...]` selector) may still contain some.

## Related Files

- [`frontend/src/features/status/api.ts`](src/features/status/api.ts.md)
- [`frontend/src/features/status/useReadiness.ts`](src/features/status/useReadiness.ts.md)
- [`frontend/package.json`](package.json.md)
- [`docs/adr/0002-dotnet-react.md`](../docs/adr/0002-dotnet-react.md.md)
