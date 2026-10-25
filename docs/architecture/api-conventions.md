# API conventions

## Error shape

**Rule.** Every error response is RFC 9457 Problem Details, `application/problem+json`, written by exactly one type: `ProblemResult` (`src/TutoringCentre.Api/Http/ProblemResult.cs`). No other code writes an error body.

Every error carries two extensions beyond the RFC fields:

| Extension | Source | Purpose |
| --- | --- | --- |
| `traceId` | `Activity.Current?.Id ?? HttpContext.TraceIdentifier` | Correlate with distributed tracing. |
| `correlationId` | `HttpContext.Items["CorrelationId"]`, set by `CorrelationIdMiddleware` | Correlate with the client's own logs; `null` only if the middleware did not run. |

Validation failures also carry `errors`: a field name (camelCase) → message list, built from `Error.Fields`.

**Nested field paths (Day 41).** A field inside a nested request object is still one dotted key in `errors`, never its own nested object: `RegisterStudentCommand`'s `Parent` value produces `errors["parent.fullName"]` or `errors["parent.phone"]`, not `errors.parent.fullName`. The dispatcher builds this by camel-casing each dotted segment of the validator's `PropertyName` (`Dispatcher.ToCamelCase`) — a validator that uses `SetValidator` on a nested property, or an `OverridePropertyName("Parent.Phone")` call, gets the nested path for free. A handler-level error that needs the same path for a field FluentValidation never sees (e.g. the nested parent's phone failing `PhoneNumber.Create`, not the validator's own shape check) builds the `Fields` dictionary directly with the same dotted key, by hand (`StudentErrors.ParentPhoneInvalid`).

## Result → HTTP mapping

**Rule.** `ResultHttpExtensions` (`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`) is the single place a domain `Result`/`Error` becomes an HTTP response. Endpoints call `.ToHttpResult(...)` and never branch on `ErrorKind` themselves.

| `ErrorKind` | HTTP status | Example |
| --- | --- | --- |
| `Validation` | 400 | bad input shape |
| `NotFound` | 404 | resource doesn't exist |
| `Conflict` | 409 | duplicate slug |
| `Rule` | 422 | business rule says no |
| `Forbidden` | 403 | caller not allowed |
| `Unauthenticated` | 401 | caller's identity is unknown (Day 14) |

A successful `Result` (non-generic) maps to `204 No Content`; a successful `Result<T>` is handed to the caller-supplied `onSuccess` function (usually `Results.Ok(value)`).

**201 + Location (Day 26).** `ToCreatedHttpResult` is the one place a successful creation becomes `201 Created`. The endpoint supplies a function from the result's value to `(Location, Body)`; both are built only from server-generated values (the created resource's own ID), never from caller-supplied input. Failure still goes through the exact same `Error.ToProblemResult()` the other mapping uses — there is no second status switch to keep in sync.

**Collection responses wrap in `items` (Day 26).** A list endpoint never returns a bare JSON array; the body is `{ "items": [...] }`. This leaves room to add pagination metadata beside `items` later without a breaking shape change.

**Optimistic concurrency via `version` (Day 26).** A mutable resource's read shape includes a `version` (its row's Postgres `xmin`, exposed as an opaque number — never interpreted or compared by the client, just round-tripped). Every state-changing request on that resource — rename, archive, restore, and so on — includes the `version` it read back, and a stale value maps through `ErrorKind.Conflict` to `409` with `code: "concurrency.stale"`, distinct from a `409` duplicate/name conflict by `code` alone (both are `Conflict`-kind; the HTTP status does not distinguish them, the `code` does).

**Action endpoints for state transitions (Day 26).** A transition that is not a plain field update (`archive`, `restore`) is its own `POST /api/{resource}/{id}/{action}` endpoint rather than an overload of `PUT`, and still takes `{ "version" }` to enforce the same concurrency check. A transition attempted from the wrong state (archiving an already-archived resource, restoring one that isn't archived) is a business rule, not a concurrency failure or a validation failure — it maps through `ErrorKind.Rule` to `422`, with a `code` specific to the violated rule (e.g. `subject.already_archived`).

## Exceptions

**Rule.** Exceptions never reach the client as-is. `GlobalExceptionHandler` (`src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs`) is the one place that logs an exception and turns it into Problem Details.

| Exception | Status | `code` | Logged at |
| --- | --- | --- | --- |
| `BadHttpRequestException` (malformed JSON, unknown field, bad route value) | 400 | `request.malformed` | Warning (no exception object) |
| anything else | 500 | `server.unexpected` | Error (with the exception object) |

The 500 body never contains the exception's message, type or stack trace — only the log does. This is why the dispatcher rolls back and rethrows on handler exceptions instead of swallowing them: the handler here is the single place that decides what the client sees.

## Strict JSON

`ConfigureHttpJsonOptions` sets `UnmappedMemberHandling.Disallow` (an unknown field in a request body is a 400, not silently ignored) and a camelCase `JsonStringEnumConverter`. `RouteHandlerOptions.ThrowOnBadRequest = true` makes a malformed body throw `BadHttpRequestException` in every environment, so it always reaches `GlobalExceptionHandler` and comes back as `request.malformed` rather than an empty, unexplained 400.

## Dates, times and instants (Day 53)

Each value says which kind of time it is, and each kind has exactly one shape on the wire:

| Kind | CLR type | Wire shape | Example | Meaning |
|---|---|---|---|---|
| Calendar date | `DateOnly` | `YYYY-MM-DD` | `2027-04-24` | A day in the centre's own calendar (a rule's `effectiveFrom`, a session's `localDate`, the timetable's `weekStart`). No zone. |
| Local time of day | `TimeOnly` | `HH:mm`, 24-hour | `18:00` | A wall-clock time in the centre's zone (a rule's `startTime`). No seconds, no offset. |
| Instant | `DateTimeOffset` | ISO 8601 in UTC with `Z` | `2027-04-24T16:00:00Z` | One moment (a session's `startsAt`/`endsAt`, `enrolledAt`, an audit entry's `occurredAt`). |
| Weekday | `DayOfWeek` | lower-case English name | `saturday` | Like every other enum. |

- Human schedules are sent as **local** values only — a weekday, a time, a duration and dates — and the server converts them in the centre's zone, applying that zone's rules for each date (Egypt's summer time included). A request never carries an offset or a UTC instant for a rule: `"18:00:00"`, `"18:00+02:00"` or `"2027-04-20T00:00:00Z"` where a time or date is expected is a 400 `request.malformed`.
- Moments come back as UTC instants, together with the zone to show them in where a client must render them (`GET /api/timetable` returns `timeZoneId`; every membership on `GET /api/me` carries its centre's `timeZoneId`). Clients format instants in that zone, never the browser's, and group sessions by the server's `localDate`, never by the browser-local date of `startsAt`.
- `HourMinuteTimeOnlyJsonConverter` reads and writes `HH:mm` exactly; `UtcInstantJsonConverter` writes every instant in UTC with `Z` (reading accepts any ISO 8601 offset). `DateOnly` uses the framework's own `yyyy-MM-dd`. Query string dates (`weekStart`) bind from `YYYY-MM-DD`.
- Because a custom converter hides the JSON shape from the OpenAPI schema exporter, a schema transformer in `Program.cs` states it: `TimeOnly` is `{ type: string, format: time }` and `DateTimeOffset` is `{ type: string, format: date-time }`, so the generated client types all three as strings.

## Correlation IDs

`CorrelationIdMiddleware` runs first in the pipeline. A client-supplied `X-Correlation-Id` header is accepted only if it is 1–64 characters of letters, digits, `.`, `_` or `-`; otherwise a new one is generated. Either way, the value is: pushed onto the Serilog `LogContext` (so every log line for the request carries it), stored on `HttpContext.Items` (so `ProblemResult` can put it in error bodies), and written back on the response header in `OnStarting` (so it survives a response that `GlobalExceptionHandler` clears and rewrites).

## Log redaction

`SensitiveDataDestructuringPolicy` is a safety net: when an object is destructured with `{@Object}`, any property named like `Password`, `Token`, `Secret`, `ConnectionString` or `Phone*` is replaced with `***` before it reaches the sink. It does not replace the primary rule from Day 7 — never log request/command objects in the first place — only limits the damage if that rule is broken later.

## Routing

All product HTTP endpoints live under the `/api` group (`app.MapGroup("/api")`). An unmatched `/api/*` route returns Problem Details 404 with `code: "route.not_found"` via `app.MapFallback`, instead of the framework's empty body. Routes outside `/api` (health checks, and the SPA shell's own static files and fallback, Task 34.1) are unaffected.

## `GET /api/system/info`

The first JSON endpoint, added to prove the conventions above end-to-end. Returns `applicationVersion`, `latestMigration`, `databaseUpToDate` and `demoMode` (Task 35.7). Anonymous access is explicit via `.AllowAnonymous()` (see Authentication and authorization below), not an oversight.

## Authentication and authorization (Day 16)

**Rule.** The authorization fallback policy requires an authenticated user. A new endpoint is protected the moment it is mapped, with no attribute needed — forgetting one fails closed (401) instead of silently exposing the endpoint.

Exactly six routes are explicitly anonymous, each with its own `.AllowAnonymous()` (or equivalent) call, never by omission:

| Route | Why |
| --- | --- |
| `GET /health` | Liveness probe; no caller is signed in yet when checking the process is up. |
| `GET /health/ready` | Readiness probe; same reason. |
| `GET /api/system/info` | Public version/migration status, proven anonymous since Day 11. |
| `GET /api/auth/antiforgery` | Bootstraps the CSRF token pair before any session exists. |
| `POST /api/auth/login` | The one way to start a session; can't require being already signed in. |
| `/api/{**path}` fallback | The uniform 404 for an unknown route must not leak whether a route exists behind a login wall. |

A second, pattern-less `app.MapFallback(...)` is also anonymous, for a subtler reason: ASP.NET Core's authorization fallback policy applies not only to mapped endpoints without `.AllowAnonymous()`, but also to requests that match *no* endpoint at all — without a catch-all, an unknown route outside `/api` (a `/does-not-exist`-style probe, or a stale deep link into a since-removed SPA route) would 401 instead of 404. This fallback stays anonymous because the SPA shell it sits behind (Task 34.1) is itself public; auth-gating happens client-side within the app, same as any other static asset. Static files and the SPA fallback (`MapSpaFallback`) both run before this one, so an existing asset or a real client route is served first — this is only what neither of those matched.

Every other endpoint — `/api/auth/logout`, `/api/me`, `/api/session/centre`, and anything added later — relies on the fallback policy rather than its own `.RequireAuthorization()` call. That redundant call was removed where it previously existed: once the default is "authenticated," repeating it per endpoint reads as if it might mean something stricter than the default, when it does not. The Development-only `/openapi/v1.json` document (`app.MapOpenApi()`) is also marked `.AllowAnonymous()` so it stays reachable for local tooling; it never exists outside Development.

## Error codes added in Month 2

`auth.invalid_credentials`, `auth.not_authenticated`, `auth.csrf_invalid`, `auth.rate_limited`,
`route.not_found`, `request.malformed`, `server.unexpected` and `validation.failed` predate Month 2 (Month 1's
authentication and error-handling work; the full authentication set is catalogued in
`docs/architecture/authentication.md`). Every code a Month 2 feature added, by the `ErrorKind` it carries and
the HTTP status that maps to (`ResultHttpExtensions`):

| Code | Kind → Status | Feature (Day) |
| --- | --- | --- |
| `tenant.not_selected` | Forbidden → 403 | Tenant scope, Layer 1 (Day 22) |
| `tenant.no_membership` | Forbidden → 403 | Centre selection (Day 19) |
| `centre.create_forbidden` | Forbidden → 403 | Centre creation restricted to the seed path (Day 20) |
| `centre.slug_taken` | Conflict → 409 | Centre creation |
| `centre.name_required`, `centre.name_too_long`, `centre.name_invalid`, `centre.slug_invalid`, `centre.time_zone_invalid` | Validation → 400 | Centre settings (Day 32) |
| `subject.not_found` | NotFound → 404 | Subjects (Day 23) |
| `subject.name_taken` | Conflict → 409 | Subjects |
| `subject.name_invalid`, `subject.centre_required` | Validation → 400 | Subjects |
| `subject.already_archived`, `subject.not_archived`, `subject.archived_read_only` | Rule → 422 | Subjects |
| `auth.permission_denied` | Forbidden → 403 | Role-based permission step (Day 28) |
| `auth.password_change_required` | Forbidden → 403 | First-login forced password change (Day 29/30) |
| `auth.current_password_invalid` | Rule → 422 | Change password (Day 30) |
| `auth.password_too_weak` | Validation → 400 | Change password (Day 30) |
| `staff.not_found` | NotFound → 404 | Staff management (Day 29) |
| `staff.already_member`, `staff.last_owner`, `staff.cannot_change_self` | Rule → 422 | Staff management |
| `staff.email_invalid` | Validation → 400 | Staff management |
| `membership.centre_required`, `membership.user_required` | Validation → 400 | Staff management |
| `membership.already_active`, `membership.already_inactive`, `membership.inactive`, `membership.role_unchanged` | Rule → 422 | Staff management |
| `concurrency.stale` | Conflict → 409 | Optimistic concurrency, shared by subjects, staff and centre settings (`version`/`xmin`) |

Audit (Day 31) adds no error codes of its own: it is read-only, with no command and therefore nothing for a
business rule to refuse.

## Error codes added in Month 3

| Code | Kind → Status | Feature (Day) |
| --- | --- | --- |
| `parent.not_found` | NotFound → 404 | Parents (Day 39); also returned when a student command's nested parent reference does not exist or belongs to another centre |
| `parent.phone_taken` | Conflict → 409 | Parents; `errors["parent.phone"]` when raised from `RegisterStudentCommand`'s nested new-parent form |
| `phone.invalid` | Validation → 400 | Parents/Students — `PhoneNumber.Create`'s own validation, re-keyed by the caller to `errors.phone` (a resource's own phone) or `errors["parent.phone"]` (a nested parent's) |
| `student.not_found` | NotFound → 404 | Students (Day 40) |
| `student.name_invalid`, `student.school_year_invalid`, `student.centre_required`, `student.parent_required` | Validation → 400 | Students |
| `student.already_archived`, `student.not_archived`, `student.archived_read_only` | Rule → 422 | Students |
| `student.parent_already_linked`, `student.too_many_parents`, `student.parent_not_linked`, `student.last_parent`, `student.primary_parent_required`, `student.already_primary` | Rule → 422, except `student.parent_not_linked` which is NotFound → 404 (unlink/make-primary addressing a parent id that is not, or no longer, linked) | Students |
| `concurrency.stale` | Conflict → 409 | Shared with Month 2; now also students (`version`/`xmin` on the student row, which guards its parent links too) |
| `schedule.duration_invalid`, `schedule.start_time_invalid`, `schedule.room_invalid` | Validation → 400 on `errors.durationMinutes` / `errors.startTime` / `errors.room` | Schedules (Days 52–53); a duration outside 30–240 or a room over 40 characters is caught first by the validator, as `validation.failed` on the same field |
| `schedule.rule_not_found`, `schedule.session_not_found` | NotFound → 404 | Schedules; the one answer for an unknown ID and another centre's |
| `schedule.teacher_double_booked`, `schedule.group_double_booked` | Conflict → 409 | Schedules; the two exclusion constraints, translated. Also `PUT /api/groups/{id}/teacher` when the new teacher is busy at one of the group's future sessions |
| `schedule.range_invalid` | Rule → 422 on the named date field | Schedules; `effectiveFrom` more than 366 days ahead, or `through` outside today to 92 days ahead (today in the centre's zone) |
| `schedule.rule_already_ended`, `schedule.rule_end_invalid`, `schedule.session_already_cancelled`, `schedule.session_in_past` | Rule → 422 | Schedules |
| `group.has_active_schedule` | Rule → 422 | `POST /api/groups/{id}/archive` while a rule is still in effect (Day 52) |
