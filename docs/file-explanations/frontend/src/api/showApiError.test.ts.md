# frontend/src/api/showApiError.test.ts

## Purpose

Tests of the toast helper and of `applyFieldErrors`.

## Where It Fits

frontend/src/api. Mocks `sonner` with `vi.mock` and a hoisted spy.

## Walkthrough

`toastError = vi.hoisted(() => vi.fn())` and `vi.mock("sonner", () => ({ toast: { error: toastError } }))` (5-6). `showApiError` tests: a conflict with an unknown code and a `correlationId` calls the toast with `This conflicts with existing data.` and `{ description: "Reference: corr-1" }`; an unexpected error without id calls it with `Something went wrong. Please try again.` and `{}`. `applyFieldErrors` tests: only the first message of *known* fields is applied (`name`), `other` is ignored; no `fieldErrors` means `setError` is never called. 4 tests.

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Mocking the toast library.

#### How it works here

Lines 5-6.

#### Why it matters here

`vi.hoisted` makes the spy exist before the hoisted `vi.mock` factory runs.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/api/showApiError.ts`](showApiError.ts.md)
- [`frontend/src/api/errorMessages.ts`](errorMessages.ts.md)
