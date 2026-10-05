# src/TutoringCentre.Api/Auth/LoginRateLimiting.cs

## Purpose

Caps login attempts per client IP with a fixed-window limiter and answers rejections with Problem Details `auth.rate_limited` (429).

## Where It Fits

Api/Auth. Registered at `Program.cs:49`; attached to the login endpoint only (`RequireRateLimiting(LoginRateLimiting.PolicyName)` in `AuthEndpoints`); `UseRateLimiter` at `Program.cs:102` enforces it. Uses `ProblemResult` for the rejection body.

## Walkthrough

Constants: `PolicyName = "login"` (17), `TestPartitionHeaderName = "X-Test-RateLimit-Partition"` (25), `PermitLimit = 10` (27), `Window = 1 minute` (28).
`AddLoginRateLimiting` (30-67): `AddRateLimiter(...)`; `AddPolicy(PolicyName, httpContext => RateLimitPartition.GetFixedWindowLimiter(partitionKey: GetPartitionKey(httpContext), factory: _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = 1 min, QueueLimit = 0 }))` - each partition (client) has its own counter; `QueueLimit = 0` means excess requests are rejected, not queued. `OnRejected` (45-63): reads the lease's `RetryAfter` metadata (fallback: the window), sets header `Retry-After` in whole seconds, and writes `ProblemDetails{Status 429, Title "Too many requests.", Detail "Too many login attempts. Try again later.", code "auth.rate_limited"}` through `ProblemResult`.
`GetPartitionKey` (69-82): if `IHostEnvironment.IsEnvironment("Testing")` and the test header is present, use its value; otherwise `HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"`. Production and Development never read the header, so a caller cannot rotate it to dodge the limit.

## Concepts Used

### Rate limiting, lockout and enumeration resistance

#### What it means

*Rate limiting* caps how many requests one source may make in a time window (stops password spraying from one place). *Lockout* temporarily blocks one account after repeated failures (stops many guesses against one account). *Enumeration resistance* means failure responses (and their timing) reveal nothing about whether an account exists.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance](../../../../PROJECT_OVERVIEW2.md#626-rate-limiting-lockout-and-enumeration-resistance).)

#### Where it appears in this file

Fixed-window limiter.

#### How it works here

`GetFixedWindowLimiter` (36-43).

#### Why it matters here

Per-IP cap of 10 login requests per minute; the 11th gets 429 (`SecurityTests.Login_11TimesWithinAMinuteFromOneClient_11thReturns429WithRetryAfter`).

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Rejection body.

#### How it works here

`OnRejected`.

#### Why it matters here

429 keeps the project's uniform error shape and a stable code.

### Middleware and the request pipeline

#### What it means

Middleware components form a chain; each receives the request, may act before calling `next`, and may act again after it returns. Order is behaviour: a component only sees what earlier components set up and only wraps what comes after it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#8-routing-and-middleware](../../../../PROJECT_OVERVIEW2.md#8-routing-and-middleware).)

#### Where it appears in this file

`UseRateLimiter` placement.

#### How it works here

`Program.cs:102`.

#### Why it matters here

After authorization, before endpoint execution.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Header `X-Test-RateLimit-Partition` (honoured only when the host environment name is exactly `Testing`). Policy `login`; 10 requests / 1 minute / client IP.

## Gotchas and Issues

Behind a reverse proxy `RemoteIpAddress` would be the proxy's address, so all users would share one window - forwarded-header handling is documented as Month 2. The limiter is per process and in memory (framework default), so multiple instances each keep their own counters. The partition map has no cleanup logic in this file (library-managed).

## Related Files

- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](AuthEndpoints.cs.md)
- [`src/TutoringCentre.Api/Http/ProblemResult.cs`](../Http/ProblemResult.cs.md)
- [`tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs`](../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs`](../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs.md)
- [`docs/architecture/authentication.md`](../../../docs/architecture/authentication.md.md)
