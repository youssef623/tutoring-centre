# frontend/src/api/problemDetails.test.ts

## Purpose

Unit tests for `isProblemDetails`.

## Where It Fits

frontend/src/api; Vitest.

## Walkthrough

Valid `{title, status}` -> true; plain string `"oops"` -> false; `{title}` without status -> false. Not tested: wrong `status` type, `null`.

## Concepts Used

This file introduces no concept that needs a tutorial beyond what its walkthrough already explains.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/api/problemDetails.ts`](problemDetails.ts.md)
