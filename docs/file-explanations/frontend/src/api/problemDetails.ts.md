# `frontend/src/api/problemDetails.ts`

## Purpose

Source comment (intent): RFC 9457 Problem Details as produced by the API (Day 11), including the project's extensions.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/api/problemDetails.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 860 bytes. Source hash: `6cfd245257d76900b655f1d70147c0b5b8b4997ad3e1a68b0035ffd3544d2541`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 2](../../../../../frontend/src/api/problemDetails.ts#L2): `export interface ProblemDetails {`
- [Line 17](../../../../../frontend/src/api/problemDetails.ts#L17): `export function isProblemDetails(value: unknown): value is ProblemDetails {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/api/apiFetch.ts:3](../../../../../frontend/src/api/apiFetch.ts#L3)
- [frontend/src/api/problemDetails.test.ts:2](../../../../../frontend/src/api/problemDetails.test.ts#L2)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
/** RFC 9457 Problem Details as produced by the API (Day 11), including the project's extensions. */
export interface ProblemDetails {
  type?: string;
  title: string;
  status: number;
  detail?: string;
  instance?: string;
  /** Stable machine-readable error code, e.g. "centre.slug_invalid". */
  code?: string;
  traceId?: string;
  correlationId?: string;
  /** Field name (camelCase) → messages; present on validation failures. */
  errors?: Record<string, string[]>;
}

/** True when `value` has the minimum Problem Details shape: a string `title` and a numeric `status`. */
export function isProblemDetails(value: unknown): value is ProblemDetails {
  return (
    typeof value === "object" &&
    value !== null &&
    "title" in value &&
    typeof value.title === "string" &&
    "status" in value &&
    typeof value.status === "number"
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
