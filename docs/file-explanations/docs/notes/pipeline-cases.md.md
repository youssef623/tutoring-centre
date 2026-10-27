# `docs/notes/pipeline-cases.md`

## Purpose

Documentation covering Dispatcher pipeline cases (specification for DispatcherTests). Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../notes/pipeline-cases.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 2,796 bytes. Source hash: `8ea0ac8ab1080365798b5303033f8c2796cc7b93a795f06516a3ff73c7705105`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Dispatcher pipeline cases (specification for DispatcherTests)

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
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
| C15 | Permission-requiring request, staff role holds the permission | `Begin(rw)`, `Handle`, `Save`, `Commit` | success |
| C16 | Permission-requiring request, staff role does not hold it (command and query variants) | (none — handler not called, nothing begun) | failure `auth.permission_denied` (Forbidden) |
| C17 | Permission-requiring request, system actor | `Begin(rw)`, `Handle`, `Save`, `Commit` | success, regardless of permission |
```

The excerpt stops at line 24; use the original file for the remaining 3 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
