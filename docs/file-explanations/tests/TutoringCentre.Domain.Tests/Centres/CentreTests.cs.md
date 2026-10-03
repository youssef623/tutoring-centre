# tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs

## Purpose
Unit tests for `Centre.Create`: a valid creation with name trimming, each invalid input producing the right error code, and boundary cases. This file accounts for 11 of the 19 Domain tests.

## Where it fits
Domain.Tests. It tests `src/TutoringCentre.Domain/Centres/Centre.cs`.

## Walkthrough
- **Lines 8–10:** constants: `ValidName`, `ValidSlug = "nour-academy"`, `Cairo = "Africa/Cairo"`.
- **Tests:**
  - **Lines 12–26:** `"  Nour Academy  "` is trimmed. All properties are set and `Id` isn't empty.
  - **Lines 28–30:** empty name → `centre.name_required`.
  - **Lines 32–36:** 121 characters → `centre.name_too_long`.
  - **Lines 38–44, Theory:** slugs `"ab"` (too short), `"Bad Slug"`, `"-abc"`, `"abc--def"` → `centre.slug_invalid`.
  - **Lines 46–50:** `"Mars/Base"` → `centre.time_zone_invalid`.
  - **Lines 52–59:** Cairo with `En` succeeds.
  - **Lines 61–64:** whitespace-only name → required ("Pins down" that `IsNullOrWhiteSpace` is used).
  - **Lines 66–76:** exactly 120 characters succeeds (inclusive boundary).
- **Lines 78–84, `AssertValidationFailure`:** checks failure, non-null error, the code, and kind `Validation`.

## Concepts used
- **xUnit `[Theory]` + `[InlineData]`.**
- **Boundary-value testing.**
- **Expression-bodied tests.**

## Data and control flow
Not applicable.

## Configuration and environment
Relies on the OS tz database for `Africa/Cairo`.

## Gotchas and issues
- **Not tested:**
  - slug longer than 60;
  - null slug;
  - trailing newline in the slug (the `\z` reason);
  - uppercase slug;
  - Windows tz ids (accepted, see `Centre.cs.md`).

## Related files
- [Centre.cs](../../../src/TutoringCentre.Domain/Centres/Centre.cs.md)
