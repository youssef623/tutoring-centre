# `frontend/README.md`

## Purpose

Documentation covering Frontend — Tutoring Centre Manager; Stack; Folder structure (end of Month 3); API contract used by the status page; Conventions. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../frontend/README.md). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 206 lines and 11,842 bytes. Source hash: `efe83a5ec69c02c271dad20f5f435678badca29a1a1ef9fb0042fd0a1ac15c47`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 170: `import reactX from "eslint-plugin-react-x";`
- Line 171: `import reactDom from "eslint-plugin-react-dom";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 72](../../../frontend/README.md#L72): `The one place a time becomes text. Every function takes the centre's IANA zone (timeZoneId, sent by the API with`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
# Frontend — Tutoring Centre Manager

Scaffolded Day 3 with Vite + React + TypeScript. This file records the decisions that shaped the scaffold, the real structure as it stands at the end of Month 3, and the API contract the status page depends on.

## Stack

- **React + TypeScript**, `strict` mode on.
- **Vite** for the dev server and build.
- **TanStack Router** for routing, **TanStack Query** for server state / data fetching.
- **Tailwind CSS** + **shadcn/ui** for styling and base components.
- **Generated API client** from the backend's OpenAPI spec — no hand-written fetch wrappers for endpoints.
- **i18next** with Arabic and English locales.
- **Logical CSS properties** (`margin-inline-start`, `padding-inline-end`, etc.) instead of physical ones (`margin-left`, `padding-right`), so layout flips correctly for Arabic RTL instead of needing a separate RTL stylesheet.

## Folder structure (end of Month 3)

```
frontend/
  src/
    app/            # app shell, providers
    routes/         # TanStack Router file-based routes (__root.tsx, login.tsx, _authenticated/*)
    api/            # generated API client (src/api/generated/), apiFetch, csrf
    features/       # one folder per feature (feature-based, not type-based):
                     #   auth, session (Can/useCan/useSession — see below), language, shell,
````

The excerpt stops at line 24; use the original file for the remaining 182 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
