# ADR 0013: Attendance window and corrections

- **Status:** Accepted (Day 60)
- **Date:** 2026-10-28

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
- Teacher can view and mark only sessions taught by that teacher. Owner and Secretary can also correct.
  After the marking window closes, office staff correct an existing record with a reason; teachers ask
  office staff to do so. No background task invents an absent record for an unmarked student.
- Corrections and their before/after audit entry commit in one transaction. A client version and the
  database version predicate reject stale corrections. Concurrent first marks use the unique constraint.
- History uses one query with explicit All, OwnSessions or None access. SQL filters by the session's
  teacher, preserving substitute and historical ownership rather than inferring it from today's group.
- The UI shows only returned history. Group grids fetch every page in a range; student counts describe
  the loaded records in the displayed period, never lifetime totals.

## Evidence

- `AttendanceWindowTests.StateAt_LiteralBoundary_ReturnsExpected` proves inclusive opening and closing
  edges; changing the closing allowance to one hour fails the 19:30 UTC boundary case.
- `AttendanceRecordTests.Mark_SameStatus_LeavesEveryFieldUnchanged` proves idempotent ordinary marking.
- `AttendanceRecordTests.Correct_InsideWindowAndNextDay_UpdatesCorrectionFieldsOnly` proves late
  corrections retain ordinary mark metadata.
- `AttendanceRecordTests.Mark_CorrectedRecord_ChecksWindowBeforeLockAndStatus` proves a corrected record
  cannot be changed through ordinary marking.
- `AttendanceQueryTests.History_MapsScope` proves the access mapping for one history query.
- `AttendanceReadServiceTests.History_AccessCountsMarkerAndSubstituteScope` and
  `History_LogsSessionTeacherPredicate_AndNoneExecutesNoQuery` prove ownership in SQL and fail-closed None.
- `AttendanceRecordPersistenceTests.TwoContextsUpdatingOneRecord_SecondGetsStaleConflict` proves the
  database concurrency predicate; `DuplicateFirstMark_TranslatesToConcurrentMark_AndKeepsOneRow` proves
  unique first-mark arbitration.
- `GetAuditLogValidatorTests.Validate_WithKnownEntityType_Succeeds` includes `attendance_record`, so
  the published attendance audit filter is accepted by the server.
- `AttendanceHistory.test.tsx` proves multi-page assembly, URL range validation, empty cells,
  permission gating, scoped counts and Arabic direction.
- `e2e/attendance.spec.ts` proves teacher marking, secretary correction, owner audit visibility and
  refusal of a pasted URL for another teacher's session through the browser.
