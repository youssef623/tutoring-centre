# frontend/src/api/errorMessages.test.ts

## Purpose

Unit tests of `messageFor` fallback behaviour.

## Where It Fits

frontend/src/api; run by Vitest.

## Walkthrough

Three tests with Arrange/Act/Assert comments: known code `centre.slug_invalid` in `en` -> the specific sentence; unknown code `centre.something_new` with kind `conflict` -> 'This conflicts with existing data.'; unknown code and unknown kind (`"bogus" as ErrorKind`) -> generic message. Not tested: Arabic output, `unexpected` kind.

## Concepts Used

### Strict TypeScript and type-aware linting

#### What it means

TypeScript checks types at build time; `strict` and extra flags such as `noUncheckedIndexedAccess` make unsafe patterns compile errors. Type-aware ESLint rules use the compiler's type information to catch more bugs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Cast to simulate invalid input.

#### How it works here

`"bogus" as ErrorKind`.

#### Why it matters here

Tests runtime fallback beyond the type system.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/api/errorMessages.ts`](errorMessages.ts.md)
- [`frontend/src/api/errors.ts`](errors.ts.md)
