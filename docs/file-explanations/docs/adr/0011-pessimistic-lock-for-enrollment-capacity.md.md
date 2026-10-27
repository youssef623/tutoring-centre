# `docs/adr/0011-pessimistic-lock-for-enrollment-capacity.md`

## Purpose

Documentation covering ADR 0011: Pessimistic lock for enrollment capacity; Context; Alternatives considered; Decision; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0011-pessimistic-lock-for-enrollment-capacity.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 155 lines and 11,113 bytes. Source hash: `4b41ea4cb312c1d2c3bd4bf2657ccee1a48e146cde18bb534e6e0d2d0057ee33`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0011: Pessimistic lock for enrollment capacity
- Line 7: Context
- Line 32: Alternatives considered
- Line 62: Decision
- Line 91: Consequences
- Line 122: Evidence

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
# ADR 0011: Pessimistic lock for enrollment capacity

- **Status:** Accepted (Day 48: `IGroupRepository.GetForEnrollmentAsync` and `LastSeatConcurrencyTests`);
  final on Day 50, with the HTTP and browser evidence listed under "Evidence"
- **Date:** 2026-10-23 (accepted), 2026-10-24 (final)

## Context

A group has a fixed number of seats (`Group.Capacity`, 1–200), and enrolling a student must never take the
group past it (Day 47; `docs/architecture/academics.md`, "Enrollments"). The decision itself is simple and
lives in the Domain: `Group.EnsureCanEnroll(activeEnrollmentCount)` refuses with `group.full` once the count of
active enrollments reaches the capacity. The difficulty is the count. It is a fact about *other rows* — the
group's active enrollments — so the enroll use case has to read it, decide, and insert, and two requests for the
last seat can interleave between the read and the insert:

```text
Tx A                                   Tx B
COUNT active = 19                      COUNT active = 19
EnsureCanEnroll(19) ok                 EnsureCanEnroll(19) ok
INSERT                                 INSERT
COMMIT                                 COMMIT        -> 21 students in a 20-seat group
```

A transaction alone does not prevent this. Under PostgreSQL's default isolation (READ COMMITTED) each
````

The excerpt stops at line 24; use the original file for the remaining 131 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
