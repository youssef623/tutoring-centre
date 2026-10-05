# frontend/src/test/msw/server.ts

## Purpose

Creates the MSW Node server from the default handlers.

## Where It Fits

frontend/src/test/msw. Used by `setup.ts` and tests (`server.use`).

## Walkthrough

`export const server = setupServer(...handlers)`. `msw/node` patches Node's HTTP/fetch so the jsdom test environment's requests are intercepted.

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Server instance.

#### How it works here

One line.

#### Why it matters here

Shared interceptor.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/test/msw/handlers.ts`](handlers.ts.md)
- [`frontend/src/test/setup.ts`](../setup.ts.md)
