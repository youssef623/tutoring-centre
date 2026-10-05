# tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs

## Purpose

Single place that names the four production assemblies for architecture tests, obtained from marker types.

## Where It Fits

Architecture.Tests. Used by every rule class added after the merge (`ApiRuleTests`, `CqrsRuleTests`, `IdentityRuleTests`, `RepositoryRuleTests`) and by the new test in `DependencyRuleTests`.

## Walkthrough

`SourceAssemblies` (5-16): properties `Domain`, `Application`, `Infrastructure` (each `typeof(<Layer>.AssemblyMarker).Assembly`) and `Api` (`typeof(Program).Assembly`), plus `All` (collection expression of the four). The marker types exist only to give tests a stable handle on each assembly (see the `AssemblyMarker.cs` files).

## Concepts Used

### Assembly scanning and reflection

#### What it means

*Reflection* lets code inspect types at runtime (`assembly.GetTypes()`, `type.GetInterfaces()`). *Assembly scanning* uses it to find classes implementing a given interface and register them automatically, so adding a new handler needs no registration line.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning](../../../PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Assembly handles via marker types.

#### How it works here

The four properties and `All`.

#### Why it matters here

A marker class is a compile-time-checked way to refer to an assembly; a string name would fail only at runtime.

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

Shared input for all rules.

#### How it works here

`All` and the four properties.

#### Why it matters here

One definition of 'the production code' for every architecture test.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

`DependencyRuleTests` still declares its own assembly handles for Domain/Application/Infrastructure (lines 10-12); the new rule classes use this helper instead, so both styles coexist.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](DependencyRuleTests.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs`](ApiRuleTests.cs.md)
- [`src/TutoringCentre.Domain/AssemblyMarker.cs`](../../src/TutoringCentre.Domain/AssemblyMarker.cs.md)
- [`src/TutoringCentre.Api/Program.cs`](../../src/TutoringCentre.Api/Program.cs.md)
