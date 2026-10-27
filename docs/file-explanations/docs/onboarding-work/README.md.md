# `docs/onboarding-work/README.md`

## Purpose

Documentation covering Onboarding audit working record; Requested acceptance conditions; Baseline; Evidence files; Completed work and evidence limits. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/README.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 4,840 bytes. Source hash: `5dfdcb71e31f5faf31d0b402e4d779a383cbf710c3e6f66e7081bbbb0c846bc5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Onboarding audit working record
- Line 5: Requested acceptance conditions
- Line 14: Baseline
- Line 18: Evidence files
- Line 24: Completed work and evidence limits

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Onboarding audit working record

This directory contains reproducible evidence for the current repository onboarding audit. These are working materials, not a replacement for `../PROJECT_OVERVIEW3.md`.

## Requested acceptance conditions

- Preserve `docs/PROJECT_OVERVIEW.md` and `docs/PROJECT_OVERVIEW2.md` byte for byte.
- Reverify repository statements from current files; distinguish source reading, tests actually executed, and interpretations.
- Produce a standalone, detailed seventeen-section `PROJECT_OVERVIEW3.md`, with at least 35 fully taught concepts, no shorter than v2 (4,018 lines, 57,697 whitespace-separated words).
- Rebuild the complete file index and important per-file explanations.
- Verify local Markdown targets and anchors, Mermaid syntax, factual findings, and the historical comparison.
- Commit and push the current branch; do not create a pull request or wait for CI.

## Baseline

The last commit touching v2 is `9d638ceb46099a4fd87ba8bc901b10e947c1f737`. The starting HEAD is `ef486f9`. `git diff --stat` reports 893 changed files, 90,595 insertions and 771 deletions. The detailed benchmark, including every concept's headings and sizes, is in `v2-benchmark.json`. The baseline has fifteen numbered top-level sections, eighteen level-one headings including its title and appendices, 35 concepts, 48 labelled fenced blocks, and twenty Mermaid diagrams.

## Evidence files

`inventory.py` creates `inventory.json` and `inventory-exclusions.json`. The inventory records paths, categories, sizes, hashes, tracking status, and sensitive-file metadata. It never reproduces `.env` content. It excludes dependencies, caches, compiled output and local editor state explicitly. The final index distinguishes its own documentation files from the application inventory to avoid recursive explanation generation.

Independent backend, frontend, and test/infrastructure drafts are being prepared in this directory. Their claims require verification before integration. A machine-read ledger proves which bytes were opened; it does not prove that every statement was manually reviewed. Executed commands and actual outcomes must be recorded separately from source-derived descriptions.

## Completed work and evidence limits
```

The excerpt stops at line 24; use the original file for the remaining 10 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
