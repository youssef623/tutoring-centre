# `frontend/orval.config.ts`

## Purpose

Current configuration file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **configuration**. [Open current file](../../../frontend/orval.config.ts). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 713 bytes. Source hash: `607f2cadf68ce6b80421d904a0dbd926eb6bbcbf939dc9d829a5567f0f1e41b3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { defineConfig } from "orval";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { defineConfig } from "orval";

export default defineConfig({
  tutoringCentre: {
    input: { target: "./openapi/TutoringCentre.Api.json" },
    output: {
      client: "react-query",
      httpClient: "fetch",
      mode: "single",
      target: "./src/api/generated/tutoring-centre.ts",
      clean: true,
      override: {
        // Every generated request goes through apiFetch — the single place errors become ApiError.
        mutator: { path: "./src/api/apiFetch.ts", name: "apiFetch" },
        // apiFetch returns the parsed body (not { data, status, headers }), so generated types must describe the body only.
        fetch: { includeHttpResponseReturnType: false },
      },
    },
  },
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
