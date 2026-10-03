# tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs

## Purpose

Proves that two simultaneous creates with the same slug leave exactly one row, because the unique index is the real guarantee.

## Where It Fits

Infrastructure.Tests/Centres. Targets the TOCTOU window between `ExistsBySlugAsync` and `INSERT`.

## Walkthrough

Arrange: command for `race-centre`; `TaskCompletionSource gate` and local `AttemptAsync()` that awaits the gate, sends as `SystemActor`, and maps outcomes to an enum: `Created`, `SlugTakenResult` (failure result), or `UniqueViolationException` (catches `DbUpdateException` whose `InnerException` is `PostgresException { SqlState: "23505" }`). Act: start two `Task.Run(AttemptAsync)`, `gate.SetResult()` to release both together, `await Task.WhenAll`. Assert: exactly one `Created`, exactly one other outcome, and exactly one row. Either losing outcome is accepted: if the loser's existence check ran late it gets the friendly result; if both checks passed, the unique index rejects the second insert. The summary notes Month 2 will translate the exception into 409.

## Concepts Used

### Concurrency, TOCTOU and unique indexes

#### What it means

TOCTOU (time of check to time of use): code checks a condition then acts, but another request can change the condition in between. Only the database can make "no two rows share a slug" true, via a unique index; application checks just produce friendlier errors in the common case.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#610-concurrency-the-slug-race-and-why-the-unique-index-is-the-real-guarantee](../../../../PROJECT_OVERVIEW.md#610-concurrency-the-slug-race-and-why-the-unique-index-is-the-real-guarantee).)

#### Where it appears in this file

TOCTOU test.

#### How it works here

Whole test.

#### Why it matters here

Documents a real concurrency hazard and its guarantee.

### async/await and cancellation

#### What it means

`async`/`await` lets a method wait for I/O (database, network) without blocking a thread: the method returns a `Task`, and execution resumes after the awaited operation completes. A `CancellationToken` is a cooperative signal (for example, the HTTP request was aborted) passed down so work can stop early.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

`TaskCompletionSource` gate, `Task.Run`, `WhenAll`.

#### How it works here

Test body.

#### Why it matters here

Forces concurrent execution.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Timing-dependent which outcome occurs; the assertion is deliberately outcome-agnostic.

## Related Files

- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs`](../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs`](../Fixtures/PostgresTestBase.cs.md)
