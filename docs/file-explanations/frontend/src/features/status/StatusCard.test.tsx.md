# frontend/src/features/status/StatusCard.test.tsx

## Purpose

Component tests for `StatusCard` against a mocked network.

## Where It Fits

frontend/src/features/status; Vitest + Testing Library + MSW. Uses `server` from `test/msw/server.ts` and `renderWithQueryClient`.

## Walkthrough

Constants: `readinessUrl`, `healthy()` (200 'Healthy'), `unavailable()` (503 'Unhealthy'). Tests: (1) healthy - Arrange `server.use(http.get(url, healthy))`; Act render; Assert `findByText('API and database are reachable')`. (2) 503 - asserts 'API is running but the database is unavailable', a Retry button, and that 'Cannot reach the API' is absent (503 is an answer, not a network failure). (3) Retry - start with 503, find Retry, swap handler to healthy with `server.use`, `fireEvent.click`, assert healthy text appears. Not tested: the network-error branch, polling, loading skeleton.

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Per-test handlers.

#### How it works here

`server.use`.

#### Why it matters here

The component and hook run unmodified.

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

User-event simulation.

#### How it works here

`fireEvent.click`.

#### Why it matters here

Drives the refetch path.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/status/StatusCard.tsx`](StatusCard.tsx.md)
- [`frontend/src/test/render.tsx`](../../test/render.tsx.md)
- [`frontend/src/test/msw/server.ts`](../../test/msw/server.ts.md)
- [`frontend/src/test/setup.ts`](../../test/setup.ts.md)
