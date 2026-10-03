# frontend/src/api/errors.ts

## Purpose

TypeScript types for the single error shape the UI will use: `ErrorKind` and `ApiError`.

## Where It Fits

frontend/src/api. Imported by `errorMessages.ts` and its test. **No screen produces or consumes `ApiError` yet.** Mirrors `src/TutoringCentre.Domain/Common/ErrorKind.cs`.

## Walkthrough

`ErrorKind = "validation" | "notFound" | "conflict" | "rule" | "forbidden" | "unexpected"` (the first five are the backend enum in camelCase; `unexpected` is for 500s/network errors/unparseable bodies). `interface ApiError { kind; code; message; status; fieldErrors?; correlationId? }` - `code` is the stable key the UI translates; `message` is developer-facing, never shown verbatim; `correlationId` is for support. A comment says the fetch wrapper that produces it arrives 'from Day 12' (not present).

## Concepts Used

### Strict TypeScript and type-aware linting

#### What it means

TypeScript checks types at build time; `strict` and extra flags such as `noUncheckedIndexedAccess` make unsafe patterns compile errors. Type-aware ESLint rules use the compiler's type information to catch more bugs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Union type.

#### How it works here

`ErrorKind`.

#### Why it matters here

Exhaustive handling is checked by the compiler.

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Frontend side of the contract.

#### How it works here

`ApiError`.

#### Why it matters here

Backend `code`/`kind`/`correlationId` map onto fields.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Hand-maintained mirror of the C# enum; drift is possible until the planned generated client exists.

## Related Files

- [`frontend/src/api/errorMessages.ts`](errorMessages.ts.md)
- [`frontend/src/api/problemDetails.ts`](problemDetails.ts.md)
- [`src/TutoringCentre.Domain/Common/ErrorKind.cs`](../../../src/TutoringCentre.Domain/Common/ErrorKind.cs.md)
