# tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs

## Purpose

In-memory `ICentreRepository` with pre-seeded rows and a record of additions.

## Where It Fits

Application.Tests/Fakes. Used by `CreateCentreHandlerTests`.

## Walkthrough

`Existing` (rows that 'already exist') and `Added` (what the handler added). `ExistsBySlugAsync` searches both lists with LINQ (`Existing.Concat(Added).Any(...)`) and returns a completed task; `Add` appends to `Added`. Tests assert `Added` is empty on failure paths and has one element on success.

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

State-based fake.

#### How it works here

Two lists.

#### Why it matters here

Enables 'did not add' assertions.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Centres/ICentreRepository.cs`](../../../src/TutoringCentre.Application/Centres/ICentreRepository.cs.md)
- [`tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs`](../Centres/CreateCentreHandlerTests.cs.md)
