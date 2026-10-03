# src/TutoringCentre.Infrastructure/AssemblyMarker.cs

## Purpose
An empty `internal sealed class AssemblyMarker;` that gives a type-safe handle to the Infrastructure assembly.

## Where it fits
Used by `tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs:11`. `AppDbContext` uses `typeof(AppDbContext).Assembly` for its own scanning instead.

## Walkthrough
- **Line 4:** type declaration.

## Concepts used
- **Assembly marker.** See [Domain marker](../TutoringCentre.Domain/AssemblyMarker.cs.md).

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [DependencyRuleTests.cs](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
