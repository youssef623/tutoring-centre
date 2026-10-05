# api.http

## Purpose

A manual walkthrough of the session API for VS Code's REST Client extension: CSRF token, login as two users, centre selection, logout with and without a token.

## Where It Fits

Repository root; a developer tool only (not used by tests or CI). Calls the endpoints in `AuthEndpoints.cs` on `https://localhost:7197`. Complements the automated `LoginFlowTests` and `SecurityTests`.

## Walkthrough

Header comments (1-10): requires `docker compose up -d`, the API on its `https` launch profile (cookies are `Secure` + `__Host-`), staff seeded (`dotnet run --project src/TutoringCentre.Api -- seed` with `Seed:Password`), a trusted dev certificate (`dotnet dev-certs https --trust`). The password is never written in the file: a git-ignored `.env` provides `password=...`, read by REST Client as `{{$dotenv password}}`. `@baseUrl = https://localhost:7197`. Requests in order: 0 `GET /api/auth/antiforgery` (named `antiforgery`; expects 200, `Cache-Control: no-store`, the `__Host-tcm.xsrf` cookie); 1 login as `owner@nile.test` with `X-XSRF-TOKEN: {{antiforgery.response.body.$.token}}`; 2 `GET /api/me`; 3 login as `teacher@both.test` (no active centre); 4 `GET /api/me` (null centre, two memberships; copy the `maadi-hub` id); 5 `POST /api/session/centre` (204); 6 `GET /api/me`; 7 select a zero GUID (403 `tenant.no_membership`); 8 logout without the header (403 `auth.csrf_invalid`) and 8b `GET /api/me` (still 200); 9 logout with the token (204); 10 `GET /api/me` (401).

## Concepts Used

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#11-configuration--environment--deployment](../PROJECT_OVERVIEW2.md#11-configuration--environment--deployment).)

#### Where it appears in this file

Password from a git-ignored `.env`.

#### How it works here

Lines 1-10 and the `{{$dotenv password}}` placeholders.

#### Why it matters here

Keeps the seed password out of the repository while allowing a copy-paste workflow.

### CSRF protection with antiforgery tokens

#### What it means

Because browsers attach cookies automatically, a malicious *other* site can make the victim's browser send a state-changing request. CSRF protection adds a second, explicit proof that the request came from the application's own script: a token that must be sent in a header, which a foreign site cannot read or set.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens](../PROJECT_OVERVIEW2.md#625-csrf-protection-double-submit-antiforgery-tokens).)

#### Where it appears in this file

Token reuse.

#### How it works here

Every POST sends the token from request 0.

#### Why it matters here

Shows the manual version of what `apiFetch` does automatically.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Variable `password` from `.env` (not committed); URL `https://localhost:7197`.

## Gotchas and Issues

The comment in step 0 says to re-run it whenever a POST fails with 403 `auth.csrf_invalid` - the token is bound to the signed-in user, so after login or logout a fresh token may be needed (the frontend refreshes automatically).

## Related Files

- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](src/TutoringCentre.Api/Auth/AuthEndpoints.cs.md)
- [`src/TutoringCentre.Api/Properties/launchSettings.json`](src/TutoringCentre.Api/Properties/launchSettings.json.md)
- [`README.md`](README.md.md)
