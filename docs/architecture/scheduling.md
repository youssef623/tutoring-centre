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
is what makes generating the same range twice harmless (the unique index `ux_class_sessions_rule_date`).

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
same teacher or group (the exclusion constraints below).

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

## Storage (Day 52)

Migration `AddSchedule` (`20261024105240`), the fifth and last Month 3 migration. It first runs
`create extension if not exists btree_gist`, which the exclusion constraints need for `=` on `uuid` inside a GiST
index. Both tables are tenant-owned (`ConfigureTenantOwned`: `centre_id`, `fk_<table>_centres`,
`ux_<table>_centre_id_id`), carry forced row-level security, and grant `tutoring_app` `SELECT`, `INSERT` and
`UPDATE` but never `DELETE`: rules are ended and sessions cancelled, never removed. Both have `created_at`,
`updated_at` and `xmin` as the row version.

**`academics.schedule_rules`**: `group_id`, `day_of_week` (`smallint`, 0 = Sunday as in .NET), `start_time`
(`time`), `duration_minutes`, `room` (`varchar(40)`, nullable), `effective_from` and `effective_to` (`date`, the
second nullable).

- `fk_schedule_rules_groups`: `(centre_id, group_id)` to `groups(centre_id, id)`.
- `ck_schedule_rules_day` (0–6) and `ck_schedule_rules_duration` (30–240); index `ix_schedule_rules_centre_group`.

**`academics.class_sessions`**: `group_id`, `teacher_id`, `schedule_rule_id`, `local_date`, `starts_at` and
`ends_at` (`timestamptz`), `room`, `status` (`varchar(10)`).

- Composite keys to the same centre's group, teacher and rule (`fk_class_sessions_groups`,
  `fk_class_sessions_teachers`, `fk_class_sessions_rules`).
- `ux_class_sessions_rule_date` (unique on `schedule_rule_id, local_date`): the natural key.
- `ck_class_sessions_status` (`scheduled`, `cancelled`) and `ck_class_sessions_range` (`ends_at > starts_at`).
- `ex_class_sessions_group_overlap` and `ex_class_sessions_teacher_overlap`, each
  `exclude using gist (centre_id with =, <group_id or teacher_id> with =, tstzrange(starts_at, ends_at, '[)') with &&) where (status = 'scheduled')`.
  Half-open, so a session ending at 19:00 and one starting at 19:00 do not collide; partial, so a cancelled
  session frees its slot; `centre_id` first, so one centre's sessions never meet another's
  (`docs/architecture/tenancy.md`). The group constraint is created first: PostgreSQL reports the first violated
  index in creation order, and "this group already meets then" is the more precise answer when both apply.
- Indexes for the reads: `ix_class_sessions_centre_date` (`centre_id, local_date, starts_at`, the timetable),
  `ix_class_sessions_centre_group_date` and `ix_class_sessions_centre_teacher`.

`ConstraintErrorCatalog` (Infrastructure) translates a violation of the two exclusion constraints into
`409 schedule.group_double_booked` ("This group already has a session at this time.") and
`409 schedule.teacher_double_booked` ("The teacher already has a session at this time."). That translation is the
only overlap check in the system: no handler queries for overlaps, so there is no read-then-insert window to race.

## Use cases (Days 52–53)

| Request | Permission | What it does |
| --- | --- | --- |
| `AddScheduleRuleCommand(groupId, dayOfWeek, startTime, durationMinutes, room, effectiveFrom)` | `schedule.manage` | Creates the rule and generates its sessions from the later of today and `effectiveFrom` through the horizon, with the group's current teacher. `effectiveFrom` at most 366 days ahead (`schedule.range_invalid`); an archived group answers `group.archived`. |
| `EndScheduleRuleCommand(scheduleRuleId, lastDate, version)` | `schedule.manage` | Sets `EffectiveTo` and, in the same transaction, cancels the rule's later sessions that are still scheduled and have not started. Earlier sessions are untouched. |
| `ExtendScheduleCommand(through)` | `schedule.manage` | For every rule in effect on an active group, generates the missing sessions from today through `through` (today to today + 92 days). Idempotent: dates that already have a session of the rule, scheduled or cancelled, are skipped, so a cancelled date is never refilled. One transaction for the centre. |
| `CancelSessionCommand(sessionId, version)` | `schedule.manage` | Cancels one session before it starts (`schedule.session_in_past` once it has). The rule keeps going; the slot is free again. |
| `ListGroupScheduleRulesQuery(groupId)` | `schedule.view` | The group's rules, ended ones included. Scoped on the group's teacher. |
| `GetTimetableQuery(weekStart, groupId?, teacherId?)` | `schedule.view` | Seven local dates from `weekStart`: every session, cancelled ones included, ordered by start, with the centre's IANA zone beside the UTC instants. Scoped on each session's own teacher. |

Two existing group use cases changed with the schedule (Day 52). `AssignGroupTeacherCommand` moves the group's
future scheduled sessions to the new teacher (`ClassSession.ReassignTeacher`), and the exclusion constraint refuses
the change when the new teacher is busy then (`409 schedule.teacher_double_booked`; the group keeps its current
teacher). `ArchiveGroupCommand` refuses while a rule is still in effect (`group.has_active_schedule`).

**Serialising writes per teacher (Day 53).** Two colliding writes for one teacher in the same instant could end in
a PostgreSQL deadlock (`40P01`) instead of the exclusion violation, because each inserts into the GiST index before
checking it. `IClassSessionRepository.LockTeacherSchedulesAsync` takes sorted, transaction-scoped advisory locks for
every teacher a command may give sessions to, before any session changes; `AddScheduleRule`, `ExtendSchedule` and
`AssignGroupTeacher` call it. The second writer then waits and is refused by the constraint with a 409
(`ScheduleDoubleBookingHttpTests`).

Every change is audited: a rule on every action, a session only when it is updated (a cancellation or a teacher
reassignment; `docs/architecture/audit.md`).

## The horizon

`ScheduleHorizon` (Application) holds three limits, all in calendar days of the centre's zone:

- `Days = 28`: adding a rule generates today through today + 27, four weeks, which holds every weekday exactly
  four times.
- `MaxEffectiveFromDaysAhead = 366`: how far ahead a new rule may start.
- `MaxExtendDaysAhead = 92`: how far `ExtendScheduleCommand` may reach.

"Today" is always `CentreCalendar.Today(clock, zone)`, never the server's date. Nothing extends the schedule on
its own yet: after four weeks the timetable runs out unless someone presses "Extend schedule" on the Timetable
page (`later.md`).

## What Month 4's job will call

The nightly job planned for Month 4 needs no new use case. For each centre it dispatches
`ExtendScheduleCommand(through: today + 28)` as `SystemActor(centreId)`, the same way the seed CLI dispatches its
per-centre commands. The command is already one centre per call, idempotent, all-or-nothing per centre, and takes
the per-teacher locks, so it can run beside staff edits. Iterating every centre is the part that does not exist
yet (`later.md`, "tenant-less background jobs").

## Endpoints (Day 53)

| Method | Route | Request | Success |
| --- | --- | --- | --- |
| `GET` | `/api/groups/{groupId}/schedule-rules` | `ListGroupScheduleRulesQuery` | 200 |
| `POST` | `/api/groups/{groupId}/schedule-rules` | `AddScheduleRuleCommand` | 201, `{ id, sessionsCreated }` |
| `POST` | `/api/schedule-rules/{id}/end` | `EndScheduleRuleCommand` | 204 |
| `POST` | `/api/schedule/extend` | `ExtendScheduleCommand` | 200, `{ sessionsCreated }` |
| `GET` | `/api/timetable?weekStart=&groupId=&teacherId=` | `GetTimetableQuery` | 200, `{ timeZoneId, weekStart, sessions }` |
| `POST` | `/api/sessions/{id}/cancel` | `CancelSessionCommand` | 204 |

The endpoints only bind and dispatch. Weekdays travel as lower-case names (`"saturday"`), local times as `"HH:mm"`,
dates as `"YYYY-MM-DD"`, instants as UTC. The server never formats a time for display.

## Frontend (Day 53)

Under `frontend/src/features/schedule`, all text in the `schedule` namespace in both languages:

- **Schedule tab** on the group page (`GroupScheduleTab`, `ScheduleRulesTable`), shown with `schedule.view`; the
  "Add weekly session" and "End" actions only with `schedule.manage`. `AddScheduleRuleDialog` uses a 12-hour,
  three-select time picker (hour, minute, AM or PM) that reads left to right in both languages, a length select
  and a first date that defaults to the centre's today; a 409 is shown in the dialog in words and leaves it open.
- **Timetable** (`/timetable`: `TimetablePage`, `TimetableGrid`, `WeekNavigator`, `SessionCard`): a week that
  starts on Saturday, seven columns from the `lg` breakpoint and a stacked list below it, `?week=` in the URL, group
  and teacher filters (the teacher one only with `teachers.view`), cancelled sessions kept in place and labelled.
  "Extend schedule" and each session's "Cancel session" only with `schedule.manage`.
- **`centreTime`** (`centreTime.ts`) is the one place times become text: every function takes the centre's zone
  from the response, never the browser's, so a laptop set to another zone still shows Cairo's wall clock.

## The demo schedule (Day 54)

The seed command adds eight weekly rules through `AddScheduleRuleCommand`, as `SystemActor(centreId)`, one scope
per write: six in Nile across its four groups and two in Maadi, Monday to Thursday only (the API tests and the
journeys schedule their own sessions on Saturdays and Sundays, some on the same groups). `EffectiveFrom` is the
seed day in the centre's zone, never a calendar date in code, so a freshly seeded instance always shows this week
and the next three. A rule is skipped when its group already has one with the same weekday and start time (read
through `ListGroupScheduleRulesQuery`), so a second run adds nothing. A `schedule.*_double_booked` answer is never
treated as "already seeded": the seed's rules are chosen not to collide, so one means the seed's own data (or data
left in the database by hand) is wrong, and the command exits with code 1. `SeedCommandTests` asserts 6 and 2
rules, 24 and 8 sessions, every rule starting on the seed day, and that a second run changes no count anywhere.

## Permissions

`schedule.view` (Owner, Secretary, Teacher — a teacher only their own groups, by the same ownership scope as
`groups.view` and `enrollments.view`) and `schedule.manage` (Owner, Secretary). Cancelling a session is part of
`schedule.manage`, not a separate key: the same people who manage the timetable cancel its meetings.

See also `docs/adr/0012-sessions-as-generated-instants.md` for the alternatives behind this model.
