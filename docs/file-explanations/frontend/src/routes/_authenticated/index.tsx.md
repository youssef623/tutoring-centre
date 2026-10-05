# frontend/src/routes/_authenticated/index.tsx

## Purpose

The dashboard page at `/` (inside the protected layout): a welcome heading, the active centre name and a placeholder card.

## Where It Fits

frontend/src/routes/_authenticated. Replaces the former `routes/index.tsx` status page at `/`. Uses `useSession` and the shell namespace.

## Walkthrough

`Route = createFileRoute("/_authenticated/")({ component: DashboardPage })`. `DashboardPage` (11-40): `useSession()`; returns `null` if `me` is null; `activeCentre` found from memberships; renders `h1` with `t("dashboard.welcome", { name: me.displayName })` (`dir="auto"` so mixed-direction names display correctly), a paragraph with the centre name, and a dashed `Card` with `dashboard.placeholder`. A comment marks it as a placeholder: 'real dashboard content lands after Day 17'.

## Concepts Used

### File-based routing with TanStack Router

#### What it means

A router maps URLs to components. In file-based routing a Vite plugin scans `src/routes/` and generates a route tree file, so adding a file adds a route with type-safe links.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Index route of a layout.

#### How it works here

`createFileRoute("/_authenticated/")`.

#### Why it matters here

The trailing slash marks the index child of the pathless layout, so the URL is `/`.

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

ICU placeholders.

#### How it works here

Lines 25, 29.

#### Why it matters here

`{name}` and `{centreName}` are filled by i18next's ICU plugin.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Placeholder content only; no real data besides the session profile.

## Related Files

- [`frontend/src/routes/_authenticated.tsx`](../_authenticated.tsx.md)
- [`frontend/src/features/session/useSession.ts`](../../features/session/useSession.ts.md)
- [`frontend/src/i18n/locales/en/shell.json`](../../i18n/locales/en/shell.json.md)
