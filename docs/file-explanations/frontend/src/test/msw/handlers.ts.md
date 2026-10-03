# frontend/src/test/msw/handlers.ts

## Purpose
The default MSW request handlers for tests: a healthy API, where `GET /health/ready` returns 200 with the body `Healthy`. Tests override per case with `server.use(...)` (line 3).

## Where it fits
Test infrastructure. Consumed by `msw/server.ts`.

## Walkthrough
- **Lines 4–6:** `handlers = [http.get("/health/ready", () => new HttpResponse("Healthy", { status: 200 }))]`.

## Concepts used
- **MSW `http` handlers and `HttpResponse`.**
- **Default-plus-override pattern.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Relative URL.** Matching relies on jsdom's base URL; the relative path is resolved against it. It works, since the tests pass.

## Related files
- [server.ts](server.ts.md)
- [StatusCard.test.tsx](../../features/status/StatusCard.test.tsx.md)
