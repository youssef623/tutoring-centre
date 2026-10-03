# src/TutoringCentre.Domain/Centres/SupportedLocale.cs

## Purpose
The enum of languages a centre can use by default "for receipts and parent messages" (line 3): `Ar` and `En`.

## Where it fits
Domain layer, `Centres` folder. Used by `Centre.DefaultLocale` and `Centre.Create`, and by `CentreTests`. It matches the frontend's `Lang = "en" | "ar"` (`frontend/src/api/errorMessages.ts:3`).

## Walkthrough
- **Lines 4–8:** `public enum SupportedLocale { Ar, En, }`. With implicit values, `Ar = 0` and `En = 1`.

## Concepts used
- **Enums as a closed value set.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Implicit numeric values: if this is ever persisted as an int, reordering the members would corrupt data. EF mapping (string or int) is not decided yet, because there is no configuration.

## Related files
- [Centre.cs](Centre.cs.md)
- [errorMessages.ts](../../../frontend/src/api/errorMessages.ts.md)
