# src/TutoringCentre.Domain/Common/Result.cs

## Purpose
The **Result pattern**. `Result` represents success or failure-with-`Error` for operations with no value. `Result<T>` adds a value on success. Expected business failures travel as these values instead of exceptions (`docs/architecture/overview.md:5`).

## Where it fits
Domain/Common, used everywhere:
- `Centre.Create` returns `Result<Centre>`;
- `ICommandHandler`/`IQueryHandler` return `Task<Result<T>>`;
- `Dispatcher` inspects `IsFailure` and logs `Error.Code`.

Tested by `ResultTests.cs`.

## Walkthrough
- **Lines 6–36, `class Result`:**
  - **Lines 8–23, protected constructor:** enforces the only two legal states. Success with an error throws `ArgumentException`; failure without an error throws `ArgumentNullException`.
  - **Lines 25–30:** `IsSuccess`, `IsFailure => !IsSuccess`, `Error?` (null on success).
  - **Lines 32–35:** `Success()` and `Failure(Error)`.
- **Lines 39–42:** `[SuppressMessage("Design","CA1000")]`. CA1000 discourages static members on generic types; justified because `Result<T>.Success/Failure` are the agreed API.
- **Lines 43–70, `sealed class Result<T> : Result`:**
  - **Line 45:** `private readonly T _value;`
  - **Lines 47–57:** private success and failure constructors (`_value = default!` on failure).
  - **Lines 61–64:** `Value` returns `_value` on success and **throws `InvalidOperationException`** on failure, with the error code in the message. Reading it is a bug ("Check IsSuccess first").
  - **Line 66:** `Success(T value)`.
  - **Line 69:** `static new Result<T> Failure(Error)` hides the base `Failure` so it returns the typed result.

## Concepts used
- **Result/Either pattern:** explicit, typed failure paths; callers must check `IsSuccess`.
- **Invariant enforcement in constructors.**
- **Member hiding with `new`.**

## Data and control flow
Handler/domain → `Result<T>` → Dispatcher (rollback on failure) → caller (Day 11: HTTP mapping).

## Configuration and environment
None.

## Gotchas and issues
- `Result<T>` derives from non-sealed `Result`, so `Dispatcher.LogOutcome(Result result, ...)` accepts both. `Result` itself can be subclassed outside the assembly because its constructor is protected.
- There are no helpers such as `Match`, `Map` or implicit conversions; call sites must spell out `Result<T>.Success(...)`. That keeps the type simple at the cost of verbosity.
- `Success(null)` is allowed for reference types.

## Related files
- [Error.cs](Error.cs.md)
- [ResultTests.cs](../../../tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs.md)
- [Dispatcher.cs](../../TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
