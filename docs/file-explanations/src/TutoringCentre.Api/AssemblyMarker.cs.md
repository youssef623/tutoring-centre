# src/TutoringCentre.Api/AssemblyMarker.cs

## Purpose
An empty `internal sealed class AssemblyMarker;` for a type-safe handle on the Api assembly.

## Where it fits
Api layer. **Not referenced by any test currently.** `DependencyRuleTests` checks only Domain, Application and Infrastructure. It exists for symmetry and future rules.

## Walkthrough
- **Line 4:** type declaration.

## Concepts used
- **Assembly marker.** See [Domain marker](../TutoringCentre.Domain/AssemblyMarker.cs.md).

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Currently unused.

## Related files
- [DependencyRuleTests.cs](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
