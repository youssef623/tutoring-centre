# `api.http`

## Purpose

Current other file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **other**. [Open current file](../../api.http). [Complete file index](INDEX.md). [Current teaching overview](../PROJECT_OVERVIEW3.md).

The reviewed file has 561 lines and 24,182 bytes. Source hash: `ce5df2b7909a727c52b382db15c24087f920fe562a378959788aec83f6fcfd72`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Manual walkthrough of the session API (Days 15-16: cookie auth, CSRF, session revalidation, rate limiting).
- Line 2: Requires: `docker compose up -d`, the API running on its "https" launch profile (the session and antiforgery
- Line 3: cookies are Secure + __Host-, which a real browser — and the antiforgery system itself — only accepts over
- Line 4: HTTPS), and staff seeded (`dotnet run --project src/TutoringCentre.Api -- seed`, with Seed:Password configured):
- Line 5: dotnet dev-certs https --trust   (once per machine)
- Line 6: dotnet run --project src/TutoringCentre.Api --launch-profile https
- Line 8: The password is never written here. Create a git-ignored `.env` file at the repo root containing:
- Line 9: password=<the Seed:Password value you configured>
- Line 10: VS Code's REST Client extension reads it via {{$dotenv password}}; no secret is ever committed.
- Line 14: 0. Fetch a CSRF token — expect 200, Cache-Control: no-store, and the __Host-tcm.xsrf cookie.
- Line 15: Every POST below sends it back as X-XSRF-TOKEN; re-run this step whenever a POST fails with 403 auth.csrf_invalid.
- Line 16: @name antiforgery
- Line 19: 1. Log in as owner@nile.test — expect 200, Set-Cookie __Host-tcm.session, activeCentreId = nile-centre's id
- Line 20: @name loginOwner
- Line 30: 2. GET /api/me — expect 200, active centre nile-centre
- Line 33: 3. Log in as teacher@both.test — expect 200, no active centre (two active memberships, no auto-select)
- Line 34: @name loginTeacher
- Line 44: 4. GET /api/me — expect 200, activeCentreId null, two memberships.
- Line 45: Copy the "maadi-hub" membership's centreId from the response body into @maadiHubCentreId below.
- Line 48: 5. Select maadi-hub — expect 204
- Line 58: 6. GET /api/me — expect 200, active centre maadi-hub
- Line 61: 7. Select a random GUID — expect 403 tenant.no_membership; /api/me still shows maadi-hub afterwards
- Line 70: 8. Logout WITHOUT the CSRF header — expect 403 auth.csrf_invalid; the session still works afterwards
- Line 73: 8b. GET /api/me — expect 200 (step 8 had no side effects)
- Line 76: 9. Logout, this time with the token — expect 204
- Line 80: 10. GET /api/me — expect 401
- Line 83: Manual walkthrough of the subjects API (Day 26): the six Subject endpoints against a real session.
- Line 85: 11. Fetch a fresh CSRF token (the session from step 9 ended) — expect 200
- Line 86: @name antiforgerySubjects
- Line 89: 12. Log in as owner@nile.test — expect 200, active centre nile-centre

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
### Manual walkthrough of the session API (Days 15-16: cookie auth, CSRF, session revalidation, rate limiting).
### Requires: `docker compose up -d`, the API running on its "https" launch profile (the session and antiforgery
### cookies are Secure + __Host-, which a real browser — and the antiforgery system itself — only accepts over
### HTTPS), and staff seeded (`dotnet run --project src/TutoringCentre.Api -- seed`, with Seed:Password configured):
###   dotnet dev-certs https --trust   (once per machine)
###   dotnet run --project src/TutoringCentre.Api --launch-profile https
###
### The password is never written here. Create a git-ignored `.env` file at the repo root containing:
###   password=<the Seed:Password value you configured>
### VS Code's REST Client extension reads it via {{$dotenv password}}; no secret is ever committed.

@baseUrl = https://localhost:7197

### 0. Fetch a CSRF token — expect 200, Cache-Control: no-store, and the __Host-tcm.xsrf cookie.
### Every POST below sends it back as X-XSRF-TOKEN; re-run this step whenever a POST fails with 403 auth.csrf_invalid.
# @name antiforgery
GET {{baseUrl}}/api/auth/antiforgery

### 1. Log in as owner@nile.test — expect 200, Set-Cookie __Host-tcm.session, activeCentreId = nile-centre's id
# @name loginOwner
POST {{baseUrl}}/api/auth/login
Content-Type: application/json
X-XSRF-TOKEN: {{antiforgery.response.body.$.token}}

{
  "email": "owner@nile.test",
  "password": "{{$dotenv password}}"
}

### 2. GET /api/me — expect 200, active centre nile-centre
GET {{baseUrl}}/api/me

### 3. Log in as teacher@both.test — expect 200, no active centre (two active memberships, no auto-select)
# @name loginTeacher
POST {{baseUrl}}/api/auth/login
Content-Type: application/json
X-XSRF-TOKEN: {{antiforgery.response.body.$.token}}

{
  "email": "teacher@both.test",
  "password": "{{$dotenv password}}"
}

### 4. GET /api/me — expect 200, activeCentreId null, two memberships.
### Copy the "maadi-hub" membership's centreId from the response body into @maadiHubCentreId below.
GET {{baseUrl}}/api/me

### 5. Select maadi-hub — expect 204
@maadiHubCentreId = replace-with-the-centreId-from-step-4
POST {{baseUrl}}/api/session/centre
Content-Type: application/json
X-XSRF-TOKEN: {{antiforgery.response.body.$.token}}

{
  "centreId": "{{maadiHubCentreId}}"
}

### 6. GET /api/me — expect 200, active centre maadi-hub
GET {{baseUrl}}/api/me

### 7. Select a random GUID — expect 403 tenant.no_membership; /api/me still shows maadi-hub afterwards
POST {{baseUrl}}/api/session/centre
Content-Type: application/json
X-XSRF-TOKEN: {{antiforgery.response.body.$.token}}

{
  "centreId": "00000000-0000-0000-0000-000000000000"
}

### 8. Logout WITHOUT the CSRF header — expect 403 auth.csrf_invalid; the session still works afterwards
POST {{baseUrl}}/api/auth/logout

### 8b. GET /api/me — expect 200 (step 8 had no side effects)
GET {{baseUrl}}/api/me

### 9. Logout, this time with the token — expect 204
POST {{baseUrl}}/api/auth/logout
X-XSRF-TOKEN: {{antiforgery.response.body.$.token}}

### 10. GET /api/me — expect 401
GET {{baseUrl}}/api/me

### Manual walkthrough of the subjects API (Day 26): the six Subject endpoints against a real session.

### 11. Fetch a fresh CSRF token (the session from step 9 ended) — expect 200
# @name antiforgerySubjects
GET {{baseUrl}}/api/auth/antiforgery

### 12. Log in as owner@nile.test — expect 200, active centre nile-centre
POST {{baseUrl}}/api/auth/login
```

The excerpt stops at line 90; use the original file for the remaining 471 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
