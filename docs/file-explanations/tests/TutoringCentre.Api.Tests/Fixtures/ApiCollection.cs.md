# tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs

## Purpose

xUnit collection for tests sharing `ApiFactory`.

## Where It Fits

Api.Tests/Fixtures; name `"api"`.

## Walkthrough

`[CollectionDefinition(Name)] class ApiCollection : ICollectionFixture<ApiFactory>`; used by `SeedCommandTests` and `ReadinessWithDatabaseTests`. Sequential execution, one container. CA1711 suppression with justification.

## Concepts Used

### xUnit mechanics (facts, theories, collections, fixtures)

#### What it means

`[Fact]` is a test; `[Theory]` + `[InlineData]` runs one test with several inputs. A *collection fixture* (`ICollectionFixture<T>` + `[Collection]`) shares one expensive object (a database container) across test classes and serialises them; `IAsyncLifetime` runs async setup/teardown.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Collection fixture.

#### How it works here

Class.

#### Why it matters here

Shared expensive host.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs`](ApiFactory.cs.md)
- [`tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs`](../Cli/SeedCommandTests.cs.md)
