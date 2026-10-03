# tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs

## Purpose

Proves the seed command is idempotent and creates exactly the two demo centres.

## Where It Fits

Api.Tests/Cli; collection `api`. Calls `SeedCommand.RunAsync(factory.Services)` directly.

## Walkthrough

Reset DB in `InitializeAsync`. Act: run twice. Assert: both exit codes 0; `select count(*) from platform.centres` = 2; `string_agg(slug, ',' order by slug)` = `maadi-hub,nile-centre`. The second run hits `centre.slug_taken`, which the command treats as success. Not tested: failure exit code 1.

## Concepts Used

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

CLI client of the dispatcher.

#### How it works here

`SeedCommand.RunAsync`.

#### Why it matters here

Verifies a non-HTTP front door.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Cli/SeedCommand.cs`](../../../src/TutoringCentre.Api/Cli/SeedCommand.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs`](../Fixtures/ApiFactory.cs.md)
