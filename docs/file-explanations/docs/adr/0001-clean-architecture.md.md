# `docs/adr/0001-clean-architecture.md`

## Purpose

Documentation covering ADR 0001: Clean Architecture; Status; Context; Decision; Alternatives Considered. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0001-clean-architecture.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 4,147 bytes. Source hash: `1c5c5a55302bb44b4b01f22e9d3e75487b14b43d0ed0f2a9f7f7e5597db8112d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0001: Clean Architecture
- Line 3: Status
- Line 7: Context
- Line 11: Decision
- Line 24: Alternatives Considered
- Line 30: Consequences

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0001: Clean Architecture

## Status

Accepted

## Context

This is a portfolio project aimed at .NET roles, so the architecture itself is part of what's being evaluated, not just the features. The same business logic has to serve three different front doors: a staff web app, a WhatsApp assistant, and background jobs (reminders, billing runs) — none of which should duplicate rules like "a payment can't be recorded against a cancelled enrollment." Those rules need to be testable without spinning up a database, both for fast feedback during development and because the AI assistant path depends on the same use cases behaving identically to the web path. The project is expected to grow over roughly eight months, so the structure needs to hold up as features accumulate, not just look clean on day one.

## Decision

Split the codebase into four projects — `TutoringCentre.Domain`, `TutoringCentre.Application`, `TutoringCentre.Infrastructure`, `TutoringCentre.Api` — with references allowed inward only:

- `Domain` references nothing.
- `Application` references `Domain`.
- `Infrastructure` references `Application` and `Domain` (it implements Application's interfaces — Dependency Inversion Principle).
- `Api` references `Application` and `Infrastructure`, and uses the `Infrastructure` reference only in its composition root (`Program.cs`) to register concrete services; that reference must never leak into endpoint code.

Within each layer, organize by feature rather than by technical type: `Application/Payments/Commands/RecordPayment/` holds the command, handler, and validator for that one use case together, instead of scattering them across `Commands/`, `Handlers/`, `Validators/` folders.

Module and layer boundaries are not just a convention — they're enforced by architecture tests running in CI (see Day 2/Day 4), so a reference that violates the allowed direction fails the build, not just a code review.

## Alternatives Considered
```

The excerpt stops at line 24; use the original file for the remaining 17 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
