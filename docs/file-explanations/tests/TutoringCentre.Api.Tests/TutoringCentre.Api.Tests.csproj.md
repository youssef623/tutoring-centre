# tests/TutoringCentre.Api.Tests/TutoringCentre.Api.Tests.csproj

## Purpose
xUnit test project for **the Api layer (HTTP-level tests)**. It references only the project under test plus the test packages.

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
  - `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`). The project uses `Microsoft.NET.Sdk.Web` so the ASP.NET Core framework is available.
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
- [HealthEndpointTests.cs](Health/HealthEndpointTests.cs.md)
- [Program.cs](../../src/TutoringCentre.Api/Program.cs.md)
