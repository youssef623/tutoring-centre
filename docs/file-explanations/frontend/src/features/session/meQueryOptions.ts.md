# `frontend/src/features/session/meQueryOptions.ts`

## Purpose

Source comment (intent): GET /api/me as "the current session, or none" rather than a request that can fail: a 401 means signed out and resolves to null, so a component reading this query never has to treat "no session" as an error state. Any other failure (network, 500) still throws and is a real query error.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/meQueryOptions.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 821 bytes. Source hash: `7d5d16a297dd12d81700b5bd3099b5da8decdfec11bf2018cd2b5e950f3995cc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { queryOptions } from "@tanstack/react-query";`
- Line 2: `import { asApiError } from "@/api/apiFetch";`
- Line 3: `import { getGetMeQueryKey, getMe, type MeDto } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../frontend/src/features/session/meQueryOptions.ts#L10): `export const meQueryOptions = queryOptions({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/session/useSession.ts:2](../../../../../../frontend/src/features/session/useSession.ts#L2)
- [frontend/src/routes/_authenticated.test.tsx:6](../../../../../../frontend/src/routes/_authenticated.test.tsx#L6)
- [frontend/src/routes/_authenticated.tsx:5](../../../../../../frontend/src/routes/_authenticated.tsx#L5)
- [frontend/src/routes/change-password.tsx:18](../../../../../../frontend/src/routes/change-password.tsx#L18)
- [frontend/src/routes/login.tsx:15](../../../../../../frontend/src/routes/login.tsx#L15)
- [frontend/src/routes/select-centre.tsx:27](../../../../../../frontend/src/routes/select-centre.tsx#L27)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { queryOptions } from "@tanstack/react-query";
import { asApiError } from "@/api/apiFetch";
import { getGetMeQueryKey, getMe, type MeDto } from "@/api/generated/tutoring-centre";

/**
 * GET /api/me as "the current session, or none" rather than a request that can fail: a 401 means
 * signed out and resolves to null, so a component reading this query never has to treat "no session"
 * as an error state. Any other failure (network, 500) still throws and is a real query error.
 */
export const meQueryOptions = queryOptions({
  queryKey: getGetMeQueryKey(),
  queryFn: async ({ signal }): Promise<MeDto | null> => {
    try {
      return await getMe({ signal });
    } catch (error) {
      if (asApiError(error).status === 401) {
        return null;
      }

      throw error;
    }
  },
  retry: false,
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
