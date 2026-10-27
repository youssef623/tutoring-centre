# `frontend/src/features/status/api.ts`

## Purpose

Source comment (intent): Calls GET /health/ready (contract in frontend/README.md). 200 → healthy; 503 → unavailable (API up, database down). Any other status, or a network failure, throws: the API itself can't be trusted or reached.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/status/api.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 709 bytes. Source hash: `07d41330d69b916e025c9e744de504ce403b6b7dc33438b1907b6e8f37384abd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 1](../../../../../../frontend/src/features/status/api.ts#L1): `export type ReadinessStatus = {`
- [Line 11](../../../../../../frontend/src/features/status/api.ts#L11): `export async function fetchReadiness(): Promise<ReadinessStatus> {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/status/useReadiness.ts:2](../../../../../../frontend/src/features/status/useReadiness.ts#L2)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
export type ReadinessStatus = {
  state: "healthy" | "unavailable";
  checkedAt: Date;
};

/**
 * Calls GET /health/ready (contract in frontend/README.md).
 * 200 → healthy; 503 → unavailable (API up, database down).
 * Any other status, or a network failure, throws: the API itself can't be trusted or reached.
 */
export async function fetchReadiness(): Promise<ReadinessStatus> {
  const response = await fetch("/health/ready");

  if (response.status === 200) {
    return { state: "healthy", checkedAt: new Date() };
  }

  if (response.status === 503) {
    return { state: "unavailable", checkedAt: new Date() };
  }

  throw new Error(`Unexpected readiness status ${String(response.status)}.`);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
