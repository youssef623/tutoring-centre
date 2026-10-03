# src/TutoringCentre.Application/Common/Cqrs/Unit.cs

## Purpose
`Unit` is the "no data" response type for commands that only succeed or fail, e.g. `ICommand<Unit>`. It lets `Result<T>` and the generic pipeline handle void-like commands uniformly.

## Where it fits
Application CQRS contracts. **Not used anywhere yet** (verified by grep).

## Walkthrough
- **Line 4:** `public readonly record struct Unit`. It's a value type, so there is no allocation, and it gets value equality.
- **Line 7:** `public static readonly Unit Value;`. The comment notes the static field is default-initialised, which is the single value of the type.

## Concepts used
- **Unit type** (from functional programming): a type with exactly one value, representing "nothing".
- **readonly record struct.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [ICommand.cs](ICommand.cs.md)
- [Result.cs](../../../TutoringCentre.Domain/Common/Result.cs.md)
