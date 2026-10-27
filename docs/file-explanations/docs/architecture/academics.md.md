# `docs/architecture/academics.md`

## Purpose

Documentation covering Academics: students, parents, teachers, groups, enrollments and the schedule (Days 37–54); Module map (end of Month 3); Entities; Why `Parent` is a separate aggregate, not part of `Student`; Why `StudentParentLink` is not a separate aggregate. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/academics.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 490 lines and 36,208 bytes. Source hash: `414cdd5f255e88396bbd507eb32bd1fac6e9aeea0d8c936c09c8e166945c4af9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 30](../../../architecture/academics.md#L30): `a teacher record is linked to it. A bool, nothing more.`
- [Line 117](../../../architecture/academics.md#L117): `SearchText.Normalize(value) is the one function that turns a name — Arabic or Latin, however it was typed —`
- [Line 121](../../../architecture/academics.md#L121): `changes, storing the result as SearchName; Day 40's search query will call the same function on the search`
- [Line 143](../../../architecture/academics.md#L143): `and writes the collection through its private backing field (Navigation(...).UsePropertyAccessMode(`
- [Line 273](../../../architecture/academics.md#L273): `**Group** (academics.groups) is the class students enrol in: a name (1–80 characters, unique per centre by`
- [Line 283](../../../architecture/academics.md#L283): `**The account link.** A teacher record may be linked to one staff account, which is what lets a signed-in`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Academics: students, parents, teachers, groups, enrollments and the schedule (Days 37–54)

The Domain model behind registering a student and the parents a centre contacts about them, how that model is
stored, the use cases and endpoints built on it, and the two screens that use them. Teachers and groups
(Days 43–46) and enrollments (Days 47–50) follow in their own sections at the end; the schedule (Days 51–53) has its
own document, `docs/architecture/scheduling.md`. Written from the implementation; every name and rule below is
checked against the code, not the plan that produced it.

## Module map (end of Month 3)

Academics is one module (`docs/architecture/overview.md`, "Modules"): one Application namespace,
`TutoringCentre.Application.Academics`, and one Domain namespace, `TutoringCentre.Domain.Academics`, with a folder
per feature. Subjects (Month 2) live here too.

| Feature | Application folder | Domain types | Tables (`academics.`) | Permissions |
| --- | --- | --- | --- | --- |
| Students | `Students` | `Student`, `StudentParentLink` | `students`, `student_parents` | `students.view`, `students.manage` |
| Parents | `Parents` | `Parent` | `parents` | `students.view`, `students.manage` |
| Teachers | `Teachers` | `Teacher` | `teachers` | `teachers.view`, `teachers.manage` |
| Groups | `Groups` | `Group` | `groups` | `groups.view`, `groups.manage` |
| Enrollments | `Enrollments` | `Enrollment` | `enrollments` | `enrollments.view`, `enrollments.manage` |
| Schedule | `Scheduling` | `ScheduleRule`, `ClassSession`, `SessionGenerator` (`Domain.Academics.Scheduling`) | `schedule_rules`, `class_sessions` | `schedule.view`, `schedule.manage` |

Inside the module the features reference each other freely, by ID and through their own ports: an enrollment names
```

The excerpt stops at line 24; use the original file for the remaining 466 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
