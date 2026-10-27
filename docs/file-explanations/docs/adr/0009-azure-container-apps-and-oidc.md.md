# `docs/adr/0009-azure-container-apps-and-oidc.md`

## Purpose

Documentation covering ADR 0009: Azure Container Apps and OIDC; Context; Alternatives considered; Decision; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0009-azure-container-apps-and-oidc.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 71 lines and 5,413 bytes. Source hash: `d6470590017758ec394719ef4bcc5029cd761a82bda2f10b14a8696853cf7108`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0009: Azure Container Apps and OIDC
- Line 9: Context
- Line 18: Alternatives considered
- Line 45: Decision
- Line 54: Consequences

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0009: Azure Container Apps and OIDC

- **Status:** Accepted — design only; not yet provisioned. No Azure subscription exists for this project as of
  2026-10-11 (Tasks 35.1–35.5 are undone, not done against a different target); see
  `docs/architecture/deployment.md` for exactly what is built versus designed, and `later.md` for the trigger
  to revisit this status.
- **Date:** 2026-10-11

## Context

The application needs to run on a public URL, deployed automatically from `main`, with migrations applied by
the pipeline — never by the running web process (Task 34.7 already rejected startup migrations: several
instances racing to apply the same migration, with no review step, is a correctness risk a container platform
with a separate job primitive avoids entirely). The pipeline needs credentials to push an image and update the
running service, and those credentials must not be a long-lived secret sitting in GitHub waiting to leak or
need rotation.

## Alternatives considered

- **Azure App Service** (a PaaS web app, deployed by pushing code or an image) is simpler to provision than a
  full Container Apps environment and well-trodden for a single web process. It has no first-class "job"
  primitive of its own for a one-shot task like `migrate`: the common pattern is either a startup script (the
  exact race Task 34.7 already rejected) or a separate, hand-rolled mechanism (a WebJob, or a second App
  Service tier doing nothing but running a command) bolted on rather than native to the platform. It also
```

The excerpt stops at line 24; use the original file for the remaining 47 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
