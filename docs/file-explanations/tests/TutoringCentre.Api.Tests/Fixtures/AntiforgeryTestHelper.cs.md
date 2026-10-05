# tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs

## Purpose

Test helper that fetches a real antiforgery token and attaches it (plus a per-call rate-limit partition) to every POST, so tests exercise the CSRF protection instead of bypassing it.

## Where It Fits

Api.Tests/Fixtures. Used by `LoginFlowTests` and `SecurityTests`. Depends on `AntiforgerySetup.HeaderName` and `LoginRateLimiting.TestPartitionHeaderName` from the Api project.

## Walkthrough

`AntiforgeryTestHelper` (8-40).
- `GetCsrfTokenAsync(client)` (13-19): `GET /api/auth/antiforgery`, parses the JSON and returns `token`. The same `HttpClient` stores the double-submit cookie `__Host-tcm.xsrf` that the response sets (client-side `CookieContainer`).
- `PostAsync(client, uri, content, rateLimitPartition = null)` (27-35): gets a fresh token; builds a POST with the optional body; adds header `AntiforgerySetup.HeaderName` (the CSRF header) and `LoginRateLimiting.TestPartitionHeaderName` with the given partition or a new `Guid` - so ordinary tests never share a login rate-limit window; sends it.
- `PostAsJsonAsync<TValue>` (37-39) wraps `PostAsync` with `JsonContent.Create(value)`.
Only the test that wants to trigger the 429 passes the same partition string on every call (in `SecurityTests`, the constant `partition` of the 429 test).

## Concepts Used

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../../../../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

Token fetch and header attachment.

#### How it works here

`GetCsrfTokenAsync` (1) and the line that adds the CSRF header (line 32).

#### Why it matters here

A real browser must do exactly this (`frontend/src/api/csrf.ts` and `apiFetch.ts`); the helper reproduces it in C# so the tests stay honest.

### Rate limiting, lockout and enumeration resistance

#### What it means

*Rate limiting* caps how many requests one source may make in a time window (stops password spraying from one place). *Lockout* temporarily blocks one account after repeated failures (stops many guesses against one account). *Enumeration resistance* means failure responses (and their timing) reveal nothing about whether an account exists.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance](../../../../PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance).)

#### Where it appears in this file

Per-call partition.

#### How it works here

The line that adds the partition header (line 33) and the doc comment above `PostAsync`.

#### Why it matters here

Without a distinct partition per call, the 10-per-minute limit would be exhausted by the whole test suite, because every request comes from the same test-server IP.

## Data and Control Flow

A test calls `PostAsJsonAsync` -> helper issues `GET /api/auth/antiforgery` (response carries token in body and sets the xsrf cookie) -> helper POSTs with header `X-XSRF-TOKEN: <token>` (header name taken from `AntiforgerySetup.HeaderName`) and `X-Test-RateLimit-Partition: <guid>` -> the filter compares header token and cookie token.

## Configuration and Environment

None directly; the partition header is only honoured in the `Testing` environment (`LoginRateLimiting.GetPartitionKey`).

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs`](../Auth/LoginFlowTests.cs.md)
- [`tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs`](../Auth/SecurityTests.cs.md)
- [`src/TutoringCentre.Api/Auth/AntiforgerySetup.cs`](../../../src/TutoringCentre.Api/Auth/AntiforgerySetup.cs.md)
- [`src/TutoringCentre.Api/Auth/LoginRateLimiting.cs`](../../../src/TutoringCentre.Api/Auth/LoginRateLimiting.cs.md)
