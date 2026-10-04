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
