# `docs/adr/0005-cookie-authentication.md`

## Purpose

Documentation covering ADR 0005: Encrypted cookie authentication over JWT; Context; Decision; Alternatives considered; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0005-cookie-authentication.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 67 lines and 4,633 bytes. Source hash: `8fc02c27073086ce4505cb87d5bb18b6a52d094cc8f0f0e5f8f39933de6e0112`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0005: Encrypted cookie authentication over JWT
- Line 6: Context
- Line 14: Decision
- Line 40: Alternatives considered
- Line 55: Consequences

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0005: Encrypted cookie authentication over JWT

- **Status:** Accepted
- **Date:** 2026-10-05

## Context

The frontend is a same-origin SPA served from the same host as the API (via the Vite dev proxy today, a
single deployable in production) — there is no third-party client, no mobile app and no cross-origin caller
to support. Users are staff only (owners, teachers, secretaries), not the public, and a user can belong to
several centres at once, so a session has to carry not just "who" but "acting in which centre" — and that
second part changes mid-session, through its own explicit action, without a fresh login.

## Decision

An encrypted, `HttpOnly`, `__Host-`-prefixed cookie (`__Host-tcm.session`) holds the session: user ID,
security stamp, and the selected centre and role once one is chosen. The browser never reads or stores the
token itself — `HttpOnly` makes it inaccessible to JavaScript, closing off the main XSS exfiltration path a
token held in `localStorage` or a JS-readable cookie would leave open.

Centre selection is server-side and server-validated: `POST /api/session/centre` re-issues the cookie only
after confirming an active membership (`tenant.no_membership` otherwise, identical whether the centre
doesn't exist, isn't the caller's, or is inactive) — the client never asserts which centre it is acting in on
a later request, only which one it asked to switch into.
```

The excerpt stops at line 24; use the original file for the remaining 43 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
