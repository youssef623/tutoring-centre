# `frontend/src/features/session/useSession.ts`

## Purpose

Source comment (intent): The signed-in session: `me` is the current user's profile, or null while signed out.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/useSession.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 388 bytes. Source hash: `00da071826054ab37ff02fe526c74571a306142064db74d16233a8c689cef87e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQuery } from "@tanstack/react-query";`
- Line 2: `import { meQueryOptions } from "./meQueryOptions";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/features/session/useSession.ts#L5): `export function useSession() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/session/useCan.ts:1](../../../../../../frontend/src/features/session/useCan.ts#L1)
- [frontend/src/features/session/useCentreTimeZone.ts:1](../../../../../../frontend/src/features/session/useCentreTimeZone.ts#L1)
- [frontend/src/features/session/useSession.test.ts:6](../../../../../../frontend/src/features/session/useSession.test.ts#L6)
- [frontend/src/routes/_authenticated.tsx:6](../../../../../../frontend/src/routes/_authenticated.tsx#L6)
- [frontend/src/routes/_authenticated/index.tsx:7](../../../../../../frontend/src/routes/_authenticated/index.tsx#L7)
- [frontend/src/routes/change-password.tsx:19](../../../../../../frontend/src/routes/change-password.tsx#L19)
- [frontend/src/routes/select-centre.tsx:28](../../../../../../frontend/src/routes/select-centre.tsx#L28)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { useQuery } from "@tanstack/react-query";
import { meQueryOptions } from "./meQueryOptions";

/** The signed-in session: `me` is the current user's profile, or null while signed out. */
export function useSession() {
  const { data, isPending, isError, error } = useQuery(meQueryOptions);

  return {
    me: data ?? null,
    isLoading: isPending,
    isError,
    error,
  };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
