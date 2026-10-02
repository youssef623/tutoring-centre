# Dispatcher pipeline cases (specification for DispatcherTests)

Call names recorded by the fake unit of work: `Begin(rw)`, `Begin(ro)`, `Save`, `Commit`, `Rollback`; the test handler records `Handle`.

| Case | Scenario | Expected call sequence | Expected result |
| --- | --- | --- | --- |
| C1 | Valid command, handler succeeds | `Begin(rw)`, `Handle`, `Save`, `Commit` | success |
| C2 | Invalid command | (none — handler not called, nothing begun) | failure `validation.failed` with field errors |
| C3 | Handler returns a failure result | `Begin(rw)`, `Handle`, `Rollback` (no `Save`, no `Commit`) | the handler's same error |
| C4 | Handler throws | `Begin(rw)`, `Handle`, `Rollback` | exception propagates |
| C5 | `SaveChanges` throws | `Begin(rw)`, `Handle`, `Save`, `Rollback` (no `Commit`) | exception propagates |
| C6 | Valid query | `Begin(ro)`, `Handle`, `Commit` (never `Save`) | success |
| C7 | Invalid query | (none) | failure `validation.failed` |
| C8 | Query handler throws | `Begin(ro)`, `Handle`, `Rollback` | exception propagates |
| C9 | Several validation errors | (none) | `Fields` grouped by camelCase field name |
