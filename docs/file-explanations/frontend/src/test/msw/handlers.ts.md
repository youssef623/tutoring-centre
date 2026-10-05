# frontend/src/test/msw/handlers.ts

## Purpose

Default network behaviour for tests: a healthy API, a signed-out session and a fixed CSRF token.

## Where It Fits

frontend/src/test/msw. Imported by `server.ts`.

## Walkthrough

`handlers` (4-23): `GET /health/ready` -> 200 `Healthy`; `GET /api/system/info` -> `{ applicationVersion: "1.0.0", latestMigration: "20261012_InitialPlatform", databaseUpToDate: true }` (now used by `SystemInfoCard.test.tsx`); `GET /api/me` -> 401 `{ title: "Unauthorized", status: 401 }` (comment: no session by default; tests that need a signed-in user override it); `GET /api/auth/antiforgery` -> `{ token: "test-csrf-token" }` (comment: every non-GET request fetches this first - `apiFetch`'s CSRF handling - and a fixed token keeps that invisible to tests that are not about CSRF).

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Default handlers.

#### How it works here

Lines 4-23.

#### Why it matters here

Every test starts from 'API healthy, nobody signed in'; per-test `server.use` overrides change one aspect.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The mock migration name `20261012_InitialPlatform` does not match the real latest migration (`20261004112858_AddIdentityAndMemberships`); the tests only check that the string is displayed.

## Related Files

- [`frontend/src/test/msw/server.ts`](server.ts.md)
- [`frontend/src/features/status/SystemInfoCard.test.tsx`](../../features/status/SystemInfoCard.test.tsx.md)
- [`frontend/src/api/apiFetch.test.ts`](../../api/apiFetch.test.ts.md)
- [`frontend/src/features/session/useSession.test.ts`](../../features/session/useSession.test.ts.md)
