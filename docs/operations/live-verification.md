# Live verification

The month's security claims, checked with direct requests, not only through the browser. Each line is an
observed result with a date — not an assumption.

**Target (2026-10-11).** No Azure subscription is provisioned (see `docs/architecture/deployment.md`), so
there is no public URL yet. Every check below ran against the **local production stack**
(`docker compose -f compose.yaml -f compose.prod.yaml up --build`, commit `aac9abe968d1`, the tip of `main` at
the time) — the real image, the real Caddy reverse proxy, the real PostgreSQL roles — the closest available
stand-in. Three items are genuinely Azure-only and are recorded as not applicable, with the reason, rather than
skipped silently.

## Checklist

| # | Check | Date | Result |
| --- | --- | --- | --- |
| 1 | HTTP redirects to, or is refused in favour of, HTTPS | 2026-10-11 | **Pass.** `curl http://localhost:8443/` → `HTTP/1.0 400 Bad Request`, body `Client sent an HTTP request to an HTTPS server.` Port 80 has no listener at all (`Connection refused`). Caddy serves TLS exclusively; there is no plaintext fallback to redirect from. |
| 2 | Session cookie has `__Host-` prefix, `Secure`, `HttpOnly`, `SameSite=Lax` | 2026-10-11 | **Pass.** `POST /api/auth/login` (with a valid antiforgery token) → `Set-Cookie: __Host-tcm.session=…; path=/; secure; samesite=lax; httponly`. `__Host-` requires `Secure` and `Path=/` with no `Domain` attribute, both present. |
| 3 | CSP and the other security headers present on HTML, assets and API responses | 2026-10-11 | **Pass.** `GET /` (HTML), `GET /assets/index-*.js` (asset) and `GET /api/system/info` (API) each carry `content-security-policy`, `x-content-type-options: nosniff`; the asset carries `cache-control: public, max-age=31536000, immutable`; the API response carries `cache-control: no-store`. `strict-transport-security` is absent on all three — see the HSTS note below. |
| 4 | No CSP violations on any page, either language | 2026-10-11 | **Pass.** Signed in as `owner@maadi.test` in a real Chromium browser against this stack (the same technique used for Task 34.8); Dashboard, Subjects, Staff, Settings and Audit log, in Arabic then switched to English mid-session, same five pages revisited — `read_console_messages` (errors only) returned none, across both passes. |
| 5 | A forged `X-Forwarded-For` does not change the IP recorded in the logs | 2026-10-11 | **Pass.** `curl -H "X-Forwarded-For: 203.0.113.99" https://localhost:8443/api/system/info`, correlation id `bfc7ee25bd974389adb443859b3e086a`. The matching request-completion log line records `"ClientIp":"172.28.0.1"` (the real connecting address) — the forged value never appears. Caddy appends the true connecting client to `X-Forwarded-For` on every proxied request (its own default behaviour) and `Proxy:KnownNetworks`'s `ForwardLimit: 1` means the app reads only that nearest, Caddy-supplied hop — a header forged further back in the chain is structurally unreachable, not merely untrusted by policy. |
| 6 | Eleven rapid logins → 429; a different network can still sign in | 2026-10-11 | **Pass.** From one client with a valid antiforgery token: 10× `401 auth.invalid_credentials`, then `429` on the 11th. Independence proven directly: from inside the trusted Compose network with `X-Forwarded-For: 203.0.113.10`, exhausted that partition (10×, then 429, and it stays 429 on a follow-up probe); a second forwarded identity, `X-Forwarded-For: 203.0.113.20`, got its own fresh 10-request budget before its own 429 — A's lockout did not touch B's. (A genuinely separate client machine was not available for this check; the partition boundary was proven directly at the header the app actually trusts, which is the stronger version of the same claim.) |
| 7 | A POST without the antiforgery header is 403 | 2026-10-11 | **Pass.** `POST /api/subjects` with a valid session cookie but no `X-XSRF-TOKEN` → `403 auth.csrf_invalid`, `"A valid antiforgery token is required for this request."` No subject was created. |
| 8 | Cross-tenant GET, PUT and archive with the other demo centre's subject ID → 404 | 2026-10-11 | **Pass.** Signed in as `owner@nile.test`, targeting Maadi's `Mathematics` subject id: `GET` → `404 subject.not_found`; `PUT` (rename) → `404 subject.not_found`; `POST …/archive` → `404 subject.not_found`. Maadi's subject was confirmed unchanged afterward. |
| 9 | The secretary's `GET /api/staff` is 403 | 2026-10-11 | **Pass.** Signed in as `secretary@nile.test` → `403 auth.permission_denied`, `"You do not have permission to perform this action."` |
| 10 | `/api/system/info` shows the deployed SHA; `/health/ready` is 200 | 2026-10-11 | **Pass.** `{"applicationVersion":"1.0.0+aac9abe968d11aed5a93475503f1d40fd5084d4c", …, "demoMode":true}` — the full 40-character SHA matches `git rev-parse HEAD` at build time exactly. `/health/ready` → `200 Healthy`. |
| 11 | Error responses contain no stack trace | 2026-10-11 | **Pass.** A genuine unhandled exception was produced in the course of check 6 (hitting the app directly, bypassing Caddy's TLS termination, trips `AntiforgeryOptions.Cookie.SecurePolicy = Always`'s SSL check — a real `InvalidOperationException` with a full .NET stack trace, visible in the container's own log). The client-facing response was exactly `{"title":"An unexpected error occurred.","status":500,"code":"server.unexpected","traceId":"…","correlationId":"…"}` — no exception type, message or stack frame reached the client. |
| 12 | A role change produces an audit entry whose correlation ID finds a trace in telemetry | 2026-10-11 | **Partial.** `PUT /api/staff/{membershipId}/role` (secretary → teacher, as the Nile owner) returned `X-Correlation-Id: b01c30688b9c48b3b9e0547564d4ae47`; `GET /api/audit` shows the matching entry: `{"action":"updated","entityType":"membership","changes":[{"field":"role","before":"secretary","after":"teacher"}],"correlationId":"b01c30688b9c48b3b9e0547564d4ae47"}` — audit-side proven. The trace-lookup half needs `Otel:Endpoint` pointed at a running local viewer (Task 35.6); this machine was critically low on memory during this session (reaped a prior attempt — see Task 35.6's own notes) and the check was not forced a second time. Not independently verified today; the role change was reverted (teacher → secretary) immediately after. |
| 13 | `tutoring_app`, allowed network, outside a transaction, no tenant setting: `SELECT count(*) FROM academics.subjects;` → 0 | 2026-10-11 | **Pass.** `docker exec` into the local Postgres container as `tutoring_app`: `count = 0`. Row-level security's `FORCE` means even a role with table privileges sees nothing without `app.current_centre` set — proven directly, no firewall change needed since this is a local container, not a cloud database. |
| — | Database check needs a temporarily opened firewall rule | N/A | **Not applicable.** Local Postgres container, no firewall to open or close. |
| — | HSTS raise and redeploy through the pipeline | N/A | **Deferred, not applicable yet.** All applicable checks passed, which is the condition the manual sets for raising HSTS to its long-term value — but `Program.cs`'s own comment ties the short interim value explicitly to "once DNS/certificates are proven stable in Azure," which has not happened: no Azure deployment exists. Raising it now would contradict the reasoning already written into the code. Left at `TimeSpan.FromMinutes(5)`; revisit this exact line once a real deployment exists. |

## An incidental finding: two independent lockout mechanisms

Running check 6 (the 11-rapid-logins check, repeated several times while getting clean evidence) against
`owner@nile.test` tripped a **second**, separate defence that is not named in the manual's checklist: ASP.NET
Core Identity's own per-account lockout (`identity.users.access_failed_count`/`lockout_end`), independent of
the IP-partitioned rate limiter (Task 34.4). Confirmed directly:

```sql
SELECT email, access_failed_count, lockout_end, lockout_enabled FROM identity.users WHERE email='owner@nile.test';
--      email      | access_failed_count |          lockout_end          | lockout_enabled
-- owner@nile.test |                   0 | 2026-10-11 11:40:07.852509+00 | t
```

The browser's own login attempt against that account failed with the same generic "incorrect email or
password" message a wrong password would produce — the account-lockout state is not distinguishable from the
outside, which is the correct behaviour (it tells an attacker nothing). Check 4's browser walk was done signed
in as `owner@maadi.test` instead, specifically to avoid waiting out this lockout. Two independent layers
stopping the same class of attack, neither one documented as depending on the other, is a good sign — not a
finding that needs fixing — but it was not something any of Month 1's rate-limit tests named directly, so it
is recorded here.

## What was promoted to the smoke test

Nothing new this round. `smoke/tests/smoke.spec.ts` (Task 35.8) already automates the small, read-mostly subset
of this list that is safe to run on every deploy: `/health/ready`, the security headers on `/`,
`/api/system/info`'s reported SHA and `demoMode`, an unauthenticated `/api/subjects` 401, and a full
sign-in → Subjects → sign-out journey. The remaining checks here are either destructive in spirit (the
rate-limit lockout, the cross-tenant write attempts), need a second identity (the CSP/console walk), or need
infrastructure the smoke test deliberately doesn't carry (the direct-to-Postgres check) — keeping the smoke
test itself small, per Task 35.8's own constraint, matters more than completeness here.
