# src/TutoringCentre.Domain/AssemblyMarker.cs

## Purpose

An empty internal class used only so tests can obtain this assembly with `typeof(AssemblyMarker).Assembly`.

## Where It Fits

Domain layer. Used by `tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`.

## Walkthrough

`internal sealed class AssemblyMarker;` (C# 12 semicolon body, line 5). `internal` plus `InternalsVisibleTo` keeps it invisible to production consumers. A compile-time `typeof` fails to build if the type is renamed, unlike `Assembly.Load("TutoringCentre.Domain")`.

## Concepts Used

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

The marker type.

#### How it works here

`DependencyRuleTests.DomainAssembly = typeof(TutoringCentre.Domain.AssemblyMarker).Assembly`.

#### Why it matters here

Typo-proof assembly handle.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

None.

## Related Files

- [`src/TutoringCentre.Domain/TutoringCentre.Domain.csproj`](TutoringCentre.Domain.csproj.md)
- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
- [`src/TutoringCentre.Application/AssemblyMarker.cs`](../TutoringCentre.Application/AssemblyMarker.cs.md)
