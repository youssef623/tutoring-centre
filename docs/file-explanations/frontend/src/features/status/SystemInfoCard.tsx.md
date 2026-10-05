# frontend/src/features/status/SystemInfoCard.tsx

## Purpose

Card showing application version, latest migration and whether the schema is up to date, via the generated client's `useGetSystemInfo`.

## Where It Fits

frontend/src/features/status. Rendered by `routes/status.tsx`. Replaces the earlier hand-written call; uses `messageFor`, `asApiError`, shadcn `Alert`, `Badge`, `Button`, `Card`, `Skeleton`, namespaces `status` and `common`.

## Walkthrough

`useGetSystemInfo()` returns `{ data, error, isPending, refetch }`. Branches: pending -> `Skeleton` with `aria-label` `systemInfo.loading`; error -> destructive `Alert` with `systemInfo.unavailableTitle`, `messageFor(apiError)`, the correlation reference (when present) and a Retry button calling `refetch()`; otherwise a `Card` titled `systemInfo.title` with a `Badge` (`schemaUpToDate` or destructive `migrationsPending` from `data.databaseUpToDate`) and a `dl` listing `applicationVersion` and `latestMigration` (`systemInfo.none` when null); values use `dir="ltr"` and `text-start` so technical strings do not flip in RTL.

## Concepts Used

### Server state with TanStack Query

#### What it means

Server state (data owned by the API) needs caching, refetching, retry and loading/error flags. `useQuery` subscribes a component to a cache entry addressed by a *query key*, runs the `queryFn`, and re-renders the component when the entry changes.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Generated hook.

#### How it works here

Line 15.

#### Why it matters here

The hook, its types and URL come from the OpenAPI contract, not hand-written code.

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Translated error and labels.

#### How it works here

Lines 13-14, 26-30, 51-67.

#### Why it matters here

Error text comes from `messageFor`, labels from `status.json`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/status/SystemInfoCard.test.tsx`](SystemInfoCard.test.tsx.md)
- [`frontend/src/routes/status.tsx`](../../routes/status.tsx.md)
- [`frontend/src/api/errorMessages.ts`](../../api/errorMessages.ts.md)
- `frontend/src/api/generated/tutoring-centre.ts` (generated / lockfile / media: no separate explanation, see INDEX)
