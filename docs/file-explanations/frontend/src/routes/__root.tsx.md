# `frontend/src/routes/__root.tsx`

## Purpose

Source comment (intent): Available to every route's `beforeLoad`/`loader` (Day 17: the guard reads the session through it).

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/routes/__root.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 791 bytes. Source hash: `01275bad2ff296b5254c4c1461c34bfdfcf0e66143833aae1020d84d2ab80567`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { QueryClient } from "@tanstack/react-query";`
- Line 2: `import { Outlet, createRootRouteWithContext } from "@tanstack/react-router";`
- Line 3: `import { DemoBanner } from "@/features/status/DemoBanner";`
- Line 4: `import { Toaster } from "@/components/ui/sonner";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../frontend/src/routes/__root.tsx#L7): `export interface RouterContext {`
- [Line 11](../../../../../frontend/src/routes/__root.tsx#L11): `export const Route = createRootRouteWithContext<RouterContext>()({`
- [Line 15](../../../../../frontend/src/routes/__root.tsx#L15): `function RootLayout() {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import type { QueryClient } from "@tanstack/react-query";
import { Outlet, createRootRouteWithContext } from "@tanstack/react-router";
import { DemoBanner } from "@/features/status/DemoBanner";
import { Toaster } from "@/components/ui/sonner";

/** Available to every route's `beforeLoad`/`loader` (Day 17: the guard reads the session through it). */
export interface RouterContext {
  queryClient: QueryClient;
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootLayout,
});

function RootLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <DemoBanner />
      <Outlet />
      {/* One toaster for the whole app; Day 12's error utilities show API failures here. */}
      <Toaster richColors />
    </div>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
