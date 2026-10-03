# src/TutoringCentre.Domain/Common/Error.cs

## Purpose
The value describing an **expected business failure**:
- a stable machine-readable `Code` (`<feature>.<reason>`) that the frontend translates;
- a developer `Message`;
- an `ErrorKind`;
- optional per-field validation messages.

## Where it fits
Domain/Common. Carried by `Result`. Created by `Centre.Create` and by `Dispatcher.ValidateAsync` (`validation.failed`). Mirrored by `frontend/src/api/errors.ts` (`ApiError`) and `problemDetails.ts` (`errors`). Rules are in `docs/architecture/overview.md:17-23`.

## Walkthrough
- **Lines 10–13:** `[SuppressMessage("Naming","CA1716")]`. CA1716 flags identifiers that are keywords in another .NET language (`Error` is reserved in VB). Justified as "C#-only solution".
- **Lines 14–18:** `public sealed record Error(string Code, string Message, ErrorKind Kind, IReadOnlyDictionary<string,string[]>? Fields = null)`. Being a positional record gives value equality, immutability and `ToString`.
- **Line 20:** `Validation(code, message)` with no fields.
- **Lines 22–28:** `Validation(code, message, fields)`. It null-guards `fields`. Doc: the fields become the HTTP 400 `errors` object (Day 11). Keys are camelCase field names.
- **Lines 30–33:** factories `NotFound`, `Conflict`, `Rule`, `Forbidden`.

## Concepts used
- **Records:** immutable data with value equality. `DispatcherTests` relies on this with `Assert.Equal(TestErrors.HandlerFailure, result.Error)`.
- **Static factory methods:** one per kind, so call sites read `Error.Conflict(...)`.

## Data and control flow
Created in Domain or Application → wrapped in `Result.Failure` → (Day 11) mapped to Problem Details → translated by the frontend's `messageFor`.

## Configuration and environment
None.

## Gotchas and issues
- Record equality on `Fields` is *reference* equality of the dictionary, not content equality. Two errors with equal but separate dictionaries are not `Equals`.
- Nothing enforces the code format `<feature>.<reason>` at runtime.

## Related files
- [ErrorKind.cs](ErrorKind.cs.md)
- [Result.cs](Result.cs.md)
- [Dispatcher.cs](../../TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
- [ErrorTests.cs](../../../tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs.md)
- [errors.ts](../../../frontend/src/api/errors.ts.md)
