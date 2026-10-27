# `docs/adr/0010-student-aggregate-and-value-objects.md`

## Purpose

Documentation covering ADR 0010: Student aggregate and value objects; Context; Alternatives considered; For the value objects (`PhoneNumber`, `SearchText`); For the aggregate boundary (`Student`, `Parent`, `StudentParentLink`). Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0010-student-aggregate-and-value-objects.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 94 lines and 7,267 bytes. Source hash: `d61c650f4cbe61b4288ec32bfe44eccc3c5c08287d56f4ff334dbc8508f6223a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0010: Student aggregate and value objects
- Line 6: Context
- Line 15: Alternatives considered
- Line 17: For the value objects (`PhoneNumber`, `SearchText`)
- Line 42: For the aggregate boundary (`Student`, `Parent`, `StudentParentLink`)
- Line 67: Decision
- Line 76: Consequences
- Line 88: Outcome (Day 54)

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0010: Student aggregate and value objects

- **Status:** Accepted
- **Date:** 2026-10-12

## Context

Month 3 needs to represent a student, the parents a centre contacts about them, and the link between the
two, before any of it is persisted or exposed (Day 37; storage and endpoints follow Days 38–41). Two separate
design questions came up while building this: how to validate a phone number and a search-friendly name
without scattering the rules that do it, and how to draw the aggregate boundary between a student and the
parents linked to them. See `docs/architecture/academics.md` for the resulting model and the invariant it
protects.

## Alternatives considered

### For the value objects (`PhoneNumber`, `SearchText`)

- **Validating strings in every validator** (each command's FluentValidation rules re-check the phone format
  and each handler re-normalizes a name for search) keeps the Domain free of the concept entirely, but means
  the same regex and the same Arabic letter-folding table exist whichever handler happens to touch a phone or
  a name, with no single place to fix or test them. A rule changed in one validator and missed in another
  would compile and pass unrelated tests, then fail only in production, on exactly the input the missed
  validator should have caught.
```

The excerpt stops at line 24; use the original file for the remaining 70 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
