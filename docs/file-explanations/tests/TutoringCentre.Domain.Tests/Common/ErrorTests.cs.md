# tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs

## Purpose
Tests the two `Error.Validation` overloads: with fields, the fields are exposed; without fields, `Fields` is null.

## Where it fits
Domain.Tests. It tests `Domain/Common/Error.cs`.

## Walkthrough
- **Lines 7–20:** a dictionary `{"slug": ["Slug is invalid."]}` → kind Validation, and `Fields["slug"]` equals the array.
- **Lines 22–30:** the no-fields overload → `Fields` is null.

## Concepts used
- **Arrange/Act/Assert.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Not tested:**
  - the null-`fields` guard;
  - the `NotFound`/`Conflict`/`Rule`/`Forbidden` factories;
  - record equality semantics.

## Related files
- [Error.cs](../../../src/TutoringCentre.Domain/Common/Error.cs.md)
