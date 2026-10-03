# frontend/src/test/setup.ts

## Purpose

Vitest setup file: starts MSW, cleans the DOM and resets handlers around tests.

## Where It Fits

frontend/src/test. Referenced by `vite.config.ts` `test.setupFiles`.

## Walkthrough

Imports `@testing-library/jest-dom/vitest` (adds matchers like `toBeInTheDocument`). `beforeAll`: `server.listen({ onUnhandledRequest: "error" })` - any request without a handler fails the test instead of reaching the network (line 8). `afterEach`: `cleanup()` (unmount; needed because Vitest globals are off so Testing Library cannot auto-register it) and `server.resetHandlers()` (drop per-test overrides). `afterAll`: `server.close()`.

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

MSW lifecycle.

#### How it works here

Lines 7-18.

#### Why it matters here

Network isolation for every test.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/test/msw/server.ts`](msw/server.ts.md)
- [`frontend/src/test/msw/handlers.ts`](msw/handlers.ts.md)
- [`frontend/vite.config.ts`](../../vite.config.ts.md)
