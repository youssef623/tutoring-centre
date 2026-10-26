# Month 3 local verification checklist (Day 54)

The end-of-Month-3 checks, walked on a freshly seeded **local** instance. There is no deployed smoke test: no cloud
deployment exists (`docs/architecture/deployment.md`), and the Month 2 smoke-test script and runbook are not used
here. A smoke test asks "is it up and basically right"; this checklist is that, walked by hand once. The acceptance
test, "does it do what we promised", is the `full-centre-setup` journey (`frontend/e2e/full-centre-setup.spec.ts`).

## The instance

Built on 2026-10-26 from branch `feature/m3-d54-month3-checkpoint`, with no leftover data:

1. `docker compose down -v`, then `docker compose up -d` (a new, empty `pgdata` volume; `db/bootstrap-roles.sh`
   creates `tutoring_owner` and `tutoring_app`).
2. `dotnet run --project src/TutoringCentre.Api -- migrate`: exit 0. The five Month 3 migrations are applied:
   `20261012053358_AddStudentsAndParents`, `20261017042944_AddTeachers`, `20261018053631_AddGroups`,
   `20261023174541_AddEnrollments`, `20261024105240_AddSchedule`.
3. `dotnet run --project src/TutoringCentre.Api -- seed`: exit 0. Per centre (subjects, teachers, groups,
   students, parents, enrollments, rules, sessions): Nile 3, 2, 4, 12, 9, 25, 6, 24; Maadi 2, 1, 2, 6, 6, 6, 2, 8;
   `giza-new` all zero (Development only, one owner). Six users in Development (the five demo accounts and the
   `giza-new` owner). Every rule starts on the seed day, 2026-10-26. A second seed run (done on an earlier fresh
   volume the same day) changed no count.
4. `dotnet run --project src/TutoringCentre.Api --launch-profile https`; `GET /health/ready` answered `Healthy`.

## Automated checks (Claude Code)

Run with `curl` against `https://localhost:7197`, signed in through `GET /api/auth/antiforgery` and
`POST /api/auth/login`. "This Saturday" is the Saturday that starts the current Cairo week, 2026-10-24.

| # | Check | Expected | Observed | Result |
| --- | --- | --- | --- | --- |
| A1 | Sign in as `owner@nile.test` | 200 | 200 | pass |
| A2 | `GET /api/students?pageSize=1` | 200, keys `items`, `nextCursor` | 200, `items` (1 item), `nextCursor` (present: more students follow) | pass |
| A3 | `GET /api/groups` | 200, key `items` | 200, `items` (Nile's 4 groups) | pass |
| A4 | `GET /api/timetable?weekStart=2026-10-24` | 200, keys `timeZoneId`, `weekStart`, `sessions` | 200, `timeZoneId` = `Africa/Cairo`, `weekStart` = `2026-10-24`, 6 sessions (the seeded Monday–Thursday sessions from today on) | pass |
| A5 | `SELECT extname FROM pg_extension` | lists `btree_gist` | `btree_gist`, `plpgsql` | pass |
| A6 | A 403: `GET /api/staff` as `secretary@nile.test` | Problem Details with a stable code and no internal detail | `{"title":"You are not allowed to perform this action.","status":403,"detail":"You do not have permission to perform this action.","code":"auth.permission_denied","traceId":…,"correlationId":…}`: no exception, stack trace, SQL, type name or missing-permission key | pass |
| A7 | A 404: `GET /api/groups/00000000-0000-7000-8000-000000000000` as the owner | Problem Details with a stable code and no internal detail | `{"title":"The requested resource was not found.","status":404,"detail":"This group could not be found.","code":"group.not_found","traceId":…,"correlationId":…}`: nothing beyond the code | pass |
| A8 | Month 2 response headers on the 200, the 403 and the 404 | unchanged since Month 2 (`SecurityHeadersSetup`, last changed 2026-10-11) | all three carry `Content-Security-Policy: default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; connect-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'; object-src 'none'`, `X-Content-Type-Options: nosniff`, `Referrer-Policy: strict-origin-when-cross-origin`, `Cross-Origin-Opener-Policy: same-origin`, `Permissions-Policy: camera=(), microphone=(), geolocation=()`, `Cache-Control: no-store` | pass |

## Checks that need your eyes

Walk these in the browser (`npm run dev` in `frontend`, then http://localhost:5173) on the same fresh instance,
before any journey or test runs against it. Write what you saw in "Observed" and mark the result.

| # | Check | How | Expected | Observed | Result |
| --- | --- | --- | --- | --- | --- |
| M1 | Arabic search finds the dotted spelling | As `secretary@nile.test`, in Arabic, Students, type "احمد" | "أحمد محمود" is in the list | | |
| M2 | The demo teacher sees only own groups | As `teacher@both.test`, choose Nile, open Groups; then switch to Maadi | Nile: only "رياضيات - المجموعة أ"; Maadi: its two groups; no Students or Teachers in the sidebar | | |
| M3 | The two-seat group fills and refuses | As the secretary, "رياضيات - المجموعة ب", Roster: enroll one more student | "2 / 2" and "Full"; "Enroll student" is disabled with its reason, and a further enrollment (e.g. from a student's page) is refused with "This group is full" | | |
| M4 | A colliding rule is refused with the teacher message | As the owner, on "فيزياء - المجموعة أ" (منى السيد, Monday and Wednesday 16:00), add a weekly session on another of her groups, e.g. "كيمياء - المجموعة أ", Monday 4:30 PM for 1 hour | The dialog stays open with "The group's teacher already has a session at that time. Choose another day or time." | | |
| M5 | Timetable times are Cairo times | As the owner, Timetable, this week | Monday "رياضيات - المجموعة أ" reads 6:00–7:30 PM and "فيزياء - المجموعة أ" 4:00–5:30 PM, with the note that times are in the centre's time zone (Africa/Cairo); the same on a laptop set to another zone | | |
| M6 | The audit log shows today's actions with masked phones | As the owner, Audit log | Today's entries for M3's enrollment (and any rule from M4 that was accepted), plus the seed's entries by "System"; every parent or student phone shown masked (country code and last four digits only, e.g. `+20••••••0001`), never in full | | |

## Result

Automated checks A1–A8: all pass (2026-10-26). Manual checks M1–M6: to be marked by you.
