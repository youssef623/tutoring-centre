# frontend/src/api/fixtures/problem-404.json

## Purpose

Sample Problem Details body for an unknown API route (`route.not_found`).

## Where It Fits

frontend/src/api/fixtures. Unused by code.

## Walkthrough

`title` 'The requested resource was not found.', `status` 404, `detail` 'The requested route does not exist.', `code` 'route.not_found', placeholder ids - matching `Program.cs` fallback + `ResultHttpExtensions` NotFound title.

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

- [`src/TutoringCentre.Api/Program.cs`](../../../../src/TutoringCentre.Api/Program.cs.md)
- [`frontend/src/api/problemDetails.ts`](../problemDetails.ts.md)
