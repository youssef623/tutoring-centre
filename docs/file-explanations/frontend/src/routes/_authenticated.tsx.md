# `frontend/src/routes/_authenticated.tsx`

## Purpose

Source comment (intent): Pathless layout: groups every protected page under one guard without adding a URL segment. This guard only decides what the UI shows; it is not a security boundary — the API is. A user who bypasses it still gets 401/403 from every endpoint, same as Day 16's fallback authorization policy.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/routes/_authenticated.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 48 lines and 1,779 bytes. Source hash: `20eb0202a27c0bd87a95d2669e062c1f539a2e9210b4913b43f07ec1c4f06919`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 3: `import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";`
- Line 4: `import { AppShell } from "@/features/shell/AppShell";`
- Line 5: `import { meQueryOptions } from "@/features/session/meQueryOptions";`
- Line 6: `import { useSession } from "@/features/session/useSession";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../frontend/src/routes/_authenticated.tsx#L14): `export const Route = createFileRoute("/_authenticated")({`
- [Line 35](../../../../../frontend/src/routes/_authenticated.tsx#L35): `function AuthenticatedLayout() {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
/* eslint-disable @typescript-eslint/only-throw-error -- TanStack Router's documented redirect idiom:
 * `redirect()` returns a Response, which the router's own data loader catches, not an Error. */
import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { AppShell } from "@/features/shell/AppShell";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { useSession } from "@/features/session/useSession";

/**
 * Pathless layout: groups every protected page under one guard without adding a URL segment.
 *
 * This guard only decides what the UI shows; it is not a security boundary — the API is. A user who
 * bypasses it still gets 401/403 from every endpoint, same as Day 16's fallback authorization policy.
 */
export const Route = createFileRoute("/_authenticated")({
  beforeLoad: async ({ context, location }) => {
    const me = await context.queryClient.query({ ...meQueryOptions, staleTime: "static" });

    if (me === null) {
      throw redirect({ to: "/login", search: { redirect: location.href } });
    }

    // The server-side gate (Day 30) is the real control; this only sends the user to the pleasant form for it
    // instead of letting every other request 403 with auth.password_change_required.
    if (me.mustChangePassword) {
      throw redirect({ to: "/change-password" });
    }

    if (me.activeCentreId === null) {
      throw redirect({ to: "/select-centre" });
    }
  },
  component: AuthenticatedLayout,
});

function AuthenticatedLayout() {
  const { me } = useSession();

  // The guard above already resolved a signed-in user with an active centre before this renders.
  if (me === null) {
    return null;
  }

  return (
    <AppShell me={me}>
      <Outlet />
    </AppShell>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
