# src/TutoringCentre.Domain/Centres/SupportedLocale.cs

## Purpose

Enum of languages a centre can use by default: `Ar` and `En`.

## Where It Fits

Domain/Centres. Used by `Centre`, `CreateCentreCommand`, `CreateCentreValidator` (`IsInEnum`), `SeedCommand`, and persisted as `'ar'`/`'en'` by a value converter in `CentreConfiguration`.

## Walkthrough

Two members. The database stores lowercase text, not the integer, via `ValueConverter<SupportedLocale,string>`, backed by a CHECK constraint. The API serialises enums as camelCase strings (`JsonStringEnumConverter(JsonNamingPolicy.CamelCase)` in `Program.cs`) so JSON would use `ar`/`en`. Frontend type `Lang` is `"en" | "ar"`.

## Concepts Used

This file introduces no concept that needs a tutorial beyond what its walkthrough already explains.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Adding a locale requires: enum, converter switch (both directions), CHECK constraint (new migration), frontend dictionary.

## Related Files

- [`src/TutoringCentre.Domain/Centres/Centre.cs`](Centre.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs`](../../TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs.md)
- [`frontend/src/api/errorMessages.ts`](../../../frontend/src/api/errorMessages.ts.md)
