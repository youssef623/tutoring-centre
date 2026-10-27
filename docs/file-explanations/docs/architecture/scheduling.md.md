# `docs/architecture/scheduling.md`

## Purpose

Documentation covering Scheduling: rules, sessions and the session generator (Days 51–54); The time model in one paragraph; The two entities; The generator; The centre time-zone contract. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/scheduling.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 288 lines and 22,019 bytes. Source hash: `513d43d79332e2dfbb24475023decc4f363147022e6002f7b9b353bbc3ff02b8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 267](../../../architecture/scheduling.md#L267): `- **centreTime** (centreTime.ts) is the one place times become text: every function takes the centre's zone`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Scheduling: rules, sessions and the session generator (Days 51–54)

How a group's weekly timetable is modelled, and how it becomes dated sessions with exact instants, including on
the two nights a year when Egypt changes its clocks: the Domain model and generator (Day 51), the tables and
the exclusion constraints that forbid double-booking, the use cases (Day 52), the endpoints and screens (Day 53)
and the demo seed's weekly rules (Day 54). Written from the implementation and its tests; the generator table
below is quoted from `SessionGeneratorTests`, and if the two ever disagree the test is the source of truth.

## The time model in one paragraph

A **rule** is kept in local time; a **session** is kept as an instant. "This group meets every Saturday at 18:00
for 90 minutes" is a statement about the wall clock in the centre's town, and it stays true when the offset
changes in April and October, so the rule stores exactly those local values and nothing else. "The meeting on
Saturday 12 December 2026" is one event that happens at one moment, so the session stores that moment in UTC,
which makes ordering, overlap and "is it on now" unambiguous. The time zone belongs to neither: it is the
centre's (`Centre.TimeZoneId`, Platform module), and the generator is handed it as a parameter.

| What | Where it is kept | Kind of time |
| --- | --- | --- |
| Weekday, start time, duration, effective dates | `ScheduleRule` | local (wall clock in the centre's zone) |
| Start and end of one meeting | `ClassSession.StartsAt`, `ClassSession.EndsAt` | UTC instants |
| The calendar date the rule produced a meeting for | `ClassSession.LocalDate` | local calendar date |
| The centre's time zone | `Centre.TimeZoneId`, read through `ICentreTimeZoneProvider` | IANA zone ID |
| "Today" | computed, `CentreCalendar.Today(clock, zone)` | local calendar date |
```

The excerpt stops at line 24; use the original file for the remaining 264 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
