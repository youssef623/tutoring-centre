# src/TutoringCentre.Infrastructure/AssemblyMarker.cs

## Purpose

Assembly handle for architecture tests.

## Where It Fits

Infrastructure layer; used by `DependencyRuleTests.InfrastructureAssembly`.

## Walkthrough

`internal sealed class AssemblyMarker;` Same pattern as the other three projects; `InternalsVisibleTo` makes it reachable from `Architecture.Tests`.

## Concepts Used

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

The marker.

#### How it works here

Declaration.

#### Why it matters here

Typo-proof assembly handle.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
- [`src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj`](TutoringCentre.Infrastructure.csproj.md)
