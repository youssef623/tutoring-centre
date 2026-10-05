# frontend/src/api/apiFetch.test.ts

## Purpose

Tests of `apiFetch` and `toApiError`: success and 204 handling, Problem Details mapping, network failure, non-JSON errors, CSRF header attachment and the single retry.

## Where It Fits

frontend/src/api. Uses MSW (`@/test/msw/server`) so no real network is touched, and the two Problem Details fixtures in `src/api/fixtures/`.

## Walkthrough

Four `describe` blocks, Arrange/Act/Assert comments:
- `apiFetch` (8-98): 200 returns the parsed body; 204 returns `undefined` (DELETE); the 404 fixture becomes an `ApiRequestError` with `kind: "notFound"`, the fixture's `code`, status 404 and `correlationId`; the 400 fixture becomes `kind: "validation"`; `HttpResponse.error()` becomes `network.unreachable` status 0; an HTML 502 body becomes `kind: "unexpected"`, code `http.502`.
- `apiFetch CSRF handling` (100-188): `beforeEach(vi.resetModules)` because the token cache in `csrf.ts` is module-level state, and each test imports a fresh `apiFetch` with `await import("./apiFetch")`. Tests: a POST carries `X-XSRF-TOKEN: test-csrf-token` (default MSW handler); a GET carries no such header; a first 403 `auth.csrf_invalid` makes `apiFetch` fetch a second token and retry, so two POST attempts, two antiforgery calls and the second request carries `token-2`; if the retry also returns 403, the error is thrown after exactly two attempts (no third).
- `toApiError` (190-219): `it.each` maps 400/401/403/404/409/422/500 to `validation/unauthenticated/forbidden/notFound/conflict/rule/unexpected`; a problem without `code` gives `http.409` and uses `title` as the message.
In total 18 tests (counted from the Vitest JSON run).

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Per-test handlers.

#### How it works here

`server.use(http.get(...))` throughout.

#### Why it matters here

Each test defines the exact server behaviour it needs; unhandled requests fail the test (`onUnhandledRequest: "error"` in `setup.ts`).

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../../../../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

Retry-once behaviour.

#### How it works here

Lines 142-187.

#### Why it matters here

The two retry tests pin down the exact policy: one refresh, one retry, then surface the error.

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Fixtures from real responses.

#### How it works here

Lines 3-4, 31-62.

#### Why it matters here

`problem-400.json` and `problem-404.json` are shaped like the backend's real Problem Details (previously unused; now exercised here).

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/api/apiFetch.ts`](apiFetch.ts.md)
- [`frontend/src/api/csrf.ts`](csrf.ts.md)
- [`frontend/src/test/msw/handlers.ts`](../test/msw/handlers.ts.md)
- [`frontend/src/api/fixtures/problem-400.json`](fixtures/problem-400.json.md)
- [`frontend/src/api/fixtures/problem-404.json`](fixtures/problem-404.json.md)
