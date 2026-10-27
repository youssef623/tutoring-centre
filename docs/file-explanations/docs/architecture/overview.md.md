# `docs/architecture/overview.md`

## Purpose

Documentation covering Architecture overview; Failures: results vs exceptions; Request pipeline (end of Month 2); Request path: `CreateCentreCommand` (CLI → table); Request path: `GET /api/system/info` (HTTP → table). Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/overview.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 202 lines and 15,762 bytes. Source hash: `da5b375e9905740f5ef9eee41513b2c1a262b2e31d713866e6a87eee352c82c7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 154](../../../architecture/overview.md#L154): `allowed) or an explicit published interface in that module's own Contracts namespace (Task 43.3, e.g.`
- [Line 160](../../../architecture/overview.md#L160): `how it is answered: every type in one must be an interface or a record, and it may not itself depend on its`
- [Line 170](../../../architecture/overview.md#L170): `| TutoringCentre.Application.Identity.Contracts.IStaffDirectory (Day 43) | Identity | IsActiveMemberWithRoleAsync(centreId, userId, role): is this user an active member of this centre with exactly this role? A bool. | linking a teacher record to a staff account |`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Architecture overview

## Failures: results vs exceptions

**Rule.** Expected business outcomes are `Result` failures returned as values. Bugs and infrastructure faults are exceptions, handled once by a global handler.

| Situation | Mechanism | Example |
| --- | --- | --- |
| Invalid input | `Result` failure, kind `Validation` | slug `"Bad Slug"` → `centre.slug_invalid` |
| Something requested doesn't exist | `NotFound` | unknown centre (from Week 2) |
| Conflicts with existing data | `Conflict` | duplicate slug (Day 9) |
| A business rule says no | `Rule` | enrolling into a full group (`group.full`) |
| The caller isn't allowed | `Forbidden` | non-system actor creating a centre (Week 2) |
| A bug | exception | reading `Value` of a failed `Result` |
| An infrastructure fault | exception | database unreachable, timeout |

**Kinds.** `Validation`, `NotFound`, `Conflict`, `Rule`, `Forbidden` — a closed set. The Api maps each kind to exactly one HTTP status (Day 11); adding a kind is an API-contract change.

**Codes.** `<feature>.<reason>`: lowercase feature, snake_case reason — `centre.slug_invalid`, `centre.name_too_long`. Codes are a public contract that the frontend translates; renaming one is a breaking change.

**Messages** are for developers and logs. Users see translated text chosen by the code.

**Safety.** Codes and messages never contain internals: no stack traces, SQL, file paths or secrets.

```

The excerpt stops at line 24; use the original file for the remaining 178 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
