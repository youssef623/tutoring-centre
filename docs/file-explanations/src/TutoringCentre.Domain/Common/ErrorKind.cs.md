# src/TutoringCentre.Domain/Common/ErrorKind.cs

## Purpose

Closed enum of failure categories; each maps to exactly one HTTP status.

## Where It Fits

Domain/Common. Switched on in `ResultHttpExtensions.ToProblemResult` (default branch throws); mirrored by the frontend `ErrorKind` type (`frontend/src/api/errors.ts`).

## Walkthrough

Members: `Validation, NotFound, Conflict, Rule, Forbidden, Unauthenticated` (the last added for 'we do not know who you are'). Doc comment: adding a kind is an API-contract change. Mapping (in the Api): 400, 404, 409, 422, 403, 401. The frontend adds a seventh, `unexpected`, for failures that have no business meaning, and maps *status codes* back to kinds in `apiFetch.ts` (`kindFromStatus`).

## Concepts Used

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

Closed set of error kinds.

#### How it works here

Enum members.

#### Why it matters here

Every failure in the system is one of a few categories, each with exactly one status.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The frontend list is a hand-maintained copy (the generated client does not include the enum); adding a kind needs changes in `ResultHttpExtensions`, `errors.ts`, `apiFetch.ts` and the translation files.

## Related Files

- [`src/TutoringCentre.Domain/Common/Error.cs`](Error.cs.md)
- [`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`](../../TutoringCentre.Api/Http/ResultHttpExtensions.cs.md)
- [`frontend/src/api/errors.ts`](../../../frontend/src/api/errors.ts.md)
- [`frontend/src/api/apiFetch.ts`](../../../frontend/src/api/apiFetch.ts.md)
