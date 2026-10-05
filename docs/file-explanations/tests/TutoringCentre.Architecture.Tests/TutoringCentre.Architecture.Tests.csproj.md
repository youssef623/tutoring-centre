# tests/TutoringCentre.Architecture.Tests/TutoringCentre.Architecture.Tests.csproj

## Purpose

Project file for the architecture tests; references all four `src` projects and the NetArchTest library.

## Where It Fits

tests/. The only test project that references every layer; it exists to inspect relationships between them.

## Walkthrough

Every test project sets `IsPackable=false` (never published as a NuGet package), adds `<Using Include="Xunit" />` (a global using so test files need no `using Xunit;`), and references `coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit` and `xunit.runner.visualstudio` (with `IncludeAssets`/`PrivateAssets` so the runner is not flowed). Versions come from `Directory.Packages.props`. No code coverage step runs in CI even though `coverlet.collector` is referenced. Extra: `NetArchTest.Rules`, and `ProjectReference`s to Domain, Application, Infrastructure and Api so each assembly is loaded and its `AssemblyMarker` can be referenced (each `src` project grants `InternalsVisibleTo`).

## Concepts Used

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

References to all layers.

#### How it works here

Four `ProjectReference` items.

#### Why it matters here

Allows `typeof(...AssemblyMarker).Assembly` for each layer.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Referencing the Api project transitively pulls in ASP.NET Core assemblies; harmless because the rules inspect only chosen assemblies.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](DependencyRuleTests.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`](ProjectReferenceTests.cs.md)
