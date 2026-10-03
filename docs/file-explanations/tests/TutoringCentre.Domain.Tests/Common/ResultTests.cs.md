# tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs

## Purpose

Tests `Result`/`Result<T>` invariants.

## Where It Fits

Domain.Tests; tests `Domain/Common/Result.cs`.

## Walkthrough

`Success_HasNoErrorAndExposesValue`; `Failure_ExposesItsError`; `Value_OnFailure_ThrowsInvalidOperationException` (reading value of a failure is a bug); `Failure_WithNullError_ThrowsArgumentNullException` (`Result.Failure(null!)` - `null!` bypasses nullable annotations deliberately). Not tested: the 'success with error' constructor guard (unreachable through factories).

## Concepts Used

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

Result invariants.

#### How it works here

All four tests.

#### Why it matters here

Guards the foundation of the failure model.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Domain/Common/Result.cs`](../../../src/TutoringCentre.Domain/Common/Result.cs.md)
