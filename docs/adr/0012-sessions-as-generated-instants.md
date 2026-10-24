# ADR 0012: Sessions as generated instants

- **Status:** Draft (Day 51: Domain model, generator and tests); to be finalised on Day 54 once sessions are
  persisted (Day 52) and exposed (Day 53)
- **Date:** 2026-10-24

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
  attendance, a cancellation or a one-off teacher change to belong to; each would need its own "exception" table
  keyed by rule and date, which is a stored session under another name. Overlap between two groups' meetings
  could not be a database constraint, because the rows it would constrain do not exist. And a rule edit would
  silently rewrite the past.
- **Generate and store sessions** (this decision). A session is a row with its own instants, status and teacher.
  Attendance, cancellation and overlap constraints attach to it directly, and the past stays as it happened.
  The cost is generation: a range must be generated (and later extended), and generation must be repeatable,
  which the natural key `(ScheduleRuleId, LocalDate)` provides.

**Which kind of time the rule keeps**

- **A UTC rule** ("every Saturday at 16:00 UTC"). Simple to expand, but wrong for half the year: the group would
  meet at 18:00 in winter and 19:00 in summer. Correcting it means editing every rule twice a year.
- **A local time with a fixed offset** ("18:00+02:00"). The same error, written down more precisely: the offset
  is a fact about one instant, and it is exactly what changes.
- **A local rule with no offset or zone** (this decision): weekday, `TimeOnly` start, duration in minutes,
  effective dates. The zone is the centre's and is passed in when sessions are generated. 18:00 stays 18:00
  across both changes.

**Which kind of time the session keeps**

- **Local date and time only.** Readable, but "is it on now", ordering across zones and overlap all need the zone
  at every query, and an hour in October is ambiguous.
- **UTC instants plus the local date** (this decision). `StartsAt` and `EndsAt` are UTC; `LocalDate` is the
  calendar date the rule produced the session for, kept because it is not the UTC date of `StartsAt` (a 00:30
  Cairo session starts the previous day in UTC) and because it is the natural key.

**The skipped hour (spring)**

- **Reject the rule** for a start time that falls in a gap. The gap is one night a year and depends on the zone;
  rejecting "Friday 00:30" all year for one night is wrong, and rejecting only that night's session leaves a hole
  no one asked for.
- **Skip that night's session.** Silently drops a meeting.
- **Move back** (to before the gap, 23:30 at UTC+2). Moves the meeting into the previous calendar day.
- **Move forward by the size of the gap** (this decision): 00:30 on 30 April 2027 becomes 01:30 at UTC+3,
  22:30 UTC on 29 April, with `LocalDate` still 30 April. The meeting happens, on its own date, as soon as the
  clock allows. It is also iCalendar's rule (RFC 5545, section 3.3.5: a nonexistent local time is read with the
  offset in force before the gap, which gives the same instant).

**The repeated hour (autumn)**

- **The second occurrence** (the later instant, at the new offset). Defensible, but surprising: a 23:30
  meeting would start an hour after people's watches first showed 23:30.
- **Reject or flag as ambiguous.** Again punishes the rule all year for one night.
- **The first occurrence** (this decision): 23:30 on 28 October 2027 is taken at UTC+3, 20:30 UTC. It is the
  earlier instant, so no one arrives to find the meeting is still an hour away, and it is iCalendar's rule too
  (RFC 5545, section 3.3.5: an ambiguous local time refers to its first occurrence).

## Decision

`ScheduleRule` stores local values only. `ClassSession` stores UTC `StartsAt` and `EndsAt` and the local
`LocalDate`, and is created only by `SessionGenerator.Generate(rule, teacherId, zone, from, to)`, a pure domain
service that computes each date's instant from that date's own wall-clock time (never the first instant plus
seven days). The conversion has three branches in one private method: a normal time uses the zone's offset on that
date; a skipped time moves forward by the gap; a repeated time takes its first occurrence. `EndsAt` is `StartsAt`
plus the duration in elapsed minutes. The zone comes from the centre through `ICentreTimeZoneProvider`, and the
generator receives it as a parameter, so it reads no clock and no machine time zone.

## Consequences

Sessions are truthful history: ending a rule and adding a new one changes future generation without rewriting
past rows, and a past session keeps its teacher. Overlap between sessions becomes a database concern on stored
rows (Day 52's exclusion constraints), not something recomputed on every read.

Generation must be run, and re-running it must be harmless; the `(ScheduleRuleId, LocalDate)` key makes the second
run a no-op once the handler filters dates that already have sessions (Day 52).

The transition rules are a policy of this system, not of the base library, and live in one method. They depend
on the time-zone database of the machine running them: if Egypt's law changes, the operating system's data must
be updated and the tests' hard-coded 2027 dates change, while the generator does not. A test
(`SessionGeneratorTests.TimeZoneData_HasEgypts2027Transitions`) fails first when the data is missing or stale.

A session generated before a change in the law keeps the instant computed under the old rule. Regenerating
future sessions after such a change is a deliberate operation (out of scope today), not something that happens
on read.

## Evidence

`tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs` pins the rules with hard-coded
UTC instants. The two transition rules in this ADR are cases 6 (skipped: 22:30 UTC on 29 April 2027) and 7
(repeated: 20:30 UTC on 28 October 2027); case 8 pins elapsed-time duration across the autumn change, and cases 4
and 5 pin that each date gets its own offset. Replacing the conversion with a fixed `+02:00` fails cases 4, 5, 7
and 8; a fixed `+03:00` fails case 6 (the skipped-time rule coincides with the winter offset, so only the summer
offset can expose it). The full table is in `docs/architecture/scheduling.md`.
