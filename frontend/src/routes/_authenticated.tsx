import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { meQueryOptions } from "@/features/session/meQueryOptions";

/**
 * Pathless layout: groups every protected page under one guard without adding a URL segment.
 *
 * This guard only decides what the UI shows; it is not a security boundary — the API is. A user who
 * bypasses it still gets 401/403 from every endpoint, same as Day 16's fallback authorization policy.
 */
export const Route = createFileRoute("/_authenticated")({
  beforeLoad: async ({ context, location }) => {
    const me = await context.queryClient.ensureQueryData(meQueryOptions);

    if (me === null) {
      throw redirect({ to: "/login", search: { redirect: location.href } });
    }

    if (me.activeCentreId === null) {
      throw redirect({ to: "/select-centre" });
    }
  },
  component: AuthenticatedLayout,
});

function AuthenticatedLayout() {
  return <Outlet />;
}
