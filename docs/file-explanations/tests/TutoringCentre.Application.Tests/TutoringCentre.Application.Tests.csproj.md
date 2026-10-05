# tests/TutoringCentre.Application.Tests/TutoringCentre.Application.Tests.csproj

## Purpose

Project file for Application unit tests; references Application and the DI package used to build a service provider in `DispatcherTests`.

## Where It Fits

tests/. Needs `InternalsVisibleTo` granted by `Application.csproj` to access `internal` handlers/validators.

## Walkthrough

Every test project sets `IsPackable=false` (never published as a NuGet package), adds `<Using Include="Xunit" />` (a global using so test files need no `using Xunit;`), and references `coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit` and `xunit.runner.visualstudio` (with `IncludeAssets`/`PrivateAssets` so the runner is not flowed). Versions come from `Directory.Packages.props`. No code coverage step runs in CI even though `coverlet.collector` is referenced. Extra: `Microsoft.Extensions.DependencyInjection` (concrete container) so `DispatcherTests` can call `new ServiceCollection()...BuildServiceProvider()`.

## Concepts Used

### xUnit mechanics (facts, theories, collections, fixtures)

#### What it means

`[Fact]` is a test; `[Theory]` + `[InlineData]` runs one test with several inputs. A *collection fixture* (`ICollectionFixture<T>` + `[Collection]`) shares one expensive object (a database container) across test classes and serialises them; `IAsyncLifetime` runs async setup/teardown.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Packages.

#### How it works here

Items.

#### Why it matters here

Test discovery.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/TutoringCentre.Application.csproj`](../../src/TutoringCentre.Application/TutoringCentre.Application.csproj.md)
- [`tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs`](Cqrs/DispatcherTests.cs.md)
