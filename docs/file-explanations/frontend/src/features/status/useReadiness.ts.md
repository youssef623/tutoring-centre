# `frontend/src/features/status/useReadiness.ts`

## Purpose

Source comment (intent): Server state for the status page: cached under ["health", "ready"] and polled every 15 seconds.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/status/useReadiness.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 348 bytes. Source hash: `bd0af44e45d73fb3fa27e4cfdadde711f8ee7da468ededad1f2cf0944bf3da34`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQuery } from "@tanstack/react-query";`
- Line 2: `import { fetchReadiness } from "./api";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/features/status/useReadiness.ts#L5): `export function useReadiness() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/status/StatusCard.tsx:6](../../../../../../frontend/src/features/status/StatusCard.tsx#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { useQuery } from "@tanstack/react-query";
import { fetchReadiness } from "./api";

/** Server state for the status page: cached under ["health", "ready"] and polled every 15 seconds. */
export function useReadiness() {
  return useQuery({
    queryKey: ["health", "ready"],
    queryFn: fetchReadiness,
    refetchInterval: 15_000,
  });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
