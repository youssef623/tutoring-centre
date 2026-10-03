# frontend/src/api/fixtures/problem-400.json

## Purpose

Sample Problem Details body for a malformed request (`request.malformed`), matching what `GlobalExceptionHandler` returns.

## Where It Fits

frontend/src/api/fixtures. I found no import of this file anywhere in `src` (probably reserved for a future contract test).

## Walkthrough

Fields: `title` 'The request could not be read.', `status` 400, `code` 'request.malformed', `traceId` and `correlationId` placeholders of zeros. No `detail` or `errors`, consistent with `GlobalExceptionHandler.Create`.

## Concepts Used

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#612-http-problem-details-and-error-handling](../../../../../PROJECT_OVERVIEW.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Example payload.

#### How it works here

Whole file.

#### Why it matters here

Shows the wire format.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Unused.

## Related Files

- [`src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs`](../../../../src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs.md)
- [`frontend/src/api/problemDetails.ts`](../problemDetails.ts.md)
