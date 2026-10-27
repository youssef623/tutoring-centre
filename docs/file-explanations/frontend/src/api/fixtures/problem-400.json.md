# `frontend/src/api/fixtures/problem-400.json`

## Purpose

JSON data with top-level keys `title`, `status`, `code`, `traceId`, `correlationId`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../frontend/src/api/fixtures/problem-400.json). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 7 lines and 223 bytes. Source hash: `170762a53530aeccc9c532988eabcffd3e37f1d306f0bf9838c2027c411cd8d1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "title": "The request could not be read.",
  "status": 400,
  "code": "request.malformed",
  "traceId": "00-00000000000000000000000000000000-0000000000000000-00",
  "correlationId": "00000000000000000000000000000000"
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
