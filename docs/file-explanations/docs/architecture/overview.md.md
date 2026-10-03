# docs/architecture/overview.md

## Purpose

Explains the failure model (Result vs exceptions), the dispatcher pipeline design and the full path of `CreateCentreCommand` from CLI to table.

## Where It Fits

Documentation. Source of truth for `Dispatcher.cs` behaviour and for `docs/notes/pipeline-cases.md`.

## Walkthrough

**Failures: results vs exceptions** - table mapping situations to mechanisms; kinds (closed set), codes (`<feature>.<reason>`), messages (developer-only), safety (no internals). **Dispatcher design** - ASCII flow for `Send(command)` and `Query(query)`; why validation runs before BEGIN; why the transaction starts before the handler (row locks `FOR UPDATE`, Month-2 row-level-security tenant setting must cover the whole handler); what happens when a query handler throws; who calls SaveChanges; where tenant/permission checks go (after validation, before BEGIN - not implemented). **Request path: CreateCentreCommand** - 11-step table of files. I verified every referenced file exists and the order matches `Program.cs`, `SeedCommand.cs`, `Dispatcher.cs`, `CreateCentreHandler.cs`.

## Concepts Used

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Dispatcher design section.

#### How it works here

Describes `Dispatcher.SendAsync/QueryAsync`.

#### Why it matters here

Documented reasons for the transaction placement.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Mentions 'Day 7/9/11', 'Month 2/3/6', 'Week 2' from a plan not in the repo.

## Related Files

- [`src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`](../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md)
- [`docs/notes/pipeline-cases.md`](../notes/pipeline-cases.md.md)
- [`src/TutoringCentre.Api/Cli/SeedCommand.cs`](../../src/TutoringCentre.Api/Cli/SeedCommand.cs.md)
- [`docs/architecture/api-conventions.md`](api-conventions.md.md)
