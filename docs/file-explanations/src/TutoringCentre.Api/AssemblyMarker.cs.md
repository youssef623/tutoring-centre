# src/TutoringCentre.Api/AssemblyMarker.cs

## Purpose

Assembly handle for architecture tests.

## Where It Fits

Api project.

## Walkthrough

`internal sealed class AssemblyMarker;` The Api's marker is not used by any current test (DependencyRuleTests scan only Domain/Application/Infrastructure).

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

Prepared for rules about the Api.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Unused today.

## Related Files

- [`src/TutoringCentre.Api/TutoringCentre.Api.csproj`](TutoringCentre.Api.csproj.md)
- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
