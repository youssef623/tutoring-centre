# tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs

## Purpose

Base class for database tests: puts the class into the postgres collection and resets all rows before each test.

## Where It Fits

Infrastructure.Tests/Fixtures. Inherited by `CreateCentreTests`, `CentreSlugRaceTests`, `MigrationTests`, `ReadOnlyQueryTests`, `TransactionBoundaryTests`, `SystemInfoQueryTests`.

## Walkthrough

`[Collection(PostgresCollection.Name)] abstract class PostgresTestBase(PostgresFixture fixture) : IAsyncLifetime`; `Fixture` property; `InitializeAsync() => Fixture.ResetAsync()` (runs before every test method because xUnit creates a new instance per test); `DisposeAsync` is a no-op.

## Concepts Used

### xUnit mechanics (facts, theories, collections, fixtures)

#### What it means

`[Fact]` is a test; `[Theory]` + `[InlineData]` runs one test with several inputs. A *collection fixture* (`ICollectionFixture<T>` + `[Collection]`) shares one expensive object (a database container) across test classes and serialises them; `IAsyncLifetime` runs async setup/teardown.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Per-test setup via `IAsyncLifetime`.

#### How it works here

`InitializeAsync`.

#### Why it matters here

Each test starts with empty tables.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`](PostgresFixture.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs`](PostgresCollection.cs.md)
