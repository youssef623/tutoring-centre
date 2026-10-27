# `.config/dotnet-tools.json`

## Purpose

JSON data with top-level keys `version`, `isRoot`, `tools`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../.config/dotnet-tools.json). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 183 bytes. Source hash: `e48342794ccf9dd33332c04e3c946488ab797c138f07d13b1a26c95b5664b3b9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "version": 1,
  "isRoot": true,
  "tools": {
    "dotnet-ef": {
      "version": "10.0.12",
      "commands": [
        "dotnet-ef"
      ],
      "rollForward": false
    }
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
