# tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs

## Purpose
Tests `Result`/`Result<T>`:
- success exposes the value and has no error;
- failure exposes the error;
- reading `Value` on a failure throws;
- a failure with a null error throws.

## Where it fits
Domain.Tests. It tests `Domain/Common/Result.cs`.

## Walkthrough
- **Line 7:** `SampleError = Error.Validation("test.invalid", ...)`.
- **Lines 9–20:** `Result<int>.Success(42)`: success, no error, `Value == 42`.
- **Lines 22–30:** failure: `IsFailure`, and the same error comes back.
- **Lines 32–38:** `Value` on a failure → `InvalidOperationException`.
- **Lines 40–45:** `Result.Failure(null!)` → `ArgumentNullException`. The `null!` deliberately bypasses nullability to test the guard.

## Concepts used
- **Testing guard clauses.**
- **The null-forgiving operator in tests.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Not tested:** non-generic `Result.Success()`, and `Result<T>.Failure(null!)`.

## Related files
- [Result.cs](../../../src/TutoringCentre.Domain/Common/Result.cs.md)
