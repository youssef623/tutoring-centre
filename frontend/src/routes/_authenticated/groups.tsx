import { Outlet, createFileRoute } from "@tanstack/react-router";

/**
 * Layout for "/groups" (list, `groups.index.tsx`) and "/groups/$groupId" (detail): by the flat-routing
 * convention the detail route is a child of this file, so without this `Outlet` its content would never
 * reach the screen. Same split as `students.tsx`.
 */
export const Route = createFileRoute("/_authenticated/groups")({
  component: GroupsLayout,
});

function GroupsLayout() {
  return <Outlet />;
}
