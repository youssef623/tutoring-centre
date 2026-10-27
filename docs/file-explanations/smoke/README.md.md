# `smoke/README.md`

## Purpose

Documentation covering Deployed smoke test; Running it; In the deployment pipeline. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../smoke/README.md). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,963 bytes. Source hash: `3cc30df9bf3409ec9f2b4e133b631e830fb68758912fa5ac380639f9ba474607`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Deployed smoke test
- Line 8: Running it
- Line 28: In the deployment pipeline

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
# Deployed smoke test

Proves a running instance is fit to serve: health, security headers, the deployed commit, that `/api` requires
auth, and that the demo owner can sign in, see Subjects, and sign out. Creates no data; takes about a minute.

Separate from `frontend/e2e`: this project never starts a local server, only points at one already running.

## Running it

```sh
npm install
npx playwright install --with-deps chromium  # once per machine
SMOKE_BASE_URL=https://localhost:8443 SMOKE_EXPECTED_SHA=local SMOKE_PASSWORD=<the demo owner's password> \
  SMOKE_EXPECT_HSTS=false npm test
```

- `SMOKE_BASE_URL` — the instance to check. Against the local production stack (Day 34), that's
  `https://localhost:8443`.
- `SMOKE_EXPECTED_SHA` — the commit SHA `/api/system/info` should report. The local stack's default build
  (`COMMIT_SHA` unset) reports `local`; a CI or deployed build passes the real Git SHA.
- `SMOKE_PASSWORD` — the demo Nile owner's (`owner@nile.test`) password. Never printed, logged, or captured —
  traces, screenshots and video are off in `playwright.config.ts` for exactly this reason.
- `SMOKE_EXPECT_HSTS` — defaults to `true`. Set to `false` only against the local production stack: ASP.NET
  Core's `UseHsts()` excludes the `localhost`/`127.0.0.1`/`[::1]` hosts by default, and Caddy's local site
````

The excerpt stops at line 24; use the original file for the remaining 9 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
