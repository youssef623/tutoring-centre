# tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs

## Purpose

Unit tests for `CreateCentreValidator`.

## Where It Fits

Application.Tests/Centres.

## Walkthrough

`Validate_EmptyName_ReportsErrorOnName`; `Validate_SlugOverSixtyCharacters_ReportsErrorOnSlug` (61 'a's); `Validate_ValidCommand_HasNoErrors`. Uses the synchronous `Validate` overload and checks `PropertyName`. Not tested: time-zone length, undefined enum value, name over 120.

## Concepts Used

### Two-tier validation

#### What it means

Validation asks whether input is acceptable. Cheap *shape* checks (required, length) can run first without touching business rules or the database; *business invariants* (slug format, time zone must exist) belong to the domain; *database constraints* are a last safety net against rows written by anything else.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#65-validation-two-tiers](../../../../PROJECT_OVERVIEW.md#65-validation-two-tiers).)

#### Where it appears in this file

Shape validator tests.

#### How it works here

All three.

#### Why it matters here

Document which rules exist.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs`](../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs.md)
