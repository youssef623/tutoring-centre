# frontend/src/api/problemDetails.ts

## Purpose

Type for RFC 9457 Problem Details as the API returns them and a type guard to recognise them.

## Where It Fits

frontend/src/api. Tested by `problemDetails.test.ts`; unused by screens yet. Mirrors `Api/Http/ProblemResult` output.

## Walkthrough

`interface ProblemDetails { type?, title, status, detail?, instance?, code?, traceId?, correlationId?, errors? }`. `isProblemDetails(value: unknown): value is ProblemDetails` checks `typeof value === "object"`, non-null, `"title" in value` with string type, `"status" in value` with number type - a TypeScript *type predicate*: after it returns true the compiler treats the value as `ProblemDetails`. It validates only the minimum shape; extras are not checked.

## Concepts Used

### Strict TypeScript and type-aware linting

#### What it means

TypeScript checks types at build time; `strict` and extra flags such as `noUncheckedIndexedAccess` make unsafe patterns compile errors. Type-aware ESLint rules use the compiler's type information to catch more bugs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Type guards.

#### How it works here

`value is ProblemDetails`.

#### Why it matters here

Safely narrows untrusted JSON from the network.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Does not verify `code` or `errors` types.

## Related Files

- [`frontend/src/api/problemDetails.test.ts`](problemDetails.test.ts.md)
- [`src/TutoringCentre.Api/Http/ProblemResult.cs`](../../../src/TutoringCentre.Api/Http/ProblemResult.cs.md)
- [`frontend/src/api/fixtures/problem-400.json`](fixtures/problem-400.json.md)
