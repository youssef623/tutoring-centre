# `docs/onboarding-work/generated-model-inspection.md`

## Purpose

Documentation covering Full inspection of generated migration models; What the generated files mean; Current modeled storage; Important model-versus-migration distinctions. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/generated-model-inspection.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 49 lines and 5,724 bytes. Source hash: `de7de4ef973197f630fb0b88489ae9ab9b7d5364d812e615c9a46665bd68a649`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Full inspection of generated migration models
- Line 5: What the generated files mean
- Line 28: Current modeled storage
- Line 40: Important model-versus-migration distinctions

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Full inspection of generated migration models

All thirteen migration designers and `AppDbContextModelSnapshot.cs` were read completely. The source comprises 9,653 lines. Each file was partitioned into contiguous, non-overlapping full-source blocks. All fifty-one distinct blocks were inspected in seven bounded packets without truncation; exact duplicate blocks were verified by SHA-256 and mapped to every occurrence. The file/range/hash ledger is `generated-model-read-ledger.json`. This is full content inspection with duplicate identity verification, not inference from filenames or a grep-only summary.

## What the generated files mean

Each `*.Designer.cs` contains its migration identifier and `BuildTargetModel`: the entire EF model as it existed after that migration, not only that migration's new entity. The current snapshot contains `BuildModel`, which EF compares against the current runtime model when generating another migration. Historical files should retain their historical metadata; the presence of an older ProductVersion in an earlier designer is not model drift.

| Target model | EF ProductVersion | Distinct mapped entities/tables |
| --- | --- | ---: |
| InitialPlatform | 10.0.12 | 1 |
| AddIdentityAndMemberships | 10.0.12 | 6 |
| AddRuntimeRoleGrants | 10.0.12 | 6 |
| AddSubjects | 10.0.12 | 7 |
| AddMembershipRowVersion | 10.0.12 | 7 |
| AddAuditLog | 10.0.12 | 8 |
| AddCentreRowVersion | 10.0.12 | 8 |
| AddDataProtectionKeys | 10.0.12 | 9 |
| AddStudentsAndParents | 10.0.12 | 12 |
| AddTeachers | 10.0.13 | 13 |
| AddGroups | 10.0.13 | 14 |
| AddEnrollments | 10.0.13 | 15 |
| AddSchedule | 10.0.13 | 17 |
| Current snapshot | 10.0.13 | 17 |
```

The excerpt stops at line 24; use the original file for the remaining 25 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
