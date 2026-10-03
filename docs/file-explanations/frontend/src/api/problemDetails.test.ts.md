# frontend/src/api/problemDetails.test.ts

## Purpose
Vitest tests for `isProblemDetails`: a valid object gives true, a string gives false, and a missing `status` gives false.

## Where it fits
Co-located test for `problemDetails.ts`.

## Walkthrough
- **Lines 5–14:** `{ title, status }` → true.
- **Lines 16–25:** `"oops"` → false.
- **Lines 27–36:** `{ title }` → false.

## Concepts used
- **Testing type guards** with representative positive and negative inputs.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Not tested:** `null`, a non-string `title`, a string `status`.

## Related files
- [problemDetails.ts](problemDetails.ts.md)
