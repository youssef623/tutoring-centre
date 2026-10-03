# frontend/README.md

## Purpose

Frontend decisions, conventions and API contract, mixed with unedited Vite template notes.

## Where It Fits

Frontend documentation. Linked from the root README.

## Walkthrough

Sections: Stack (React+TS strict, Vite, TanStack Router/Query, Tailwind + shadcn/ui, 'Generated API client' - planned, i18next with ar/en - planned, logical CSS properties); Planned folder structure (`app/ api/ features/ components/ui/ i18n/`); API contract used by the status page (table for `GET /health/ready`: 200 `Healthy`, 503 `Unhealthy`, network error); Conventions (one folder per feature with `api.ts`, `use*.ts`, components, tests; no `fetch` in components; every query renders loading/empty/error/success; errors are modelled - an API answer is data, no trustworthy answer is a thrown error; logical CSS only; MSW for tests with a fresh `QueryClient`; no `any`, no `!`, no `eslint-disable` in feature code; scripts to pass before a PR); Vite template notes; a second API contract table for `GET /api/system/info`. I verified: the status page honours the 200/503 contract and the no-fetch-in-components rule. **Not yet true:** i18next, generated client, `i18n/` and `app/` as described (`app/` exists with `queryClient.ts` only).

## Concepts Used

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

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

The 'logical CSS only' rule is stated for feature code; generated shadcn files in `components/ui/` contain physical utilities such as `text-left`, `pr-18`, `right-2`.

## Related Files

- [`frontend/src/features/status/api.ts`](src/features/status/api.ts.md)
- [`frontend/src/features/status/useReadiness.ts`](src/features/status/useReadiness.ts.md)
- [`frontend/package.json`](package.json.md)
- [`docs/adr/0002-dotnet-react.md`](../docs/adr/0002-dotnet-react.md.md)
