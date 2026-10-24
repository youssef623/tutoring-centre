# Scheduling: rules, sessions and the session generator (Day 51)

How a group's weekly timetable is modelled, and how it becomes dated sessions with exact instants, including on
the two nights a year when Egypt changes its clocks. Today this is Domain code plus one cross-module contract;
nothing is persisted or exposed yet (tables, constraints and use cases arrive on Day 52, endpoints and screens on
Day 53). Written from the implementation and its tests; the test table below is quoted from
`SessionGeneratorTests`, and if the two ever disagree the test is the source of truth.

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

Nothing stores an offset. An offset is a fact about one instant in one zone, and it is exactly what changes
twice a year; storing it next to a rule would freeze the wrong one into half the year's sessions.

## The two entities

Both live in `TutoringCentre.Domain.Academics.Scheduling`, are tenant-owned (`ITenantOwned`) and are built in the
same style as `Subject` and `Enrollment`: a private constructor, a factory, `Result` values, no clock.

**`ScheduleRule`** holds `GroupId`, `DayOfWeek`, `StartTime` (`TimeOnly`), `DurationMinutes`, `Room` (optional),
`EffectiveFrom` and `EffectiveTo` (optional, `DateOnly`). `CentreId` and `GroupId` never change.

- `Create(centreId, groupId, dayOfWeek, startTime, durationMinutes, room, effectiveFrom)`: the duration is
  `MinDurationMinutes` (30) to `MaxDurationMinutes` (240) in steps of 5 (`schedule.duration_invalid`, field
  `durationMinutes`); the start time is on a 5-minute boundary with no seconds or smaller
  (`schedule.start_time_invalid`, field `startTime`); the room is trimmed, blank becomes null, at most
  `RoomMaxLength` (40) characters (`schedule.room_invalid`, field `room`). Empty IDs give
  `schedule.centre_required` and `schedule.group_required`.
- `End(lastDate)` sets `EffectiveTo` once (`schedule.rule_already_ended` the second time) and never before
  `EffectiveFrom` (`schedule.rule_end_invalid`). Ending on `EffectiveFrom` itself is allowed.
- `OccursOn(date)`: the same weekday, on or after `EffectiveFrom`, and on or before `EffectiveTo` when set.
- There is no update method. To move a group from 18:00 to 17:00, end the old rule and add a new one: the
  sessions the old rule already produced keep saying when they really happened.

**`ClassSession`** holds `GroupId`, `TeacherId`, `ScheduleRuleId`, `LocalDate`, `StartsAt`, `EndsAt`, `Room` and
`Status` (`ClassSessionStatus`: `Scheduled`, `Cancelled`). `(ScheduleRuleId, LocalDate)` is its natural key, which
is what will make generating the same range twice harmless (Day 52 makes that a unique index).

- Its factory, `ClassSession.Schedule`, is `internal`: the only public way to obtain a session is
  `SessionGenerator.Generate`, so no handler can invent one with instants of its own. The Domain project grants
  `InternalsVisibleTo` to `TutoringCentre.Architecture.Tests` only; the Domain tests build every session through
  the generator.
- `TeacherId` is copied from the group when the session is generated, so a past session keeps the teacher who
  taught it even after the group is reassigned. It changes only through `ReassignTeacher(teacherId, now)`, which
  moves a `Scheduled` session that has not started and leaves a started or cancelled one alone (succeeding
  either way, so a caller can pass every session of the group).
- `Cancel(now)` cancels once (`schedule.session_already_cancelled`) and only before the start
  (`schedule.session_in_past` when `StartsAt <= now`). There is no un-cancel.
- No local start or end time is stored beside the instants; local display is a conversion at read time.

`LocalDate` is stored because it is not derivable from `StartsAt` without the zone, and it is not the UTC date
of `StartsAt`: a Sunday 00:30 session in Cairo starts at 22:30 UTC on Saturday. It is the date a person means
when they say "the Sunday class", and the date the natural key is built on.

## The generator

`SessionGenerator.Generate(rule, teacherId, zone, from, to)` returns, in date order, one session for each date
from `from` to `to` (both inclusive) on which `rule.OccursOn(date)`; an empty list when `to` is before `from`.
It is a static domain service: the logic belongs to neither the rule nor a session, and it is pure. Every input is
a parameter; it reads no clock, no repository and no machine time zone, so the same inputs always give the same
dates and instants (only the IDs differ).

For each date it combines the date with the rule's `StartTime` into a wall-clock time and converts that to UTC
in one private method with three visible branches:

1. **Skipped time (spring forward).** The wall-clock time does not exist that night. It moves forward by the
   size of the gap. In Cairo on Friday 30 April 2027, 00:00 jumps to 01:00, so a 00:30 start becomes 01:30 at
   UTC+3: **22:30 UTC on 29 April**, with `LocalDate` 30 April. (Equivalently: the time is read with the offset
   in force before the gap.)
2. **Repeated time (autumn back).** The wall-clock time happens twice. The first occurrence is taken: the one
   with the larger offset. In Cairo on Thursday 28 October 2027, 24:00 goes back to 23:00, so a 23:30 start is
   taken at UTC+3: **20:30 UTC**, not 21:30 UTC an hour later.
3. **Normal time.** The zone's offset on that date: UTC+2 in Cairo's winter, UTC+3 in its summer.

`EndsAt` is `StartsAt` plus the duration as elapsed minutes, never wall-clock arithmetic: a 120-minute session that
starts at 22:30 on 28 October 2027 ends 120 minutes later (21:30 UTC), even though the wall clock reads 23:30 at
the end, only one hour after the start.

Each date's instant is computed from that date's own wall-clock time. Taking the first instant and adding seven
days of hours would put every session after a clock change an hour off: across 30 April, 18:00 on 24 April is
16:00 UTC and 18:00 on 1 May is 15:00 UTC, only 6 days 23 hours later.

What the generator does not decide: "today" (the caller passes the range, from `CentreCalendar.Today`), which
dates already have sessions (the caller filters on rule and `LocalDate`), and overlap with other sessions of the
same teacher or group (exclusion constraints, Day 52).

## The centre time-zone contract

The time zone is Platform data, and Academics may not reach `Centre` or `ICentreReadService`
(`ModuleBoundaryRuleTests`). Platform therefore publishes a second cross-module contract, after Day 43's
`IStaffDirectory`:

- `ICentreTimeZoneProvider` (`TutoringCentre.Application.Centres.Contracts`), one method,
  `GetForCurrentCentreAsync(ct)`, returning a `TimeZoneInfo` and nothing else. The centre is always the current
  actor's, never a parameter.
- `CentreTimeZoneProvider` (`TutoringCentre.Infrastructure.Centres`) reads `platform.centres.time_zone_id` with
  an explicit `centre.Id == centreId` predicate, the centre ID taken from the actor: the tenant table has no EF
  query filter and no row-level security, so nothing else narrows the query. It is scoped and caches the zone in
  a field, so the row is read at most once per request. The ID was validated by `Centre.Create`, so an unknown ID
  is a bug and throws.
- `CentreCalendar.Today(clock, zone)` (`TutoringCentre.Application.Common.Time`, shared kernel) is the centre's
  current calendar date. At 22:30 UTC on 31 December it is already 1 January in Cairo.

## Egypt's rule and the test data

Egypt (`Africa/Cairo`) since 2023: UTC+2 in winter, UTC+3 in summer; clocks go forward at 00:00 on the last
Friday of April and back at 24:00 on the last Thursday of October. For 2027 that is Friday 30 April and Thursday
28 October. The tests hard-code those dates; if the law changes, the tests' dates change and the generator does
not. `SessionGeneratorTests.TimeZoneData_HasEgypts2027Transitions` checks that the time-zone data on the machine
running the tests has both transitions, so a missing or stale database fails with that message first instead of
as a wrong instant. Confirmed on Day 51 on the development machine (Windows, .NET 10); in CI the same test runs
on the Linux image, whose time-zone data `SystemClockTests` already depend on.

## Test cases (`SessionGeneratorTests`)

Every expected instant is a UTC literal worked out by hand, never derived with the calls the generator makes.

| # | Rule and range | Expected |
| --- | --- | --- |
| 1 | Saturday 18:00, 90 min; 5 Dec 2026 to 1 Jan 2027 | four sessions, 5/12/19/26 Dec, each 16:00–17:30 UTC |
| 2 | same rule; range starts Wednesday 2 Dec | first session Saturday 5 Dec |
| 3 | effective 12 Dec, ended 19 Dec; range all of December | only 12 and 19 Dec |
| 4 | Saturday 18:00 over 24 Apr and 1 May 2027 (spring) | 16:00 UTC, then 15:00 UTC; 6 days 23 hours apart |
| 5 | Saturday 18:00 over 23 and 30 Oct 2027 (autumn) | 15:00 UTC, then 16:00 UTC; 7 days 1 hour apart |
| 6 | Friday 00:30, 60 min, on 30 Apr 2027 (skipped) | 22:30–23:30 UTC on 29 Apr; `LocalDate` 30 Apr |
| 7 | Thursday 23:30, 60 min, on 28 Oct 2027 (repeated) | 20:30–21:30 UTC (first occurrence, UTC+3) |
| 8 | Thursday 22:30, 120 min, on 28 Oct 2027 (crosses autumn) | 19:30–21:30 UTC, exactly 120 minutes |
| 9 | Sunday 00:30 on 10 Jan 2027 | starts 22:30 UTC Saturday 9 Jan; `LocalDate` Sunday 10 Jan |
| 10 | `to` before `from` | empty |
| 11 | generated twice over April–November 2027 | equal dates and instants, different IDs |

**Mutation check (Day 51).** With the conversion replaced by a fixed `+02:00`, cases 4, 5, 7 and 8 fail. Case 6
still passes, and that is correct rather than a gap: moving a skipped time forward by the gap gives the same
instant as reading it at the pre-gap offset, which in Cairo is the winter `+02:00`. With a fixed `+03:00` instead,
case 6 fails (as do 1–5 and 9). Together the two mutations show every transition case is pinned by an assertion
that some wrong conversion breaks. The restore left no trace in the diff.

`ScheduleRuleTests` cover every invalid field and the boundaries (25, 30, 240, 245 minutes; 18:02 and 18:00:30;
40 and 41 characters), `OccursOn` around the effective range and both `End` rules. `ClassSessionTests` cover
cancel and reassign on generator-built sessions. `CentreTimeZoneProviderTests` prove the lookup follows the actor
(a Europe/London centre beside the two Cairo ones) and the scope cache; `CentreCalendarTests` the New Year case.

## Permissions

`schedule.view` (Owner, Secretary, Teacher — a teacher only their own groups, by the same ownership scope as
`groups.view` and `enrollments.view`) and `schedule.manage` (Owner, Secretary). Cancelling a session is part of
`schedule.manage`, not a separate key: the same people who manage the timetable cancel its meetings.

See also `docs/adr/0012-sessions-as-generated-instants.md` for the alternatives behind this model.
