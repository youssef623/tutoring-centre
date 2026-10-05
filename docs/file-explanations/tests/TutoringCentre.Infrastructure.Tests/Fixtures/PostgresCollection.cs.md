# tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs

## Purpose

xUnit collection definition that makes all database tests share one `PostgresFixture`.

## Where It Fits

Infrastructure.Tests/Fixtures. Name `"postgres"` is used by `[Collection]` on `PostgresTestBase`.

## Walkthrough

`[CollectionDefinition(Name)] public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>` with `public const string Name = "postgres"`. Classes in one collection run sequentially (no parallelism), which is required because they share a database and reset it per test. CA1711 (type name ends in 'Collection') is suppressed with a justification.

## Concepts Used

### xUnit mechanics (facts, theories, collections, fixtures)

#### What it means

`[Fact]` is a test; `[Theory]` + `[InlineData]` runs one test with several inputs. A *collection fixture* (`ICollectionFixture<T>` + `[Collection]`) shares one expensive object (a database container) across test classes and serialises them; `IAsyncLifetime` runs async setup/teardown.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Collection fixture.

#### How it works here

Class.

#### Why it matters here

One container per run.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](PostgresFixture.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs`](PostgresTestBase.cs.md)
