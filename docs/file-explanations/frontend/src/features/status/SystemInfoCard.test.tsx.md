# frontend/src/features/status/SystemInfoCard.test.tsx

## Purpose

Tests the system-information card on success and on a 500 Problem Details response.

## Where It Fits

frontend/src/features/status. Uses `renderWithQueryClient` and MSW.

## Walkthrough

Test 1: default handler (`1.0.0`, `20261012_InitialPlatform`, up to date) -> findByText `1.0.0`, the migration id and `Database schema up to date`. Test 2: a 500 response with `code server.unexpected`, `correlationId corr-500` -> shows `Something went wrong. Please try again.` and `Reference: corr-500`. 2 tests.

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Overriding one endpoint.

#### How it works here

Lines 23-36.

#### Why it matters here

`server.use` replaces the default handler for just this test.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/status/SystemInfoCard.tsx`](SystemInfoCard.tsx.md)
- [`frontend/src/test/msw/handlers.ts`](../../test/msw/handlers.ts.md)
- [`frontend/src/test/render.tsx`](../../test/render.tsx.md)
