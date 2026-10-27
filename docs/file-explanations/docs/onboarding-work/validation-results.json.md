# `docs/onboarding-work/validation-results.json`

## Purpose

JSON data with top-level keys `files_checked`, `links_checked`, `errors`, `v2`, `v3`, `historical_overviews_unchanged`, `concepts`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../onboarding-work/validation-results.json). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 408 lines and 12,273 bytes. Source hash: `5f758cc298f576731ce64c803e5cd9756fc3fda9cfb3c789beee1bcd63756f77`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```json
{
  "files_checked": 1077,
  "links_checked": 14911,
  "errors": [],
  "v2": {
    "lines": 4018,
    "words": 57697
  },
  "v3": {
    "lines": 6783,
    "words": 152612,
    "concepts": 62,
    "numbered_top_level_sections": [
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "10",
      "11",
      "12",
      "13",
      "14",
      "15",
      "16",
      "17"
    ]
  },
  "historical_overviews_unchanged": true,
  "concepts": [
    {
      "number": "6.1",
      "title": "Clean Architecture, dependency inversion, and the composition root",
      "words_without_fences": 1516,
      "missing_teaching_headings": []
    },
    {
      "number": "6.2",
      "title": "Dependency injection, lifetimes, and shared scope ownership",
      "words_without_fences": 1841,
      "missing_teaching_headings": []
    },
    {
      "number": "6.3",
      "title": "CQRS and the current dispatcher pipeline",
      "words_without_fences": 1375,
      "missing_teaching_headings": []
    },
    {
      "number": "6.4",
      "title": "Results, expected failures, and exception boundaries",
      "words_without_fences": 1162,
      "missing_teaching_headings": []
    },
    {
      "number": "6.5",
      "title": "Validation, invariants, and multiple lines of defense",
      "words_without_fences": 877,
      "missing_teaching_headings": []
    },
    {
      "number": "6.6",
      "title": "Entities, value objects, aggregates, and historical state",
      "words_without_fences": 885,
      "missing_teaching_headings": []
    },
    {
      "number": "6.7",
      "title": "Unit of work, transaction-local tenancy, and cleanup",
      "words_without_fences": 1090,
      "missing_teaching_headings": []
    },
    {
      "number": "6.8",
      "title": "Repositories, DTO projections, and EF Core's execution model",
      "words_without_fences": 1794,
      "missing_teaching_headings": []
    },
    {
      "number": "6.9",
      "title": "Optimistic concurrency, uniqueness, row locks, and advisory locks",
      "words_without_fences": 1122,
      "missing_teaching_headings": []
    },
    {
      "number": "6.10",
```

The excerpt stops at line 90; use the original file for the remaining 318 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
