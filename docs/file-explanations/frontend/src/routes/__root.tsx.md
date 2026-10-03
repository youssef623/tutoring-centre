# frontend/src/routes/__root.tsx

## Purpose

Root route: the layout wrapping every page, plus the global toast container.

## Where It Fits

frontend/src/routes. TanStack Router file convention: `__root` is the parent of all routes. Registered in generated `routeTree.gen.ts`.

## Walkthrough

`export const Route = createRootRoute({ component: RootLayout })`. `RootLayout` returns a full-height div with `bg-background text-foreground`, a centered `<main className="mx-auto w-full max-w-3xl p-6">` containing `<Outlet />` (where the matched child route renders), and one `<Toaster richColors />` (comment: one toaster for the whole app; future API-error utilities will show failures there). Nothing triggers a toast yet.

## Concepts Used

### File-based routing with TanStack Router

#### What it means

A router maps URLs to components. In file-based routing a Vite plugin scans `src/routes/` and generates a route tree file, so adding a file adds a route with type-safe links.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Root route and `Outlet`.

#### How it works here

`createRootRoute`, `<Outlet />`.

#### Why it matters here

Shared layout without repeating it per page.

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Layout component.

#### How it works here

`RootLayout`.

#### Why it matters here

Pure function of its (absent) props.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Lint warns that a file exporting `Route` and a component breaks fast refresh (2 warnings overall).

## Related Files

- [`frontend/src/routes/index.tsx`](index.tsx.md)
- `frontend/src/routeTree.gen.ts` (generated / lockfile / media: no separate explanation, see INDEX)
- [`frontend/src/components/ui/sonner.tsx`](../components/ui/sonner.tsx.md)
