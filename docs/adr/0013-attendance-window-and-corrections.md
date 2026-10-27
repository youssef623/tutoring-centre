# ADR 0013: Attendance window and corrections

- **Status:** Draft (Day 55); final review planned for Day 60
- **Date:** 2026-10-27

## Context

Taking attendance during class and correcting an old attendance record serve different purposes.
Routine marking must tolerate repeat requests without moving the mark time. A later change needs an
accountable user and an explanation, while retaining who made the ordinary mark and when.

The rule must agree across manual marking, roster queries and future QR scanning. Session instants
provide a common basis without time-zone conversions or clock reads inside the Domain.

## Alternatives considered

- **Free editing with an audit trail.** Simple for callers, but gives ordinary marking the same power as
  historical correction. An audit trail explains what changed without requiring a reason before it changes.
- **An append-only event per change.** Preserves every transition directly, but requires event folding and
  a more involved read model for a small manual attendance slice. Dedicated audit entries can retain history
  without making every attendance read reconstruct it.
- **Lock after the window with no correction path.** Easy to enforce, but prevents staff from correcting
  an honest mistake discovered the next day.
- **A marking window plus explicit correction (this decision).** Keeps routine marking simple and gives
  historical changes their own reason, user and audit semantics.

## Decision

Allow ordinary marking from **30 minutes before session start through 2 hours after session end**, with
both edges inclusive. Cancellation is checked by the caller of the window policy. The window itself
compares only the supplied instants.

Allow corrections once the session has started, with no upper time limit. Reject cancelled sessions,
invalid normalized reasons and unchanged statuses. A correction requires an accountable user, updates
correction metadata and preserves the ordinary mark's source, time and user. Once corrected, a record
can only change through another correction.

Represent not recorded as no record. Use one record per student per session, with centre ownership and
immutable references to its session, student and enrollment. Keep Attendance independent of Academics.

## Consequences

- `AttendanceWindow.OpensBefore` is 30 minutes and `ClosesAfter` is 2 hours. These are policy constants
  in code, not per-centre settings. A policy change needs a reviewed code and boundary-test change.
- Same-status marking is a successful no-op. Retries do not rewrite the mark time or source.
- Teacher can view and mark; Owner and Secretary can also correct. Ownership checks for the session's
  teacher are planned for Day 57.
- Correction metadata on the record is not the complete history. Before/after audit entries are planned
  for Day 56; optimistic concurrency is planned for Day 57.
- The draft remains open until persistence, endpoints and screens supply evidence for the Day 60 review.

## Evidence (Day 55)

Literal UTC boundary tests cover 15:29:59, 15:30, 19:30 and 19:30:01 for a session from 16:00 to 17:30.
Changing the closing allowance to 1 hour makes the 19:30 test fail. Record tests cover check precedence,
unchanged fields on rejection, same-status marking, repeated corrections and preservation of mark metadata.
Giving Teacher the correction permission makes exactly one of the 57 literal permission cells fail.
