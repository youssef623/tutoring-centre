# tests/TutoringCentre.Architecture.Tests/TutoringCentre.Architecture.Tests.csproj

## Purpose
xUnit test project for **architecture rules across all four src projects (it references Domain, Application, Infrastructure and Api)**. It references only the project under test plus the test packages.

## Where it fits
Test layer. Listed in `TutoringCentre.slnx` under `/tests/`. Run by `dotnet test` locally and in CI (`ci.yml:35-36`).

## Walkthrough
- **`IsPackable=false`:** never packed as a NuGet package.
- **`<Using Include="Xunit" />`:** a global using, so test files need no `using Xunit;`.
- **Packages** (all versionless; versions come from `Directory.Packages.props`):
  - `coverlet.collector` (`PrivateAssets=all`)
  - `Microsoft.NET.Test.Sdk`
  - `xunit`
  - `xunit.runner.visualstudio` (`PrivateAssets=all`)
  - `NetArchTest.Rules` (IL-level dependency checks).
- **ProjectReference:** the project under test (see the file).

## Concepts used
- **Central Package Management:** no `Version=` attributes.
- **Global usings.**

## Data and control flow
Not applicable.

## Configuration and environment
None. Target framework and strictness come from `Directory.Build.props`, so test code also builds with `TreatWarningsAsErrors`. `.editorconfig` disables CA1707 for `tests/**`.

## Gotchas and issues
- `coverlet.collector` is referenced, but no workflow collects or publishes coverage.

## Related files
- [DependencyRuleTests.cs](DependencyRuleTests.cs.md)
- [ProjectReferenceTests.cs](ProjectReferenceTests.cs.md)
