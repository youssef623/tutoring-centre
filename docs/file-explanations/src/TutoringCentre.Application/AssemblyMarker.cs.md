# src/TutoringCentre.Application/AssemblyMarker.cs

## Purpose
An empty `internal sealed class AssemblyMarker;` that gives a type-safe handle to the Application assembly.

## Where it fits
Used in two places:
- `DependencyInjection.AddApplication` (`DependencyInjection.cs:18`): `AddCqrsHandlers(typeof(AssemblyMarker).Assembly)` scans *this* assembly for handlers and validators;
- `tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs:10`.

## Walkthrough
- **Line 4:** a type with no body.

## Concepts used
- **Assembly marker.** See [the Domain marker](../TutoringCentre.Domain/AssemblyMarker.cs.md).

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [DependencyInjection.cs](DependencyInjection.cs.md)
- [HandlerRegistration.cs](Common/Cqrs/HandlerRegistration.cs.md)
