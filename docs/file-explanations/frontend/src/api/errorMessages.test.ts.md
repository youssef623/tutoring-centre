# frontend/src/api/errorMessages.test.ts

## Purpose
Vitest unit tests for `messageFor`'s three-level fallback: a known code, an unknown code with a known kind, and an unknown code with an unknown kind.

## Where it fits
Co-located test for `errorMessages.ts`.

## Walkthrough
- **Lines 6–22:** `centre.slug_invalid` gives the English slug text.
- **Lines 24–38:** `kind: "conflict"` with an unknown code gives "This conflicts with existing data."
- **Lines 40–54:** `kind: "bogus" as ErrorKind` (a deliberate cast) gives the generic text.

## Concepts used
- **Arrange/Act/Assert in Vitest.**
- **Type assertions** to build impossible values for testing.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Arabic isn't tested.** Only `"en"` is exercised.

## Related files
- [errorMessages.ts](errorMessages.ts.md)
