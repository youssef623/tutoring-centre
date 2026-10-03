# frontend/src/routes/index.tsx

## Purpose

The `/` route: the 'System status' page.

## Where It Fits

frontend/src/routes. Depends on `features/status/StatusCard`.

## Walkthrough

`createFileRoute("/")({ component: IndexPage })`. `IndexPage` returns `<section className="space-y-4"><h1 className="text-2xl font-semibold">System status</h1><StatusCard /></section>`. Because the path string is checked against the generated route tree, a typo is a type error.

## Concepts Used

### File-based routing with TanStack Router

#### What it means

A router maps URLs to components. In file-based routing a Vite plugin scans `src/routes/` and generates a route tree file, so adding a file adds a route with type-safe links.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

`createFileRoute`.

#### How it works here

Line 4-6.

#### Why it matters here

Type-checked path-to-component binding.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/status/StatusCard.tsx`](../features/status/StatusCard.tsx.md)
- [`frontend/src/routes/__root.tsx`](__root.tsx.md)
