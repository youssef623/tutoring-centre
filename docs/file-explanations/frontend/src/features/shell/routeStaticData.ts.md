# `frontend/src/features/shell/routeStaticData.ts`

## Purpose

Source comment (intent): Per-route layout data read by the shell (Day 53). A route that needs more width than the reading-width column every other page uses — the timetable's seven day columns — sets `staticData: { layout: "wide" }`.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/shell/routeStaticData.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 344 bytes. Source hash: `5fdac2a804b375730b6399baae55d101a2595a0172987dd915c0651821517624`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../frontend/src/features/shell/routeStaticData.ts#L6): `interface StaticDataRouteOption {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
/**
 * Per-route layout data read by the shell (Day 53). A route that needs more width than the reading-width column
 * every other page uses — the timetable's seven day columns — sets `staticData: { layout: "wide" }`.
 */
declare module "@tanstack/react-router" {
  interface StaticDataRouteOption {
    layout?: "wide";
  }
}

export {};
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
