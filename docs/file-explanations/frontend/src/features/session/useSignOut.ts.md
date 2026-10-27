# `frontend/src/features/session/useSignOut.ts`

## Purpose

Source comment (intent): Logs out, refreshes the CSRF token (bound to the now-signed-out user), clears every cached query, then returns to login.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/useSignOut.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 675 bytes. Source hash: `700a1c2acad955e09191add81a3112d3ce4acc2ffd87487cb9a65bc2f9deed95`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { useNavigate } from "@tanstack/react-router";`
- Line 3: `import { refreshCsrfToken } from "@/api/csrf";`
- Line 4: `import { useLogout } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/features/session/useSignOut.ts#L7): `export function useSignOut(): () => Promise<void> {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/shell/AppShell.tsx:27](../../../../../../frontend/src/features/shell/AppShell.tsx#L27)
- [frontend/src/routes/select-centre.tsx:29](../../../../../../frontend/src/routes/select-centre.tsx#L29)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { refreshCsrfToken } from "@/api/csrf";
import { useLogout } from "@/api/generated/tutoring-centre";

/** Logs out, refreshes the CSRF token (bound to the now-signed-out user), clears every cached query, then returns to login. */
export function useSignOut(): () => Promise<void> {
  const logoutMutation = useLogout();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  return async () => {
    await logoutMutation.mutateAsync();
    await refreshCsrfToken();
    queryClient.clear();
    await navigate({ to: "/login" });
  };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
