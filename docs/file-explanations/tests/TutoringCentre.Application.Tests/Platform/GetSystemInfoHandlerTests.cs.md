# tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs

## Purpose

Unit tests for `GetSystemInfoHandler` with a private fake read service.

## Where It Fits

Application.Tests/Platform.

## Walkthrough

Nested `FakeSystemInfoReadService(SchemaStatus)` returns a fixed status. Tests: zero pending -> `DatabaseUpToDate` true and migration echoed; two pending -> false; any status -> version non-blank and contains no `+`, and null migration preserved. Not tested: the `unknown` fallback (needs an assembly without a version).

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Inline fake.

#### How it works here

Nested class.

#### Why it matters here

Test independence from EF.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs`](../../../src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs.md)
- [`src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs`](../../../src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs.md)
