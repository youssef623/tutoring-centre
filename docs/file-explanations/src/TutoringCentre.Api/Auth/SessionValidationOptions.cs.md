# src/TutoringCentre.Api/Auth/SessionValidationOptions.cs

## Purpose

Typed options for the revalidation cache duration.

## Where It Fits

Api/Auth. Bound from configuration section `SessionValidation` in `AuthenticationSetup` and read by `SessionRevalidationHandler`.

## Walkthrough

`DefaultCacheDuration = TimeSpan.FromSeconds(60)` (6) and `CacheDuration { get; set; } = DefaultCacheDuration` (8). Doc: key `SessionValidation:CacheDuration` (e.g. `"00:01:00"`); zero disables caching. `ApiFactory` sets it to `00:00:00` so tests see revocation on the very next request.

## Concepts Used

### Options pattern and ValidateOnStart

#### What it means

The options pattern binds configuration into a typed class and can validate it. `ValidateOnStart` runs that validation when the host starts, so a bad configuration stops the app immediately instead of failing on first use.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#614-options-validation-and-health-checks](../../../../PROJECT_OVERVIEW2.md#614-options-validation-and-health-checks).)

#### Where it appears in this file

Options class.

#### How it works here

Whole file.

#### Why it matters here

Tunable security/performance trade-off.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

`SessionValidation:CacheDuration` (TimeSpan string).

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs`](SessionRevalidationHandler.cs.md)
- [`src/TutoringCentre.Api/Auth/AuthenticationSetup.cs`](AuthenticationSetup.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs`](../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs.md)
