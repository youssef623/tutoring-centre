# frontend/src/api/errors.ts

## Purpose
Defines the frontend's single error model:
- `ErrorKind` mirrors the backend's `ErrorKind`, plus `"unexpected"` for 500s, network failures and unparseable responses;
- `ApiError` is the one shape every failed API call will be turned into, by the fetch wrapper planned for Day 12.

## Where it fits
Shared `src/api/` layer. Used by `errorMessages.ts` and its test. It mirrors `src/TutoringCentre.Domain/Common/ErrorKind.cs` and `Error.cs`. Nothing produces `ApiError` values yet.

## Walkthrough
- **Line 5:** `type ErrorKind = "validation" | "notFound" | "conflict" | "rule" | "forbidden" | "unexpected"`.
- **Lines 8–20, `interface ApiError`:**
  - `kind`;
  - `code`: a stable code such as `centre.slug_invalid`;
  - `message`: developer-facing, never shown verbatim;
  - `status`: HTTP status;
  - `fieldErrors?`: per field, for validation;
  - `correlationId?`: to quote in bug reports (from Day 11).

## Concepts used
- **String-literal union types:** closed sets checked by the compiler.
- **Contract mirroring** across languages, until the OpenAPI client generates it.

## Data and control flow
(Day 12) HTTP response → fetch wrapper → `ApiError` → `messageFor` → toast or form.

## Configuration and environment
None.

## Gotchas and issues
- **Casing differs from the backend:** camelCase `notFound` here, PascalCase `NotFound` in C#. A mapping step is needed.
- **Field name drift:** `fieldErrors` here vs `errors` in `ProblemDetails`. The future wrapper must map between them.
- **Prettier:** line 5 exceeds `printWidth` 100, so `prettier --check` flags the file.

## Related files
- [errorMessages.ts](errorMessages.ts.md)
- [problemDetails.ts](problemDetails.ts.md)
- [ErrorKind.cs](../../../src/TutoringCentre.Domain/Common/ErrorKind.cs.md)
