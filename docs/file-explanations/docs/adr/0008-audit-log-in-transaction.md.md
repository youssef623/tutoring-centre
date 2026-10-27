# `docs/adr/0008-audit-log-in-transaction.md`

## Purpose

Documentation covering ADR 0008: Audit log written in the command's own transaction; Context; Alternatives considered; Decision; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0008-audit-log-in-transaction.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 64 lines and 4,822 bytes. Source hash: `7e176dae23cc9069e607a2267cc21059deeb9607b6dedddc0ce2ca89e99f52dc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../adr/0008-audit-log-in-transaction.md#L8): `A role change, a subject rename or a centre settings change needs a durable record of who changed what and`
- [Line 9](../../../adr/0008-audit-log-in-transaction.md#L9): `when, with the before and after values — and that record has to exist whenever the change itself exists,`
- [Line 10](../../../adr/0008-audit-log-in-transaction.md#L10): `with nothing that can commit the data change but lose its audit row, or the reverse. The record also must`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0008: Audit log written in the command's own transaction

- **Status:** Accepted
- **Date:** 2026-10-10

## Context

A role change, a subject rename or a centre settings change needs a durable record of who changed what and
when, with the before and after values — and that record has to exist whenever the change itself exists,
with nothing that can commit the data change but lose its audit row, or the reverse. The record also must
never block or meaningfully slow down the everyday commands it watches, and it must not give any handler a
new thing to remember.

## Alternatives considered

- **Explicit handler calls** (each handler that changes an audited entity calls an `IAuditLog.Record(...)`
  port itself) keeps the mechanism visible at the call site, but puts the audit allow-list's enforcement in
  every handler's hands rather than in one place: a handler that forgets the call, or a new handler for a
  new command on an already-audited entity, silently produces no audit row, and nothing fails the build to
  say so. It also means Application code has to know field-level detail (what counts as a change, what
  values are allowed) that belongs to the audit concern, not the use case.
- **Database triggers** (a PostgreSQL trigger on `UPDATE`/`INSERT`/`DELETE` of an audited table, building the
  audit row from `OLD`/`NEW`) genuinely cannot be bypassed by any application-layer mistake, and would catch
  a write made outside the application entirely. It costs real things this product does not want yet: audit
```

The excerpt stops at line 24; use the original file for the remaining 40 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
