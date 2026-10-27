# `docs/operations/live-verification.md`

## Purpose

Documentation covering Live verification; Checklist; An incidental finding: two independent lockout mechanisms; What was promoted to the smoke test. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../operations/live-verification.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 62 lines and 9,734 bytes. Source hash: `59edd0a7316daaea8cfed8bbe5920782e9075d30a8aa2c74ffa823471c9c2794`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 50](../../../operations/live-verification.md#L50): `stopping the same class of attack, neither one documented as depending on the other, is a good sign — not a`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
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
```

The excerpt stops at line 24; use the original file for the remaining 38 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
