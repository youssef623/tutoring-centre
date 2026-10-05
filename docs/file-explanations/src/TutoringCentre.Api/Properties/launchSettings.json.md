# src/TutoringCentre.Api/Properties/launchSettings.json

## Purpose

Local-development launch profiles for `dotnet run` and IDEs; the `https` profile is the one the frontend, the E2E suite and the README use.

## Where It Fits

Read by `dotnet run --launch-profile` and Visual Studio/Rider. Not used in production or tests. The Vite proxy target (`frontend/vite.config.ts`, default `https://localhost:7197`) and `playwright.config.ts` point at the `https` profile.

## Walkthrough

Profiles `https` (first) with `applicationUrl https://localhost:7197;http://localhost:5245`, and `http` with `applicationUrl http://localhost:5080`, both `commandName: Project`, `dotnetRunMessages: true`, `launchBrowser: true`, `ASPNETCORE_ENVIRONMENT=Development`. The merge swapped the order of the two profiles (https first) and the README now documents the https profile, because the session and antiforgery cookies are `Secure` and `__Host-`, which the browser stores only from an HTTPS origin. `Development` is what turns on startup auto-migration. The plain-http sibling binding `http://localhost:5245` of the https profile is what `playwright.config.ts` uses for the readiness probe.

## Concepts Used

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#11-configuration--environment--deployment](../../../../PROJECT_OVERVIEW2.md#11-configuration--environment--deployment).)

#### Where it appears in this file

Environment selection.

#### How it works here

`ASPNETCORE_ENVIRONMENT=Development`.

#### Why it matters here

Controls auto-migration and Development-only endpoints such as `/openapi/v1.json`.

### Cookie authentication and server-side sessions

#### What it means

After a successful login the server must remember *who* the browser is on later requests (HTTP itself is stateless). Cookie authentication does that with one cookie that the browser attaches automatically. Here the cookie holds an **encrypted, tamper-proof ticket** (the user's claims); only the server can read or create it, and flags such as `HttpOnly`, `Secure`, `SameSite` and the `__Host-` name prefix limit where and how browsers send it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions](../../../../PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions).)

#### Where it appears in this file

HTTPS requirement.

#### How it works here

`applicationUrl` of the `https` profile.

#### Why it matters here

`__Host-` cookies are rejected over plain http.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

`ASPNETCORE_ENVIRONMENT=Development`; ports 7197 (https), 5245 (http sibling of the https profile), 5080 (the separate `http` profile).

## Gotchas and Issues

`launchBrowser: true` opens the base URL, which has no page (the unmatched-route 404 problem+json). The `http` profile (5080) still exists but is not usable for sign-in in a browser because of the `Secure` cookies.

## Related Files

- [`src/TutoringCentre.Api/Program.cs`](../Program.cs.md)
- [`frontend/vite.config.ts`](../../../frontend/vite.config.ts.md)
- [`frontend/playwright.config.ts`](../../../frontend/playwright.config.ts.md)
- [`README.md`](../../../README.md.md)
