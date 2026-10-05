# src/TutoringCentre.Api/Auth/RequestValidation.cs

## Purpose

Converts a FluentValidation result into the same `validation.failed` Error the dispatcher produces, for requests that bypass the dispatcher.

## Where It Fits

Api/Auth, `internal static`. Used by `AuthEndpoints.LoginAsync`.

## Walkthrough

`ToValidationError(ValidationResult)` (12-29): null when valid; otherwise groups failures by camel-cased property name (`ToCamelCase`, 31-33, same algorithm as `Dispatcher`), distinct messages per field, and returns `Error.Validation("validation.failed", "One or more fields are invalid.", fields)`. Result: a login validation failure looks exactly like any other validation failure on the wire.

## Concepts Used

### Two-tier validation

#### What it means

Validation asks whether input is acceptable. Cheap *shape* checks (required, length) can run first without touching business rules or the database; *business invariants* (slug format, time zone must exist) belong to the domain; *database constraints* are a last safety net against rows written by anything else.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#65-validation-two-tiers](../../../../PROJECT_OVERVIEW2.md#65-validation-two-tiers).)

#### Where it appears in this file

Reusing the error shape.

#### How it works here

Lines 21-28.

#### Why it matters here

One validation response format.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The camel-casing and grouping logic is duplicated from `Dispatcher.ValidateAsync` (a small duplication the code comment acknowledges by cross-reference).

## Related Files

- [`src/TutoringCentre.Api/Auth/LoginRequest.cs`](LoginRequest.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
