# tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs

## Purpose

Unit tests for the `Centre.Create` business rules.

## Where It Fits

Domain.Tests. Tests `src/TutoringCentre.Domain/Centres/Centre.cs`; no dependencies beyond Domain.

## Walkthrough

Constants `ValidName`, `ValidSlug`, `Cairo` and helper `AssertValidationFailure(result, code)` (asserts failure, error not null, expected code, and `ErrorKind.Validation`).
- `Create_WithValidInput_SucceedsAndTrimsName`: Arrange - none; Act - `Create("  Nour Academy  ", ...)`; Assert - success, name trimmed, other fields copied, `Id != Guid.Empty`.
- `Create_WithEmptyName_ReturnsNameRequired`, `Create_WithWhitespaceOnlyName_ReturnsNameRequired` (pins that `IsNullOrWhiteSpace` covers whitespace).
- `Create_With121CharacterName_ReturnsNameTooLong` and `Create_With120CharacterName_Succeeds` (inclusive boundary).
- `[Theory] Create_WithInvalidSlug_ReturnsSlugInvalid` for `"ab"`, `"Bad Slug"`, `"-abc"`, `"abc--def"`.
- `Create_WithUnknownTimeZone_ReturnsTimeZoneInvalid` (`Mars/Base`) and `Create_WithCairoTimeZone_Succeeds`.
Not tested: slug length 60/61, null arguments, uppercase letters alone, trailing newline in slug, whitespace-only time zone.

## Concepts Used

### xUnit mechanics (facts, theories, collections, fixtures)

#### What it means

`[Fact]` is a test; `[Theory]` + `[InlineData]` runs one test with several inputs. A *collection fixture* (`ICollectionFixture<T>` + `[Collection]`) shares one expensive object (a database container) across test classes and serialises them; `IAsyncLifetime` runs async setup/teardown.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

`[Fact]`/`[Theory]`/`[InlineData]`.

#### How it works here

All methods.

#### Why it matters here

Parameterised slug cases.

### The Result pattern (failures as values)

#### What it means

Expected business failures (invalid input, duplicate, not allowed) are returned as ordinary values - a `Result` holding either a value or an `Error` - instead of thrown. Exceptions are reserved for bugs and infrastructure faults. The caller's code must look at the result, so the failure path cannot be forgotten, and no exception-handling cost or hidden control flow is involved.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#64-the-result-pattern-failures-as-values](../../../../PROJECT_OVERVIEW.md#64-the-result-pattern-failures-as-values).)

#### Where it appears in this file

Asserting `Result` values.

#### How it works here

`result.IsSuccess`, `result.Error.Code`.

#### Why it matters here

Failures are values, so tests don't need `Assert.Throws`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Time-zone tests depend on the OS time-zone database containing `Africa/Cairo`.

## Related Files

- [`src/TutoringCentre.Domain/Centres/Centre.cs`](../../../src/TutoringCentre.Domain/Centres/Centre.cs.md)
- [`src/TutoringCentre.Domain/Common/Result.cs`](../../../src/TutoringCentre.Domain/Common/Result.cs.md)
