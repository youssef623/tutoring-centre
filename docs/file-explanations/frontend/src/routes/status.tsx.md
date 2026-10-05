# frontend/src/routes/status.tsx

## Purpose

The `/status` page: the former status screen (API readiness card and system-information card) moved to its own public route.

## Where It Fits

frontend/src/routes. Public (no guard). Uses `StatusCard`, `SystemInfoCard` and `LanguageSwitcher`.

## Walkthrough

`Route = createFileRoute("/status")({ component: StatusPage })`. `StatusPage` (10-23): a centred `main` (max width 3xl) with a right-aligned `LanguageSwitcher`, an `h1` `System status` (a hard-coded English string, not translated) and the two cards.

## Concepts Used

### File-based routing with TanStack Router

#### What it means

A router maps URLs to components. In file-based routing a Vite plugin scans `src/routes/` and generates a route tree file, so adding a file adds a route with type-safe links.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

A public route beside the protected tree.

#### How it works here

`/status` has no guard.

#### Why it matters here

Operators can check health without signing in.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The heading `System status` is not in the locale files. The page is public and calls `/health/ready` and `/api/system/info`; both work without a session because the API maps them with `.AllowAnonymous()` (`Program.cs` for the health endpoints, `Endpoints/PlatformEndpoints.cs:19` for system info).

## Related Files

- [`frontend/src/features/status/StatusCard.tsx`](../features/status/StatusCard.tsx.md)
- [`frontend/src/features/status/SystemInfoCard.tsx`](../features/status/SystemInfoCard.tsx.md)
- [`frontend/src/features/language/LanguageSwitcher.tsx`](../features/language/LanguageSwitcher.tsx.md)
