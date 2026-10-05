# frontend/src/api/errors.ts

## Purpose

TypeScript types for the single error shape the UI uses: `ErrorKind` and `ApiError`.

## Where It Fits

frontend/src/api. Produced by `apiFetch.ts` (`toApiError`, `ApiRequestError`) and consumed by `errorMessages.ts`, `showApiError.ts`, `queryClient.ts` and routes. Mirrors `src/TutoringCentre.Domain/Common/ErrorKind.cs`.

## Walkthrough

`ErrorKind = "validation" | "notFound" | "conflict" | "rule" | "forbidden" | "unauthenticated" | "unexpected"` (5-12): the first six are the backend enum in camelCase (the backend gained `Unauthenticated` for 401); `unexpected` is for 500s, network errors and unparseable bodies. `interface ApiError { kind; code; message; status; fieldErrors?; correlationId? }` (15-27): `code` is the stable key the UI translates; `message` is developer-facing and never shown verbatim; `fieldErrors` are validation messages per field name; `correlationId` is for support. The comment says the fetch wrapper is produced 'from Day 12'; it is `apiFetch.ts`.

## Concepts Used

### Strict TypeScript and type-aware linting

#### What it means

TypeScript checks types at build time; `strict` and extra flags such as `noUncheckedIndexedAccess` make unsafe patterns compile errors. Type-aware ESLint rules use the compiler's type information to catch more bugs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

String-literal union types.

#### How it works here

Lines 5-12.

#### Why it matters here

A `switch` over `kind` is checked for exhaustiveness by the compiler.

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Shape of the client-side error.

#### How it works here

Lines 15-27.

#### Why it matters here

Fields mirror Problem Details (`code`, `errors`, `correlationId`) so `toApiError` is a direct mapping.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Hand-maintained mirror of the C# enum (the generated client does not include `ErrorKind`, it only describes endpoints), so a new backend kind needs a manual change here and in `kindFromStatus`.

## Related Files

- [`frontend/src/api/apiFetch.ts`](apiFetch.ts.md)
- [`frontend/src/api/errorMessages.ts`](errorMessages.ts.md)
- [`frontend/src/api/problemDetails.ts`](problemDetails.ts.md)
- [`src/TutoringCentre.Domain/Common/ErrorKind.cs`](../../../src/TutoringCentre.Domain/Common/ErrorKind.cs.md)
