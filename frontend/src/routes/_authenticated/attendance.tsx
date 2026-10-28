import { Outlet, createFileRoute } from "@tanstack/react-router";

/**
 * Layout for "/attendance" (today's sessions, `attendance.index.tsx`) and "/attendance/sessions/$sessionId"
 * (the roster): by the flat-routing convention the roster route is a child of this file, so without this
 * `Outlet` its content would never reach the screen. Same split as `groups.tsx`.
 */
export const Route = createFileRoute("/_authenticated/attendance")({
  component: AttendanceLayout,
});

function AttendanceLayout() {
  return <Outlet />;
}
