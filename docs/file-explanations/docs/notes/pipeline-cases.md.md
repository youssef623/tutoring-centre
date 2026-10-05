# docs/notes/pipeline-cases.md

## Purpose

Specification table (cases C1-C9) that `DispatcherTests` implements.

## Where It Fits

Documentation note under `docs/notes/`; written before the tests ('specification for DispatcherTests').

## Walkthrough

Defines the fake unit-of-work call names (`Begin(rw)`, `Begin(ro)`, `Save`, `Commit`, `Rollback`) and nine cases: C1 valid command succeeds -> `Begin(rw),Handle,Save,Commit`; C2 invalid command -> no calls, `validation.failed`; C3 handler returns failure -> `Begin(rw),Handle,Rollback`; C4 handler throws -> same calls, exception propagates; C5 SaveChanges throws -> `Begin(rw),Handle,Save,Rollback`; C6 valid query -> `Begin(ro),Handle,Commit`; C7 invalid query; C8 query handler throws -> `Begin(ro),Handle,Rollback`; C9 several validation errors grouped by camelCase field.

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

The call-name list.

#### How it works here

Defines the vocabulary recorded by `FakeUnitOfWork.Calls`.

#### Why it matters here

Lets the pipeline order be asserted without a database.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

None.

## Related Files

- [`tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs`](../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs.md)
- [`tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs`](../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs.md)
- [`docs/architecture/overview.md`](../architecture/overview.md.md)
