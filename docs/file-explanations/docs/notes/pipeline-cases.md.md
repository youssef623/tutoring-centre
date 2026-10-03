# docs/notes/pipeline-cases.md

## Purpose
The **test specification** for `DispatcherTests`: nine cases (C1–C9), each with a scenario, the exact expected sequence of unit-of-work calls, and the expected result.

## Where it fits
Test documentation. It defines the call names (`Begin(rw)`, `Begin(ro)`, `Save`, `Commit`, `Rollback`, `Handle`) that `tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs` and `Cqrs/TestRequests.cs` record.

## Walkthrough
| Case | Scenario | Sequence | Test method |
| --- | --- | --- | --- |
| C1 | valid command, success | Begin(rw), Handle, Save, Commit | `SendAsync_ValidCommandHandlerSucceeds_BeginsHandlesSavesAndCommits` |
| C2 | invalid command | none | `SendAsync_InvalidCommand_DoesNotBeginOrCallHandler` |
| C3 | handler returns failure | Begin(rw), Handle, Rollback | `SendAsync_HandlerReturnsFailure_RollsBackWithoutSavingOrCommitting` |
| C4 | handler throws | Begin(rw), Handle, Rollback | `SendAsync_HandlerThrows_RollsBackAndRethrows` |
| C5 | SaveChanges throws | Begin(rw), Handle, Save, Rollback | `SendAsync_SaveChangesThrows_RollsBackAndDoesNotCommit` |
| C6 | valid query | Begin(ro), Handle, Commit | `QueryAsync_ValidQuery_BeginsReadOnlyHandlesAndCommitsWithoutSaving` |
| C7 | invalid query | none | `QueryAsync_InvalidQuery_DoesNotBeginOrCallHandler` |
| C8 | query handler throws | Begin(ro), Handle, Rollback | `QueryAsync_HandlerThrows_RollsBackAndRethrows` |
| C9 | several validation errors | none; `Fields` grouped by camelCase name | `SendAsync_SeveralFieldFailures_GroupsMessagesByCamelCaseFieldName` |

## Concepts used
- **Specification-first testing.**
- **Call-recording fakes** (a "spy" test double).

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Not covered by the spec or the tests: commit throwing, rollback throwing, and a query handler returning a failure Result.

## Related files
- [DispatcherTests.cs](../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs.md)
- [FakeUnitOfWork.cs](../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs.md)
- [overview.md](../architecture/overview.md.md)
