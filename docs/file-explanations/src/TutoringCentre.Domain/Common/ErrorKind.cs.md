# src/TutoringCentre.Domain/Common/ErrorKind.cs

## Purpose
The closed set of expected failure kinds: `Validation`, `NotFound`, `Conflict`, `Rule`, `Forbidden`. The doc comment (lines 3–6) says the Api will map each kind to exactly one HTTP status (Day 11), so adding a kind is an API-contract change.

## Where it fits
Domain/Common. Used by `Error`. Mirrored (camelCase, plus `"unexpected"`) by `frontend/src/api/errors.ts:5`.

## Walkthrough
- **Lines 7–14:** the enum members, in the order listed above.

## Concepts used
- **Closed enumeration as a contract.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- The kind-to-HTTP-status mapping does not exist yet. The obvious mapping (400/404/409/422?/403) is not decided in any file, so don't assume it.
- The frontend's names are camelCase (`notFound`), so the future mapper must convert.

## Related files
- [Error.cs](Error.cs.md)
- [errors.ts](../../../frontend/src/api/errors.ts.md)
- [overview.md](../../../docs/architecture/overview.md.md)
