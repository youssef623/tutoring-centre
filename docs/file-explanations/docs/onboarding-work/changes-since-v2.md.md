# `docs/onboarding-work/changes-since-v2.md`

## Purpose

Documentation covering File-level changes since the v2 baseline. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/changes-since-v2.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 899 lines and 70,037 bytes. Source hash: `ea4bf9f9e55023ee9db875bc68466273d66dfbe05b3a5b75d955880e0f66b7be`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: File-level changes since the v2 baseline

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# File-level changes since the v2 baseline

Generated directly with Git rename detection between the v2 document baseline and the audit starting HEAD. A status of M means modified; A added; D deleted; R a detected rename. A modification status does not by itself distinguish a rewrite from a small edit.

| Git status | Path(s) |
| --- | --- |
| A | `.dockerignore` |
| M | `.env.example` |
| M | `.github/dependabot.yml` |
| M | `.github/workflows/ci.yml` |
| M | `.gitignore` |
| A | `.gitleaksignore` |
| A | `CLAUDE.md` |
| A | `Caddyfile` |
| M | `Directory.Packages.props` |
| A | `Dockerfile` |
| M | `README.md` |
| M | `api.http` |
| A | `compose.prod.yaml` |
| M | `compose.yaml` |
| A | `database/demos/week4-rls.sql` |
| A | `db/bootstrap-roles.sh` |
| M | `docs/adr/0004-unit-of-work-port.md` |
| A | `docs/adr/0006-tenant-isolation.md` |
```

The excerpt stops at line 24; use the original file for the remaining 875 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
