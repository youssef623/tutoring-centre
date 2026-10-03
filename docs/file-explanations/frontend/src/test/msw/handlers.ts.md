# frontend/src/test/msw/handlers.ts

## Purpose

Default network behaviour for tests: a healthy API.

## Where It Fits

frontend/src/test/msw. Imported by `server.ts`.

## Walkthrough

`http.get("/health/ready", () => new HttpResponse("Healthy", {status: 200}))` and `http.get("/api/system/info", () => HttpResponse.json({applicationVersion:"1.0.0", latestMigration:"20261012_InitialPlatform", databaseUpToDate:true}))`. The second handler is currently unused, and its migration id differs from the real `20261002222404_InitialPlatform` (a mock value).

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Request handlers.

#### How it works here

Array.

#### Why it matters here

Defaults that tests override.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Mock data does not match the real migration name.

## Related Files

- [`frontend/src/test/msw/server.ts`](server.ts.md)
- [`frontend/src/features/status/StatusCard.test.tsx`](../../features/status/StatusCard.test.tsx.md)
