# tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs

## Purpose

xUnit collection for tests sharing `ConventionsFactory`.

## Where It Fits

Api.Tests/Fixtures; name `"conventions"`.

## Walkthrough

`[CollectionDefinition(Name)] class ConventionsCollection : ICollectionFixture<ConventionsFactory>`; used by CorrelationId, LoggingConvention and ProblemDetails tests.

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

One host with test-only endpoints.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs`](ConventionsFactory.cs.md)
