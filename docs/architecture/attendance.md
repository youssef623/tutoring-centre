# Attendance: marking, corrections, queries and card scans (Slices 11–12)

Attendance is a separate Domain module. It identifies the centre, session, student and enrollment by ID;
it does not depend on the Academics domain or store session times. The caller supplies session instants
and the current instant. No attendance rule reads a clock, time zone, database or repository.

## What a record means

There is one attendance record per student per session. A student who has not been marked has no record;
there is no unmarked or unknown status. Persistence enforces uniqueness, tenant references and row-level
security. Commands check historical enrollment expectations through the Academics roster contract.

The four recorded statuses are Present, Late, Absent and Excused. Present and Late count as arrivals.
Excused is an affirmative fact, such as a parent notifying the centre. The two sources are Manual and Qr;
Card scans use Qr. Neither enum's numeric value or order is part of the contract.

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

The record keeps only the latest correction metadata. Audit entries retain before and after values.
Corrections compare the caller's version after authorization, before domain rules, and retain the database
version predicate at save time. A stale request returns `409 concurrency.stale`, including a repeated correction.
Concurrent first marks are arbitrated by the unique index and return `409 attendance.concurrent_mark`.

## Permissions and ownership

Owner and Secretary hold `attendance.view`, `attendance.mark` and `attendance.correct`. Teacher holds
`attendance.view` and `attendance.mark`, but cannot correct. The session's teacher is the ownership
boundary: having a permission does not itself grant access to another teacher's session.

History uses one `GetAttendanceQuery` for every actor. Its required `AttendanceAccess` argument is `All`,
`OwnSessions(teacherId)` or `None`. Missing ownership never defaults to `All`. `None` returns an empty page
without a database query. `OwnSessions` filters `academics.class_sessions.teacher_id` in SQL, rather than
the group's current teacher. Student and group filters only narrow that scope; filters outside scope return
an empty page. Month 5 adds a parent access kind here: resolve linked student IDs from the authenticated parent,
then apply those IDs as an additional SQL predicate inside the same read service. It must not treat a
caller-supplied student filter as authority or fall back to All. None remains the fail-closed default.

History orders by `(local_date DESC, starts_at DESC, record_id DESC)` with an opaque URL-safe cursor and
fetches one extra row. Date ranges span at most 92 days; page sizes range from 1 to 100, defaulting to 50.
Marker names come from `identity.users` by user ID; responses never expose marker emails or correction reasons.

## HTTP contract

All five routes belong to the Attendance endpoint group and require a session with a selected centre.
Writes require `X-XSRF-TOKEN`. Unknown JSON properties are rejected at every nesting level.

| Method and route | Operation | Response |
| --- | --- | --- |
| `GET /api/attendance/sessions?date=YYYY-MM-DD` | `listAttendanceSessions` | Date, time zone, sessions with windows and expected/recorded counts |
| `GET /api/sessions/{sessionId}/attendance` | `getSessionAttendance` | Session header, time zone, window, expected students with a record or null |
| `PUT /api/sessions/{sessionId}/attendance` | `markAttendance` | Created, updated and unchanged counts |
| `POST /api/attendance/{recordId}/correct` | `correctAttendance` | 204; requires status, reason and version |
| `GET /api/attendance?studentId=&groupId=&from=&to=&cursor=&pageSize=` | `getAttendance` | Items and nextCursor |

The sessions date defaults to today in the centre's time zone, using `IClock`, and accepts dates within
366 days of today. Instants are UTC. Cancelled sessions remain visible with their status. Rosters use
enrollment intervals at the session's start: late enrollees are absent, while students withdrawn after
the start remain expected. Unknown, other-centre and unowned sessions share `attendance.session_not_found`.
Unknown and other-centre records share `attendance.not_found`. Teacher corrections are forbidden.

The API logs request completion and command outcomes without request bodies. Correction reasons must
never appear in request logs. The generated React Query client includes four marking hooks and an infinite
history hook; generated files are rebuilt from OpenAPI.

## Development demo

Run `dotnet run --project src/TutoringCentre.Api -- demo-session --centre nile-centre --group "<name>"`.
`--in-minutes` defaults to 10. The helper uses a `SystemActor`, the centre's local weekday and a start
rounded up to five minutes, with a 60-minute duration. It retries teacher conflicts at the next half-hour
up to four times, refuses between 23:00 and midnight, and refuses outside Development even if demo mode is enabled.

The existing add-rule command generates its usual horizon and has no end-date input. The helper therefore
dispatches add-rule followed by end-rule for today. Later generated sessions remain cancelled history;
the rule's effective interval is today only. No scheduling behavior changes or direct inserts are needed.
The helper prints the session ID and its attendance URL.

## Implementation boundary

Slice 11 provides Domain rules, persistence, commands, queries, endpoints, isolation tests and generated
hooks, plus session marking/correction screens and history views. Slice 12 adds the scan command on Day 63;
Day 64 exposes the card and scan endpoints, per-user scan limit and HTTP race coverage.
See [ADR 0013](../adr/0013-attendance-window-and-corrections.md) for the accepted decision and evidence.

## History views

The group detail page's fourth tab is `?tab=attendance&from=YYYY-MM-DD&to=YYYY-MM-DD`, rendered only
with `attendance.view`. Its default is today and the preceding 27 calendar days in the active centre's
zone. Invalid or reversed dates and ranges spanning more than 92 days show validation without a history
request. The grid fetches every page, derives students and sessions only from returned records, and
leaves missing cells unrecorded. Withdrawn students remain visible; substitute sessions are absent from
a teacher's view when the server excludes them. No current-roster or contact data is used.

The student card is also gated by `attendance.view`, with no request without that permission. It covers
today and the preceding 29 centre-local calendar days. Load more fetches older records within that range.
“In this period” counts describe loaded records only. Mark and correction success invalidate the infinite
history cache. Teachers still lack `students.view` and cannot reach student detail pages.

## Card scan validation order (Day 63)

`RecordScanCommand(SessionId, Token)` is tenant-scoped and requires `attendance.mark`. The token is
body data only. The command accepts no student ID and masks its token in `ToString()`. Validation touches no data. Actor checks precede the transaction; Day 64 exposes this through the scan endpoint.
Explicit validator error codes survive dispatch; built-in validation failures retain `validation.failed`.

Each check returns before loading the data used by the next check. Test names below are executable
evidence, not suggested future coverage.

| Step | Check | Failure | Reason for position and proving test |
| --- | --- | --- | --- |
| 1 | `CardToken.Parse` accepts token; session ID is non-empty | 400 `card.invalid_format` on `token`; empty ID is field validation | Reject foreign QR payloads without resolving actor or touching data. `DispatcherTests.SendAsync_InvalidScan_PreservesTokenCodeBeforeActorAndTransaction`; `RecordScanValidatorTests.EmptySession_IsRejected`. |
| 2 | Selected tenant and `attendance.mark` permission | 401 `auth.not_authenticated`; 403 `tenant.not_selected` or `auth.permission_denied` | Authorization must precede transaction and session lookup. `RecordScanOrderTests.ActorCheck_PrecedesSessionAndTransaction`. |
| 3 | Session exists in selected centre through roster source | 404 `attendance.session_not_found` | Establish the session before ownership or card data. `RecordScanOrderTests.MissingSession_BeatsDeniedScope`; `SingleFailure_ReturnsContractCode("missing", …)`. |
| 4 | Teaching scope allows session teacher | Same 404 | Prevent teachers testing cards in another teacher's session, including outside its window. `RecordScanOrderTests.TeachingScope_BeatsClosedWindow`; `OtherTeacher_BeatsRevokedCard`; `ScanIsolationTests.OtherTeacherSession_HiddenBeforeCardLookup`. |
| 5 | Not cancelled; marking window open | 422 `attendance.session_cancelled`, `attendance.session_not_open`, `attendance.window_closed` | Outside class the scanner must reveal nothing about cards. `RecordScanOrderTests.Cancellation_BeatsNotOpenWindowAndUnknownCard`; `ClosedWindow_BeatsUnknownCard`; `SingleFailure_ReturnsContractCode` covers each window failure. |
| 6 | One tenant-scoped lookup of `CardToken.Hash()` | 404 `card.not_recognised` | Only an authorized open session may test a token. Unknown and other-centre tokens have identical errors. `RecordScanOrderTests.UnknownCard_BeatsUnexpectedStudent`; `ScanIsolationTests.MaadiCardInNile_IdenticalToUnknown_NoRowsInEitherCentre`. |
| 7 | Card active | 422 `card.revoked` | Only a known card in this centre may disclose replacement status; do not load enrollment first. `RecordScanOrderTests.RevokedCard_BeatsUnexpectedStudent`. |
| 8 | Student expected at session | 422 `attendance.student_not_in_session` | Resolve student and historical enrollment only after card checks; do not read attendance yet. `RecordScanOrderTests.UnexpectedStudent_PreventsRecordRead`; `ScanIsolationTests.OtherGroupStudent_NotExpected_NoRecord`. |
| 9 | Existing attendance and domain arrival rules | 422 `attendance.record_corrected`; concurrent insert: 409 `attendance.concurrent_mark` | Decide the effect last, after all access checks. `RecordScanHandlerTests.CorrectedRecord_OfficeDecisionStands`; `ExistingArrival_PreservesFirstStatusTimeAndSource`; `ConcurrentScanTests.FiveParallelScans_TenRaces_OneRecordedOneRow_RetryIsAlreadyRecorded`. |

Steps 5 and 6 were swapped locally: `ClosedWindow_BeatsUnknownCard` failed with
`card.not_recognised` instead of `attendance.window_closed`. The original order was restored and tests rerun.

### Arrival and late rules

After step 8, use one `IClock` instant for the domain checks and `ScanPolicy.StatusFor`.
Strictly later than session start plus 15 minutes is Late. Exactly 15 minutes, session start,
and earlier eligible arrivals are Present. `RecordScanHandlerTests.NewArrival_UsesScanPolicy`
tests start, +15 minutes and +16 minutes; `ScanPolicyTests` also pins the exact instant boundary.

| Existing record | Scan effect | Evidence |
| --- | --- | --- |
| None | Domain `AttendanceRecord.Create`, source Qr; `Recorded` | `RecordScanHandlerTests.NewArrival_UsesScanPolicy` |
| Uncorrected Present or Late, manual or QR | `AlreadyRecorded`, preserving first status, time, source and user; no mutation | `RecordScanHandlerTests.ExistingArrival_PreservesFirstStatusTimeAndSource`; `RecordScanUseCaseTests.ScanTwice_OneRow_FirstTimeStatusAndVersion_NoAudit` |
| Uncorrected Absent or Excused | Domain `Mark`, arrival status and Qr; `Recorded` | `RecordScanHandlerTests.NonArrival_ScanWins`; `RecordScanUseCaseTests.ManualNonArrival_ScanRecordsQrArrival` |
| Corrected, including a corrected arrival | Domain rejects with `attendance.record_corrected` | `RecordScanHandlerTests.CorrectedRecord_OfficeDecisionStands`; `RecordScanUseCaseTests.CorrectedRecord_ScanCannotOverwriteOrAudit` |

Repeats retain the row's PostgreSQL version and create no audit entry. New scans create no ordinary
mark audit entry. An update from Absent or Excused uses the existing marking audit behavior.
A SystemActor produces a null marking user (`RecordScanHandlerTests.SystemActor_HasNoMarkingUser`).

The card lookup uses `ux_student_cards_centre_token_hash`. A scan changes at most one attendance
row. The attendance natural key arbitrates simultaneous first scans. Five separate scopes are forced
to overlap after reading no attendance, repeated for ten sessions; exactly one records and other
inserts return `attendance.concurrent_mark`. A subsequent retry returns `AlreadyRecorded`.
The HTTP race proves a single retry on conflict; the scanner uses that same retry policy.

The handler emits one structured Information entry with `Outcome`, `SessionId`, and nullable
`CardId`. Before a successful card lookup, CardId is null. Never include token, student name or
command object. `RecordScanHandlerTests.ScanLog_ContainsOnlyOutcomeSessionAndCard` checks success
and unknown-card log fields. Dispatcher logging remains unchanged and never logs command objects.

## Card and scan HTTP endpoints (Day 64)

| Method and route | Operation | Permission | Success |
| --- | --- | --- | --- |
| GET `/api/students/{studentId}/card` | `getStudentCard` | `attendance.cards.manage` | 200 `{card: metadata or null}` |
| POST `/api/students/{studentId}/card` | `issueStudentCard` | `attendance.cards.manage` | 201, Location points to status, no-store |
| POST `/api/groups/{groupId}/cards` | `issueGroupCards` | `attendance.cards.manage` | 200, no-store |
| POST `/api/student-cards/{cardId}/revoke` | `revokeStudentCard` | `attendance.cards.manage` | 204 |
| POST `/api/sessions/{sessionId}/scans` | `recordScan` | `attendance.mark`, teacher owns session | 200 for both outcomes |

Issue has no body. Bulk issue accepts `studentIds`, 1–60 distinct non-empty IDs. Revoke accepts
`reason` (lost, damaged or withdrawn) and `version`; reissued is server-only. Scan accepts only
`token`. All writes retain session and CSRF protection and strict JSON binding. Unknown or foreign
students, groups, cards and sessions share their respective not-found codes; a known student without
a card returns `{card:null}`. The status query now explicitly checks tenant-scoped student existence.

The `scan` sliding-window policy allows 60 requests per minute per authenticated user ID, with six
segments and queue zero. An exhausted user receives 429 `auth.rate_limited` and `Retry-After`;
another user on the same IP retains their own allowance. The policy has no test-only partition bypass.
`Permissions-Policy` is exactly `camera=(self), microphone=(), geolocation=()`.

`CardEndpointTests`, `ScanEndpointTests`, `CardCrossTenantTests`, `ScanCrossTenantTests`, and
`ScanRateLimitTests` prove the contract, no-store headers, secret-free log sink and isolation in both
directions. `PermissionMatrixTests.CardsAndScans` contains 15 literal role cases; endpoint inventory
classifies all five routes. `ScanRaceHttpTests` uses five independent clients over two users, forces
first reads to overlap, retries one `attendance.concurrent_mark`, and proves exactly one recorded
outcome and row in each of five fresh sessions. Dropping the natural unique index in a throwaway
test database exposes duplicate rows; the mutation is restored before normal verification.

## Card printing and scanner (Days 65–66)

Office staff manage the three card states on the student page. Reissue confirms that the current
card stops working. Versioned revoke offers Lost, Damaged and Withdrawn; stale versions refetch
and close, and already-revoked errors refetch status. Archived students cannot be issued cards.
`/groups/$groupId/cards` selects current roster students, sends one bulk request for at most 60,
and prints eight ID-1 cards per A4 page with cut marks. Arabic and English share the card face;
names use automatic direction while the QR and short card number remain left-to-right.

The issue dialog and group sheet hold tokens only in component state and discard them on close
or navigation. Browser-local SVG generation uses error correction M and a quiet zone. No full
token text is shown, printed, persisted or sent to a QR service. Returning requires issuing again.
Card component tests prove storage is untouched, permission guards, confirmation, selection and
versioned revoke; the status cache contains metadata only.

An open roster links to `/attendance/sessions/$sessionId/scan`, guarded by `attendance.mark`.
Session facts still come from the protected roster query; another teacher sees Session not found.
The header shows session/group, teacher, window times and open state. Camera scanning uses the
rear camera and a dynamically imported decoder at roughly ten frames per second. All stream
tracks stop on unmount, hidden tabs, manual mode, closed windows or rate-limit pause. Starting,
scanning, denied, unavailable and paused states are translated; denial offers manual entry.

Both camera and manual/keyboard-wedge entry feed `useScanQueue`. Manual entry has autocomplete
and spellcheck disabled, clears on submit, and validates token shape with Zod before queuing.
Foreign prefixes are ignored. Recent SHA-256 fingerprints suppress the same code for three
seconds without retaining its text. These filters reduce noise; server validation and the natural
key provide the security and idempotency guarantees.

Exactly one request runs at a time; up to five codes wait in memory. Completed credentials are
removed from the active ref; mutation variables contain an empty placeholder, not the token.
Unmount aborts the request and clears pending codes. History retains only the last ten names,
statuses, outcomes and times. A new arrival produces a green banner, brief vibration where
supported and roster-query invalidation. A repeat produces amber with the original status/time.
Banners include icons and text, so colour is not the only signal.

The queue retries `attendance.concurrent_mark` once. Unknown/revoked/unexpected cards produce
translated red reasons. A closed window stops scanning and links back to the roster. A 429
flushes waiting codes and pauses decoding for the parsed `Retry-After` duration; the standard API
error wrapper exposes this metadata without changing other error mapping. No scanner token enters
storage, URL, query cache, mutation variables or result history.

`ScannerPage.test.tsx` mocks the decoder boundary and uses fixed/fake time for repeat filtering.
It proves queue bounds and serialization, retry limits, rate-limit pause/resume, closed windows,
red errors, denied-camera fallback, track cleanup including late permission resolution, manual
validation and secret absence. Existing roster tests remain unchanged and green. The real-browser
`e2e/qr-attendance.spec.ts` issues a disposable card, records via manual entry, repeats, reissues,
rejects the old card and verifies source QR. Physical camera readability and phone latency remain
manual demo checks.

Local phone camera use follows [the README](../../README.md#scanning-from-a-phone): optional
ignored certificates enable Vite HTTPS and `dev:lan` LAN binding. The API remains localhost-only
behind the existing same-origin proxy; Secure cookies, CSRF and production settings are unchanged.
Without both files the existing HTTP/localhost startup is retained. No agent installs a CA or
changes trust stores. [ADR 0014](../adr/0014-opaque-hashed-card-tokens.md) records the full decision.

## Portal attendance and assistant reads (81.1�83.3)

`GET /api/attendance` remains the only attendance history endpoint. Portal scope is resolved on every query; student filters outside that set return the same `attendance.not_found` as nonexistent students. Group filters only narrow the child set. Filtering occurs before keyset pagination. Portal actors cannot read session rosters or mark/correct attendance.

`get_attendance` uses the same query inside the gateway's freshly validated family actor scope. Model arguments contain handles and ISO calendar dates only. Results contain names, local dates/times, status words and group names, never database identifiers. Missing records do not imply absence.

Assistant runs take a transaction-scoped PostgreSQL advisory try-lock keyed by centre and conversation. Only `assistant.conversation_busy` is registered for retry; other business failures retain the existing relay behavior. Deterministic output guards check identifiers, ungrounded attendance, foreign full names and format before persisting outbound text. These checks are not a semantic fact checker. The centre name directory is reloaded for each run; rejected candidate text is never persisted or logged.

Staff with `assistant.view` use the keyset timeline, including safe decision summaries. It uses explicit centre predicates and a batched tool-call read. Other staff retain the messages view.
