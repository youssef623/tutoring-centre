# `docs/adr/0012-sessions-as-generated-instants.md`

## Purpose

Documentation covering ADR 0012: Sessions as generated instants; Context; Alternatives considered; Decision; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0012-sessions-as-generated-instants.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 135 lines and 9,475 bytes. Source hash: `72d83976f7400d63d272bf1b3a13ed3f50cbf3067ed7ff4c986fb605c66055ba`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0012: Sessions as generated instants
- Line 7: Context
- Line 19: Alternatives considered
- Line 73: Decision
- Line 83: Consequences
- Line 101: Evidence
- Line 110: Evidence (Days 52–54)

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0012: Sessions as generated instants

- **Status:** Accepted. Drafted on Day 51 with the Domain model, generator and tests; final on Day 54, once
  sessions were persisted (Day 52) and exposed (Day 53), with the evidence listed under "Evidence (Days 52–54)"
- **Date:** 2026-10-24 (draft), 2026-10-26 (final)

## Context

A group meets on a weekly pattern ("every Saturday at 18:00 for 90 minutes"), and Month 4's attendance needs a
concrete thing to attach to: "the meeting on 12 December". Two questions follow. First, are those meetings rows
in the database, or computed from the pattern whenever someone looks? Second, in which kind of time is each piece
kept — local wall-clock time, UTC, or a local time with an offset?

The second question is sharpened by where the centres are. Egypt (`Africa/Cairo`) observes daylight saving
again since 2023: UTC+2 in winter, UTC+3 in summer, forward at 00:00 on the last Friday of April, back at 24:00 on
the last Thursday of October. A weekly time therefore maps to a different UTC instant in summer than in winter,
one local hour a year does not exist, and another happens twice.

## Alternatives considered

**Where occurrences live**

- **Compute occurrences at read time** (store only the rule; expand it into dates whenever a timetable is
  shown). Nothing to keep in sync, and a rule change is visible everywhere at once. But there is nothing for
```

The excerpt stops at line 24; use the original file for the remaining 111 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
