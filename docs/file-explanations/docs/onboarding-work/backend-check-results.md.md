# `docs/onboarding-work/backend-check-results.md`

## Purpose

Documentation covering Backend checks executed during the onboarding audit; Environment; Build; Tests; Other checks. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/backend-check-results.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 64 lines and 4,098 bytes. Source hash: `61550095fc5c1e436072eac3a47e14510e850a1384dfb65e6afd0cb881c2b8aa`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Backend checks executed during the onboarding audit
- Line 5: Environment
- Line 11: Build
- Line 35: Tests
- Line 51: Other checks

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
# Backend checks executed during the onboarding audit

Date: 2026-10-27. Working directory: repository root. These are observations from this session, not copied historical documentation.

## Environment

`dotnet --info` reports SDK 10.0.401 and host/runtime 10.0.12 on Windows x64. In the restricted shell its workload-information query also prints a Service Control Manager access-denied exception. That is an environment inspection limitation, not evidence that the SDK is absent.

`docker version` reports Docker client 29.8.1, API 1.56, desktop-linux context. Restricted access to the daemon fails with permission denied on the Docker Desktop named pipe. The full test run therefore uses the approved unrestricted command environment, where Testcontainers successfully starts disposable databases.

## Build

Command: `dotnet build -c Release --no-restore`.

Restricted execution exited 1 after 5.94 seconds, with `Build FAILED`, zero warnings and zero errors and no useful diagnostic. Re-running the same command with approved unrestricted access succeeded.

Final unrestricted result:

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:01:01.90
```
````

The excerpt stops at line 24; use the original file for the remaining 40 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
