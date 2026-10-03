# src/TutoringCentre.Domain/AssemblyMarker.cs

## Purpose
An empty `internal sealed class AssemblyMarker;` (line 4). Tests use it to get a compile-time-checked handle on the Domain assembly: `typeof(AssemblyMarker).Assembly`.

## Where it fits
Domain layer, test support. Used by `tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs:9`. Each of the four `src` projects has an identical marker.

## Walkthrough
- **Line 1:** file-scoped namespace `TutoringCentre.Domain`.
- **Line 3:** XML doc: "Contains no logic."
- **Line 4:** a type declaration with no body (C# 12 allows `class X;`).

## Concepts used
- **Assembly marker type:** a safer alternative to `Assembly.Load("name")`, because a typo fails at compile time instead of at runtime.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Removing the matching `InternalsVisibleTo` causes a CS0122 error in the architecture tests (noted in the old `tests/.semantic.md:96`).

## Related files
- [DependencyRuleTests.cs](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
- [TutoringCentre.Domain.csproj](TutoringCentre.Domain.csproj.md)
