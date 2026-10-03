# frontend/src/api/problemDetails.ts

## Purpose
The TypeScript shape of the **RFC 9457 Problem Details** responses the API will return (Day 11), including project extensions (`code`, `traceId`, `correlationId`, `errors`). Also a runtime type guard, `isProblemDetails`.

## Where it fits
Shared `src/api/` layer. Tested by `problemDetails.test.ts`. It will be used by the Day-12 fetch wrapper to parse error bodies into `ApiError`. Nothing calls it yet.

## Walkthrough
- **Lines 2–14, `interface ProblemDetails`:**
  - standard fields: `type?`, `title`, `status`, `detail?`, `instance?`;
  - extensions: `code?`, `traceId?`, `correlationId?`, and `errors?` (a camelCase field → messages map; it matches `Error.Fields`).
- **Lines 17–26, `isProblemDetails(value: unknown): value is ProblemDetails`:** true when the value is a non-null object with a string `title` and a numeric `status`. It uses `in` narrowing.

## Concepts used
- **User-defined type guards** (`value is T`).
- **`unknown` + narrowing:** a safe way to handle untrusted JSON.
- **RFC 9457.**

## Data and control flow
Response JSON (`unknown`) → `isProblemDetails` → typed `ProblemDetails`.

## Configuration and environment
None.

## Gotchas and issues
- **Shallow guard.** It checks only `title` and `status`, so optional fields may still have the wrong types at runtime.
- **`title` may not always be present.** RFC 9457 makes `title` optional, but this guard requires it. That's fine if the backend always sends it, which is unverifiable until Day 11.

## Related files
- [problemDetails.test.ts](problemDetails.test.ts.md)
- [errors.ts](errors.ts.md)
