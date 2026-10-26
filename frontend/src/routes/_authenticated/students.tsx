import { Outlet, createFileRoute } from "@tanstack/react-router";

/**
 * Pathless-in-effect layout: "/students/new" and "/students/$studentId" (Day 42) are TanStack Router children
 * of this file by the flat-routing convention (their generated `getParentRoute` points here), so without this
 * `Outlet` their content would never actually reach the screen — the parent's own route would render instead,
 * with the child route's component loaded but nowhere to mount. The list itself lives in `students.index.tsx`,
 * the index route for exactly "/students", the same split `_authenticated.tsx` / `_authenticated/index.tsx`
 * already uses for the dashboard.
 */
export const Route = createFileRoute("/_authenticated/students")({
  component: StudentsLayout,
});

function StudentsLayout() {
  return <Outlet />;
}
