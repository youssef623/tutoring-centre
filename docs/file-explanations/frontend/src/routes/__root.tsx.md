# frontend/src/routes/__root.tsx

## Purpose

Root route: the layout wrapping every page, the typed router context, and the global toast container.

## Where It Fits

frontend/src/routes. TanStack Router file convention: `__root` is the parent of all routes. Registered in generated `routeTree.gen.ts`. Children: `login`, `select-centre`, `status`, `_authenticated` (and its index).

## Walkthrough

`RouterContext { queryClient: QueryClient }` (6-8) - available to every route's `beforeLoad`/`loader` (the guard reads the session through it). `Route = createRootRouteWithContext<RouterContext>()({ component: RootLayout })` (10-12). `RootLayout` (14-22): a full-height div with `bg-background text-foreground`, an `<Outlet />` (where the matched child renders) and one `<Toaster richColors />` (comment: one toaster for the whole app; API-error utilities show failures there). Pages now bring their own `main` element (the earlier shared centred `<main>` wrapper is gone), so the login page can be full-width.

## Concepts Used

### File-based routing with TanStack Router

#### What it means

A router maps URLs to components. In file-based routing a Vite plugin scans `src/routes/` and generates a route tree file, so adding a file adds a route with type-safe links.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Root route with typed context.

#### How it works here

Lines 6-12.

#### Why it matters here

`createRootRouteWithContext` makes `context.queryClient` type-safe in every route.

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

One shared layout.

#### How it works here

`RootLayout`.

#### Why it matters here

The toaster lives once at the top, so any component can call `toast` and it appears.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Lint warns that a file exporting `Route` and a component breaks fast refresh (a project-wide pattern for route files).

## Related Files

- [`frontend/src/routes/_authenticated.tsx`](_authenticated.tsx.md)
- [`frontend/src/routes/login.tsx`](login.tsx.md)
- `frontend/src/routeTree.gen.ts` (generated / lockfile / media: no separate explanation, see INDEX)
- [`frontend/src/components/ui/sonner.tsx`](../components/ui/sonner.tsx.md)
- [`frontend/src/main.tsx`](../main.tsx.md)
