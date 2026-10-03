# tests/TutoringCentre.Domain.Tests/TutoringCentre.Domain.Tests.csproj

## Purpose

Project file for the Domain unit tests; references only the Domain project.

## Where It Fits

tests/. Run by `dotnet test TutoringCentre.slnx` in CI. Depends on `src/TutoringCentre.Domain`.

## Walkthrough

Every test project sets `IsPackable=false` (never published as a NuGet package), adds `<Using Include="Xunit" />` (a global using so test files need no `using Xunit;`), and references `coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit` and `xunit.runner.visualstudio` (with `IncludeAssets`/`PrivateAssets` so the runner is not flowed). Versions come from `Directory.Packages.props`. No code coverage step runs in CI even though `coverlet.collector` is referenced. It references only `TutoringCentre.Domain`, so these tests cannot accidentally depend on Application or Infrastructure.

## Concepts Used

### xUnit mechanics (facts, theories, collections, fixtures)

#### What it means

`[Fact]` is a test; `[Theory]` + `[InlineData]` runs one test with several inputs. A *collection fixture* (`ICollectionFixture<T>` + `[Collection]`) shares one expensive object (a database container) across test classes and serialises them; `IAsyncLifetime` runs async setup/teardown.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests](../../../PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Test SDK + xUnit packages.

#### How it works here

`PackageReference` items.

#### Why it matters here

Makes `dotnet test` discover `[Fact]`/`[Theory]`.

### MSBuild, Central Package Management and analyzers

#### What it means

MSBuild builds .NET projects from XML project files. `Directory.Build.props` is imported into every project below it. Central Package Management keeps all NuGet versions in one file. Analyzers are compile-time rules; `TreatWarningsAsErrors` makes violations fail the build.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#5-startup--bootstrapping](../../../PROJECT_OVERVIEW.md#5-startup--bootstrapping).)

#### Where it appears in this file

Versionless references.

#### How it works here

All package references.

#### Why it matters here

Versions central.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`Directory.Packages.props`](../../Directory.Packages.props.md)
- [`src/TutoringCentre.Domain/TutoringCentre.Domain.csproj`](../../src/TutoringCentre.Domain/TutoringCentre.Domain.csproj.md)
- [`.github/workflows/ci.yml`](../../.github/workflows/ci.yml.md)
