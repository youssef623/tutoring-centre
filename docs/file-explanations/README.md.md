# `README.md`

## Purpose

Documentation covering Tutoring Centre Manager; Quick start; Prerequisites; Run locally; Architecture. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../README.md). [Complete file index](INDEX.md). [Current teaching overview](../PROJECT_OVERVIEW3.md).

The reviewed file has 235 lines and 14,973 bytes. Source hash: `77ef5ceac37170312603e300c78fcafbbcf7356fff6ccd6ba124d925f752a50b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Tutoring Centre Manager
- Line 14: Quick start
- Line 16: Prerequisites
- Line 23: Run locally
- Line 195: Architecture
- Line 233: Quality gates

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Tutoring Centre Manager

[![CI](https://github.com/youssef623/tutoring-centre/actions/workflows/ci.yml/badge.svg)](https://github.com/youssef623/tutoring-centre/actions/workflows/ci.yml)

A multi-tenant management platform for Egyptian tutoring centres: students and parents, groups and schedules, attendance (manual and QR), fees and payments, and a WhatsApp assistant that answers parents' questions in Egyptian Arabic and English through the same authorized use cases as the staff web app. Built with ASP.NET Core (.NET 10) using Clean Architecture and a hand-written CQRS pipeline, PostgreSQL, and a React + TypeScript frontend.

**Live instance:** not yet deployed — no Azure subscription provisioned yet (see
[Deployment](docs/architecture/deployment.md) for exactly what is built versus still only designed). This
section will carry the public URL once that work lands. Once it does, the demo accounts are the same emails
as the local seed (`owner@nile.test`, `owner@maadi.test`, `teacher@both.test`, `secretary@nile.test`) — the
demo password is never written here; it lives only in the deployment's own secret store
(`docs/architecture/deployment.md`'s secrets inventory).

## Quick start

### Prerequisites

- .NET 10 SDK (the exact line is pinned in `global.json`)
- Node.js LTS (22.12 or later)
- Docker Desktop (running)
- Git

### Run locally

```

The excerpt stops at line 24; use the original file for the remaining 211 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
