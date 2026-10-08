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
| C10 | Tenant-scoped command, staff actor without a centre | (none — handler not called, nothing begun) | failure `tenant.not_selected` (Forbidden) |
| C11 | Tenant-scoped query, staff actor without a centre | (none) | failure `tenant.not_selected` (Forbidden) |
| C12 | Tenant-scoped request (command or query), anonymous actor | (none) | failure `auth.not_authenticated` (Unauthenticated) |
| C13 | Tenant-scoped request with a centre; non-scoped request without a centre | `Handle` runs either way | success |
| — | Ordering: tenant-scoped command, invalid input, actor without a centre | (none) | failure `validation.failed` — validation wins over the tenant step |
| C14 | Handler succeeds, `SaveChangesAsync` returns a failed Result | `Begin(rw)`, `Handle`, `Save`, `Rollback` (no `Commit`) | the save's own error (Task 25.3) |
