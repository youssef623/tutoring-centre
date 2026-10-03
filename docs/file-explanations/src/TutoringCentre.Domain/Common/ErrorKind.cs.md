# src/TutoringCentre.Domain/Common/ErrorKind.cs

## Purpose

Closed enum of failure categories; each maps to exactly one HTTP status.

## Where It Fits

Domain/Common. Switched on in `ResultHttpExtensions.ToProblemResult` (default branch throws); mirrored by the frontend `ErrorKind` type.

## Walkthrough

Members: `Validation, NotFound, Conflict, Rule, Forbidden`. Doc comment: adding a kind is an API-contract change. Mapping (in the Api): 400, 404, 409, 422, 403. The frontend adds a sixth, `unexpected`, for failures that have no business meaning.

## Concepts Used

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

The enum.

#### How it works here

Used as `Error.Kind`.

#### Why it matters here

A small closed set keeps HTTP mapping total and testable (`ResultHttpExtensionsTests` theory covers all five).

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The frontend enum is a hand-maintained copy; there is no generation step yet (planned per ADR 0002).

## Related Files

- [`src/TutoringCentre.Domain/Common/Error.cs`](Error.cs.md)
- [`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`](../../TutoringCentre.Api/Http/ResultHttpExtensions.cs.md)
- [`frontend/src/api/errors.ts`](../../../frontend/src/api/errors.ts.md)
