# frontend/src/test/setup.ts

## Purpose

Vitest setup file: loads i18n, stubs `matchMedia`, starts MSW and cleans the DOM and handlers around tests.

## Where It Fits

frontend/src/test. Referenced by `vite.config.ts` `test.setupFiles`.

## Walkthrough

Imports `@testing-library/jest-dom/vitest` (matchers such as `toBeInTheDocument`) and `@/i18n` (so `t()` returns real English text in every test). `window.matchMedia` is stubbed (9-19) because jsdom has no `matchMedia` and Sonner's `Toaster` (rendered on every route by `__root.tsx`) reads it for `prefers-color-scheme`; the stub returns `matches: false`. `beforeAll`: `server.listen({ onUnhandledRequest: "error" })` - any request without a handler fails the test (22-24). `afterEach`: `cleanup()` (unmount; needed because Vitest globals are off) and `server.resetHandlers()` (26-29). `afterAll`: `server.close()` (31-33).

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Strict unhandled-request policy.

#### How it works here

Line 23.

#### Why it matters here

A forgotten handler fails loudly instead of reaching the network.

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Real translations in tests.

#### How it works here

Import of `@/i18n`.

#### Why it matters here

Assertions use real strings, so wording changes in JSON show up in tests.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The jsdom message 'Not implemented: Window's scrollTo() method' is printed repeatedly during the router tests (observed in the test run); it is noise, not a failure.

## Related Files

- [`frontend/src/test/msw/server.ts`](msw/server.ts.md)
- [`frontend/src/test/msw/handlers.ts`](msw/handlers.ts.md)
- [`frontend/vite.config.ts`](../../vite.config.ts.md)
- [`frontend/src/i18n/index.ts`](../i18n/index.ts.md)
