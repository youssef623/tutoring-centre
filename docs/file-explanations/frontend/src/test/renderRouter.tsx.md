# `frontend/src/test/renderRouter.tsx`

## Purpose

Source comment (intent): Renders the app's real route tree (not a copy built for tests) at `initialPath`, with a fresh query client wired the same way main.tsx wires the real one — including the session-expiry redirect hook-up, so a global 401 during a test navigates through this test's own router, not a stale one from an earlier test.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/test/renderRouter.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,232 bytes. Source hash: `4246c9d88e953b217b9bfc3e63afe3edcd31461e5cc0f899e4f2e4ec407def1a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { QueryClientProvider } from "@tanstack/react-query";`
- Line 2: `import { RouterProvider, createMemoryHistory, createRouter } from "@tanstack/react-router";`
- Line 3: `import { render } from "@testing-library/react";`
- Line 4: `import { createAppQueryClient, setLoginRedirect } from "@/app/queryClient";`
- Line 5: `import { routeTree } from "@/routeTree.gen";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../frontend/src/test/renderRouter.tsx#L12): `export function renderRouter(initialPath: string) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/dashboard/DashboardOverview.test.tsx:6](../../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx#L6)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:8](../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L8)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:8](../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L8)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:7](../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L7)
- [frontend/src/features/enrollments/TransferDialog.test.tsx:7](../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L7)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:8](../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L8)
- [frontend/src/features/schedule/ExtendScheduleDialog.test.tsx:8](../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx#L8)
- [frontend/src/features/schedule/GroupScheduleTab.test.tsx:8](../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx#L8)
- [frontend/src/features/schedule/TimetablePage.test.tsx:7](../../../../../frontend/src/features/schedule/TimetablePage.test.tsx#L7)
- [frontend/src/routes/_authenticated.test.tsx:5](../../../../../frontend/src/routes/_authenticated.test.tsx#L5)
- [frontend/src/routes/_authenticated/audit.test.tsx:7](../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L7)
- [frontend/src/routes/_authenticated/groups.$groupId.test.tsx:6](../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L6)
- [frontend/src/routes/_authenticated/groups.index.test.tsx:6](../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L6)
- [frontend/src/routes/_authenticated/parents.$parentId.test.tsx:6](../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx#L6)
- [frontend/src/routes/_authenticated/settings.test.tsx:7](../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L7)
- [frontend/src/routes/_authenticated/staff.test.tsx:7](../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L7)
- [frontend/src/routes/_authenticated/students.$studentId.test.tsx:6](../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L6)
- [frontend/src/routes/_authenticated/students.index.test.tsx:6](../../../../../frontend/src/routes/_authenticated/students.index.test.tsx#L6)
- [frontend/src/routes/_authenticated/students.new.test.tsx:7](../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L7)
- [frontend/src/routes/_authenticated/subjects.test.tsx:6](../../../../../frontend/src/routes/_authenticated/subjects.test.tsx#L6)
- 4 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider, createMemoryHistory, createRouter } from "@tanstack/react-router";
import { render } from "@testing-library/react";
import { createAppQueryClient, setLoginRedirect } from "@/app/queryClient";
import { routeTree } from "@/routeTree.gen";

/**
 * Renders the app's real route tree (not a copy built for tests) at `initialPath`, with a fresh query
 * client wired the same way main.tsx wires the real one — including the session-expiry redirect hook-up,
 * so a global 401 during a test navigates through this test's own router, not a stale one from an earlier test.
 */
export function renderRouter(initialPath: string) {
  const queryClient = createAppQueryClient();
  const history = createMemoryHistory({ initialEntries: [initialPath] });
  const router = createRouter({ routeTree, history, context: { queryClient } });

  setLoginRedirect((redirectTarget) => {
    void router.navigate({ to: "/login", search: { redirect: redirectTarget } });
  });

  const utils = render(
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>,
  );

  return { ...utils, router, queryClient };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
