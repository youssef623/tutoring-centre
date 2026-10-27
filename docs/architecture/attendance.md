# Attendance: marking and corrections (Day 55)

Attendance is a separate Domain module. It identifies the centre, session, student and enrollment by ID;
it does not depend on the Academics domain or store session times. The caller supplies session instants
and the current instant. No attendance rule reads a clock, time zone, database or repository.

## What a record means

There is one attendance record per student per session. A student who has not been marked has no record;
there is no unmarked or unknown status. Enforcing uniqueness and checking enrollment membership belong
to persistence and the use cases planned for Days 56–57, not to an isolated record.

The four recorded statuses are Present, Late, Absent and Excused. Present and Late count as arrivals.
Excused is an affirmative fact, such as a parent notifying the centre. The two sources are Manual and Qr;
QR scanning is planned for Day 63. Neither enum's numeric value or order is part of the contract.

Each record belongs to one centre. Its centre, session, student and enrollment references never change.
It keeps the status, source, time and optional user of the latest ordinary mark, plus correction metadata.

## Taking attendance

The marking window opens **30 minutes before the session starts** and closes **2 hours after it ends**.
Both edges are inclusive. For a session from 16:00 to 17:30 UTC, marking is allowed from 15:30 through
19:30 UTC. Cancellation prevents marking even within that window. A session must end after it starts;
invalid timing is a programming error rather than a business rejection.

Before any correction, marking a different status updates the mark's status, source, time and user.
Marking the same status succeeds without changing any field, including the mark time and source.
The checks run in this order: cancellation, window not open, window closed, previous correction,
then unchanged status. A failed operation leaves the record unchanged.

## Correcting a record

A correction is a separate operation, allowed at or after the session's start, inside or after the
marking window. A cancelled session cannot be corrected. The reason is trimmed and runs of whitespace
are collapsed to a single space; its resulting length must be 3–200 characters. The status must change,
and an accountable user is required.

A successful correction changes the status, increments the correction count, and records its normalized
reason, instant and user. It preserves the source and the ordinary mark's time and user. After the first
correction, ordinary marking is rejected even if the window is still open; further changes need corrections.
Checks run in this order: cancellation, session not started, invalid normalized reason, unchanged status,
then missing accountable user. Every rejection leaves all fields unchanged.

The record keeps only the latest correction metadata. Audit entries capturing before and after values
are planned for Day 56. Repository version comparison and concurrent-save handling are planned for Day 57.

## Permissions and ownership

Owner and Secretary hold `attendance.view`, `attendance.mark` and `attendance.correct`. Teacher holds
`attendance.view` and `attendance.mark`, but cannot correct. The session's teacher is the ownership
boundary planned for Day 57: having a permission does not itself grant access to another teacher's session.

## Implementation boundary

Day 55 provides pure Domain rules, permission keys and English/Arabic frontend messages and labels.
No attendance table, mapping, navigation, repository, endpoint or screen exists yet. See
[ADR 0013](../adr/0013-attendance-window-and-corrections.md) for the draft policy decision and alternatives.
