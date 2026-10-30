/* eslint-disable @typescript-eslint/only-throw-error -- TanStack Router redirect contract. */
import { createFileRoute, redirect, Outlet, useLocation } from "@tanstack/react-router";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { useSession } from "@/features/session/useSession";
import { isPortalRole, sessionHome } from "@/features/session/roleShell";
import { PortalShell } from "@/features/portal/PortalShell";

export const Route = createFileRoute("/portal")({
  beforeLoad: async ({ context }) => {
    const me = await context.queryClient.query({ ...meQueryOptions, staleTime: "static" });
    if (me === null) throw redirect({ to: "/login" });
    if (me.mustChangePassword || me.activeCentreId === null)
      throw redirect({ href: sessionHome(me) });
    if (!isPortalRole(me.activeRole)) throw redirect({ to: "/" });
  },
  component: PortalPage,
});

function PortalPage() {
  const { me } = useSession();
  const path = useLocation({ select: (location) => location.pathname });
  return me === null ? null : (
    <PortalShell me={me}>{path === "/portal" ? undefined : <Outlet />}</PortalShell>
  );
}
