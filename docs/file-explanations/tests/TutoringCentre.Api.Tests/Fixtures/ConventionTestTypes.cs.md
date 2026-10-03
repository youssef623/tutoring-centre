# tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs

## Purpose

Test-only commands, handlers and validator that produce every error kind on demand.

## Where It Fits

Api.Tests/Fixtures. Used by `ConventionEndpoints` and registered by `ConventionsFactory`.

## Walkthrough

Records `ConventionCommand(Kind)`, `TestNameCommand(Name)`, `TestNameBody(Name)`, `SensitiveProbe(Email, Password, PhoneNumber)`. `ConventionCommandHandler`: switch on `Kind`: `ok` -> success; `validation` -> `Error.Validation(code,msg,fields{name:[...]})`; `notfound`/`conflict`/`rule`/`forbidden` -> matching errors; `throws` -> throws `InvalidOperationException("Secret internal detail: Host=db;Password=hunter2")` (a planted secret to prove it never reaches the response); anything else -> `ArgumentOutOfRangeException`. `TestNameHandler` returns `"Hello {Name}"`; `TestNameValidator` requires non-empty name.

## Concepts Used

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

One handler, all error kinds.

#### How it works here

Switch expression.

#### Why it matters here

Parametrises the HTTP mapping tests.

### Two-tier validation

#### What it means

Validation asks whether input is acceptable. Cheap *shape* checks (required, length) can run first without touching business rules or the database; *business invariants* (slug format, time zone must exist) belong to the domain; *database constraints* are a last safety net against rows written by anything else.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#65-validation-two-tiers](../../../../PROJECT_OVERVIEW.md#65-validation-two-tiers).)

#### Where it appears in this file

FluentValidation in the test host.

#### How it works here

`TestNameValidator`.

#### Why it matters here

Shows validation failures reach HTTP 400.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The planted password string is a fake test value.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs`](ConventionEndpoints.cs.md)
- [`tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs`](../Http/ProblemDetailsTests.cs.md)
