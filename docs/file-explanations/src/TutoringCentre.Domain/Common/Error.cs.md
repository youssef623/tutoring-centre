# src/TutoringCentre.Domain/Common/Error.cs

## Purpose

Immutable description of an expected business failure: a stable code, a developer message, a kind and optional per-field messages.

## Where It Fits

Domain/Common. Created in `Centre.Create`, `Membership.Create`, `CreateCentreHandler`, the identity handlers, `IdentityAuthenticationService`, `Dispatcher.ValidateAsync`, `Program.cs` (fallback routes). Mapped to HTTP by `ResultHttpExtensions.ToProblemResult`; mirrored by frontend `errors.ts`/`errorMessages.ts`.

## Walkthrough

`public sealed record Error(string Code, string Message, ErrorKind Kind, IReadOnlyDictionary<string,string[]>? Fields = null)` (line 14). Because it is a record, two errors with the same values are equal - `DispatcherTests` asserts `Assert.Equal(TestErrors.HandlerFailure, result.Error)`.
Factory methods (lines 20-34): `Validation(code,message)`; `Validation(code,message,fields)` which null-checks `fields`; `NotFound`; `Conflict`; `Rule`; `Forbidden`; `Unauthenticated` (line 34, added for 401). Each sets the matching `ErrorKind`.
Doc comment contract: `Code` is `<feature>.<reason>` (`centre.slug_invalid`), a stable machine-readable id the frontend translates; `Message` is for developers and logs. `[SuppressMessage CA1716]`: `Error` is a keyword in Visual Basic, irrelevant in this C#-only solution.

## Concepts Used

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

The record declaration.

#### How it works here

`public sealed record Error(...)`.

#### Why it matters here

Value equality, immutability, `with`-expressions and a compact declaration for a message-like type.

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW2.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

Role as the failure half of `Result`.

#### How it works here

`Result.Failure(Error)`.

#### Why it matters here

Carries code + kind so the Api can choose the HTTP status without knowing business rules.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

`Fields` is a mutable-array dictionary typed as read-only; arrays inside are mutable, but nothing mutates them.

## Related Files

- [`src/TutoringCentre.Domain/Common/ErrorKind.cs`](ErrorKind.cs.md)
- [`src/TutoringCentre.Domain/Common/Result.cs`](Result.cs.md)
- [`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`](../../TutoringCentre.Api/Http/ResultHttpExtensions.cs.md)
- [`frontend/src/api/errorMessages.ts`](../../../frontend/src/api/errorMessages.ts.md)
- [`tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs`](../../../tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs.md)
