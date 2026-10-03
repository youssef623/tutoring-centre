# src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs

## Purpose

Shape validation of `CreateCentreCommand` (required fields, maximum lengths, defined enum).

## Where It Fits

Application/Centres/Commands/CreateCentre, `internal`. Discovered by `AddValidatorsFromAssembly`; run by `Dispatcher.ValidateAsync` before the transaction.

## Walkthrough

`AbstractValidator<CreateCentreCommand>` (FluentValidation). Constructor rules:
- `Name`: `NotEmpty().MaximumLength(Centre.NameMaxLength)` (120).
- `Slug`: `NotEmpty().MaximumLength(Centre.SlugMaxLength)` (60).
- `TimeZoneId`: `NotEmpty().MaximumLength(64)` via private const `TimeZoneIdMaxLength` (duplicated in `CentreConfiguration`).
- `DefaultLocale`: `IsInEnum()` - rejects numeric values outside `Ar`/`En`.
Doc: shape only; business rules (slug format, time-zone existence) live in `Centre.Create`, 'the single source of truth'. (FluentValidation documents `NotEmpty` as also rejecting whitespace-only strings; that is library behaviour and no test here asserts it.)

## Concepts Used

### Two-tier validation

#### What it means

Validation asks whether input is acceptable. Cheap *shape* checks (required, length) can run first without touching business rules or the database; *business invariants* (slug format, time zone must exist) belong to the domain; *database constraints* are a last safety net against rows written by anything else.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#65-validation-two-tiers](../../../../../../PROJECT_OVERVIEW.md#65-validation-two-tiers).)

#### Where it appears in this file

The rule set.

#### How it works here

Lines in constructor.

#### Why it matters here

Cheap rejection before any database work; limits tie to domain constants.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The 64 limit exists twice (validator and EF config) without a shared constant.

## Related Files

- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs`](CreateCentreCommand.cs.md)
- [`src/TutoringCentre.Domain/Centres/Centre.cs`](../../../../TutoringCentre.Domain/Centres/Centre.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../../Common/Cqrs/Dispatcher.cs.md)
- [`tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs`](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs.md)
